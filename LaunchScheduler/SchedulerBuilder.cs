using LaunchScheduler.Process;
using LaunchScheduler.Process.Lifecycle;
using LaunchScheduler.Rules;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LaunchScheduler;

/// <summary>
/// Provides a fluent interface for constructing a <see cref="Scheduler"/> instance.
/// </summary>
public class SchedulerBuilder
{
    private ILogger? _logger;
    private ProcessInfo? _processInfo;
    private IProcessLifecycleManager? _lifecycleManager;
    private IRuleProvider? _ruleProvider;
    private TimeSpan? _startupGracePeriod;
    private TimeSpan? _shutdownGracePeriod;
    
    /// <summary>
    /// Sets the logger instance used for logging events, warnings, and errors within the scheduler.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    /// <summary>
    /// Sets information about the scheduled process.
    /// </summary>
    /// <param name="info">Information about the process.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetProcessInfo(ProcessInfo info)
    {
        _processInfo = info;
        return this;
    }

    /// <summary>
    /// Sets information about the scheduled process.
    /// </summary>
    /// <param name="name">The name of the process.</param>
    /// <param name="path">The file path of the process.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetProcessInfo(string name, string path)
    {
        _processInfo = new ProcessInfo(name, path);
        return this;
    }

    /// <summary>
    /// Sets the lifecycle manager responsible for managing the lifecycle of a scheduled process.
    /// </summary>
    /// <param name="lifecycleManager">The lifecycle manager.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetLifecycleManager(IProcessLifecycleManager lifecycleManager)
    {
        _lifecycleManager = lifecycleManager;
        return this;
    }

    /// <summary>
    /// Sets the rule provider responsible for providing scheduling rules to the scheduler.
    /// </summary>
    /// <param name="ruleProvider">The rule provider.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetRuleProvider(IRuleProvider ruleProvider)
    {
        _ruleProvider = ruleProvider;
        return this;
    }

    /// <summary>
    /// Sets the grace period allowed for the process to complete its startup phase. 
    /// </summary>
    /// <param name="gracePeriod">The duration of the grace period.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetStartupGracePeriod(TimeSpan gracePeriod)
    {
        _startupGracePeriod = gracePeriod;
        return this;
    }
    
    /// <summary>
    /// Sets the grace period allowed for the process to complete its shutdown phase. 
    /// </summary>
    /// <param name="gracePeriod">The duration of the grace period.</param>
    /// <returns>The updated <see cref="SchedulerBuilder"/> instance.</returns>
    public SchedulerBuilder SetShutdownGracePeriod(TimeSpan gracePeriod)
    {
        _shutdownGracePeriod = gracePeriod;
        return this;
    }

    /// <summary>
    /// Constructs a new <see cref="Scheduler"/> instance with the provided configuration.
    /// </summary>
    /// <returns>The <see cref="Scheduler"/> instance created using the current builder configuration.</returns>
    /// <exception cref="InvalidOperationException">A required dependency has not been set via the respective builder function.</exception>
    public Scheduler Build()
    {
        if (_processInfo == null)
        {
            throw new InvalidOperationException($"{nameof(ProcessInfo)} is required.");
        }

        if (_lifecycleManager == null)
        {
            throw new InvalidOperationException($"{nameof(IProcessLifecycleManager)} is required.");
        }

        if (_ruleProvider == null)
        {
            throw new InvalidOperationException($"{nameof(IRuleProvider)} is required.");
        }

        _logger ??= NullLogger.Instance;

        ProcessManager processManager = new(_processInfo, _lifecycleManager, _logger);
        if (_startupGracePeriod != null)
        {
            processManager.StartupGracePeriod = _startupGracePeriod.Value;
        }

        if (_shutdownGracePeriod != null)
        {
            processManager.ShutdownGracePeriod = _shutdownGracePeriod.Value;
        }

        return new Scheduler(processManager, _ruleProvider, _logger);
    }
}