namespace LaunchScheduler;

/// <summary>
/// Represents the available actions for a scheduler rule.
/// </summary>
public enum SchedulerAction
{
    /// <summary>
    /// Indicates that the scheduler should start a process.
    /// </summary>
    Start,
    
    /// <summary>
    /// Indicates that the scheduler should stop a running process.
    /// </summary>
    Stop,
    
    /// <summary>
    /// Indicates that the scheduler should restart a running process.
    /// </summary>
    Restart
}