using System.Diagnostics;
using ValheimServerScheduler.Process.Lifecycle;

namespace ValheimServerScheduler.Process;
using Process = System.Diagnostics.Process;

internal sealed class ProcessManager
{
    private readonly ProcessInfo _info;
    private readonly IProcessLifecycleManager _lifecycleManager;
    
    private Process? _spawnedInstance = null;

    public ProcessManager(ProcessInfo info, IProcessLifecycleManager lifecycleManager)
    {
        _info = info;
        _lifecycleManager = lifecycleManager;
    }

    public void Start()
    {
        if (_spawnedInstance is not null)
            throw new InvalidOperationException("A spawned instance already exists");

        ProcessStartInfo processInfo = _lifecycleManager.CreateStartInfo(_info);
        _spawnedInstance = Process.Start(processInfo);
        
        if (_spawnedInstance is null)
            throw new InvalidOperationException("Failed to start the process.");
        
        _spawnedInstance.Exited += (_, _) => { _spawnedInstance = null; };
    }

    public void KillSpawnedInstance()
    {
        if (_spawnedInstance is null)
            return;

        _lifecycleManager.KillProcess(_spawnedInstance);
        _spawnedInstance = null;
    }

    public void KillAllInstances()
    {
        KillSpawnedInstance();
        foreach (Process proc in GetRunningInstances())
        {
            proc.Kill();
        }
    }

    public bool IsSpawnedInstanceRunning() => _spawnedInstance != null;

    public bool IsExternalInstanceRunning() => GetRunningInstances().Any(x => x == _spawnedInstance);

    private IEnumerable<Process> GetRunningInstances()
    {
        return Process.GetProcesses()
            .Where(proc => proc.ProcessName == _info.ProcessName)
            .ToArray();
    }
}