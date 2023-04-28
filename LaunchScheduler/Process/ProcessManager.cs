using System.ComponentModel;
using System.Diagnostics;
using LaunchScheduler.Process.Lifecycle;

namespace LaunchScheduler.Process;

using Process = System.Diagnostics.Process;

/// <summary>
/// Manages the lifecycle of a specific process, providing methods for starting, stopping, and determining it's state.
/// </summary>
public sealed class ProcessManager : IDisposable
{
    private readonly ProcessInfo _processInfo;
    private readonly IProcessLifecycleManager _lifecycleManager;

    private Process? _spawnedInstance;

    /// <summary>
    /// Gets or sets the grace period for process startup.
    /// </summary>
    public TimeSpan StartupGracePeriod { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets or sets the grace period for process shutdown.
    /// </summary>
    public TimeSpan ShutdownGracePeriod { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessManager"/> class.
    /// </summary>
    /// <param name="processInfo">The details of the process to be managed.</param>
    /// <param name="lifecycleManager">The lifecycle manager for the process.</param>
    public ProcessManager(ProcessInfo processInfo, IProcessLifecycleManager lifecycleManager)
    {
        _processInfo = processInfo;
        _lifecycleManager = lifecycleManager;
    }

    /// <summary>
    /// Starts the managed process and awaits the startup grace period.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the awaited grace period.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a spawned instance already exists or if the process fails to start.
    /// </exception>
    public async Task Start(CancellationToken cancellationToken = default)
    {
        if (_spawnedInstance is not null && !_spawnedInstance.HasExited)
            throw new InvalidOperationException("A spawned instance already exists");

        ProcessStartInfo processInfo = _lifecycleManager.CreateStartInfo(_processInfo);
        _spawnedInstance = Process.Start(processInfo);

        if (_spawnedInstance is null)
            throw new InvalidOperationException("Failed to start the process.");

        _spawnedInstance.Exited += (_, _) =>
        {
            _spawnedInstance.Dispose();
            _spawnedInstance = null;
        };

        try
        {
            await Task.Delay(StartupGracePeriod, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("The startup grace period was cancelled.");
            throw;
        }
    }

    /// <summary>
    /// Attempts to gracefully stop the managed process
    /// If the process is still alive after the grace period, it will be forcefully killed.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the awaited grace period.</param>
    public async Task StopSpawnedInstance(CancellationToken cancellationToken = default)
    {
        if (_spawnedInstance is null)
            return;

        // Ensure any previously spawned process is disposed of before replacing the reference.
        if (_spawnedInstance.HasExited)
        {
            _spawnedInstance.Dispose();
            _spawnedInstance = null;
            return;
        }

        _lifecycleManager.AttemptGracefulTermination(_spawnedInstance);

        try
        {
            await Task.Delay(ShutdownGracePeriod, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("The shutdown grace period was cancelled; forcefully killing the process before the grace period ended.");
            throw;
        }
        finally
        {
            if (_spawnedInstance != null)
            {
                _lifecycleManager.KillProcess(_spawnedInstance);
                _spawnedInstance = null;
            }
        }
    }

    /// <summary>
    /// Attempts to gracefully stop all instances of the managed process.
    /// If any instance is still alive after the grace period, it will be forcefully killed.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the awaited grace period.</param>
    public async Task StopAllInstances(CancellationToken cancellationToken = default)
    {
        Process[] instances = GetRunningInstances();
        if (instances.Length == 0)
            return;

        foreach (Process instance in instances)
        {
            _lifecycleManager.AttemptGracefulTermination(instance);
        }

        try
        {
            await Task.Delay(ShutdownGracePeriod, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            Debug.WriteLine("The shutdown grace period was cancelled; forcefully killing the processes before the grace period ended.");
            throw;
        }
        finally
        {
            foreach (Process instance in instances)
            {
                if (!instance.HasExited)
                    _lifecycleManager.KillProcess(instance);
                
                instance.Dispose();
            }
        }
    }

    /// <summary>
    /// Determines if the spawned instance of the managed process is running.
    /// </summary>
    /// <returns>True if the spawned instance is running, false otherwise.</returns>
    public bool IsSpawnedInstanceRunning() => _spawnedInstance is {HasExited: false};

    /// <summary>
    /// Determines if any external instances of the managed process are running.
    /// </summary>
    /// <returns>True if any external instances are running, false otherwise.</returns>
    public bool IsExternalInstanceRunning() => GetRunningInstances().Any(x => x != _spawnedInstance);

    private Process[] GetRunningInstances()
    {
        return Process.GetProcesses()
            .Where(proc => proc.ProcessName == _processInfo.Name)
            .Where(proc =>
            {
                try
                {
                    return string.Equals(proc.MainModule?.FileName, _processInfo.FilePath,
                        StringComparison.OrdinalIgnoreCase);
                }
                catch (Win32Exception)
                {
                    // Insufficient privileges.
                    return false;
                }
                catch (InvalidOperationException)
                {
                    // The process has exited.
                    return false;
                }
            })
            .ToArray();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && _spawnedInstance is not null)
        {
            _spawnedInstance.Dispose();
            _spawnedInstance = null;
        }
    }

    ~ProcessManager()
    {
        Dispose(false);
    }
}