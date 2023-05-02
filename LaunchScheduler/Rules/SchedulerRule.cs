namespace LaunchScheduler.Rules;

/// <summary>
/// Represents a scheduled action to be performed at a specific time and day of the week.
/// </summary>
public readonly struct SchedulerRule
{
    /// <summary>
    /// Gets the action to be performed.
    /// </summary>
    public SchedulerAction Action { get; }
    
    /// <summary>
    /// Gets the day of the week on which the action is scheduled for.
    /// </summary>
    public DayOfWeek DayOfWeek { get; }
    
    /// <summary>
    /// Gets the time of day at which the action is scheduled for.
    /// </summary>
    public TimeSpan TimeOfDay { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerRule"/> class.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week on which the action is scheduled for.</param>
    /// <param name="timeOfDay">The time of day at which the action is scheduled for.</param>
    /// <param name="action">The action to be performed.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified time of day is not between 00:00:00 and 23:59:59.</exception>
    public SchedulerRule(DayOfWeek dayOfWeek, TimeSpan timeOfDay, SchedulerAction action)
    {
        if (timeOfDay < TimeSpan.Zero || timeOfDay >= TimeSpan.FromDays(1))
            throw new ArgumentOutOfRangeException(nameof(timeOfDay), 
                "Time of day must be between 00:00:00 and 23:59:59");
        
        Action = action;
        DayOfWeek = dayOfWeek;
        TimeOfDay = timeOfDay;
    }
}