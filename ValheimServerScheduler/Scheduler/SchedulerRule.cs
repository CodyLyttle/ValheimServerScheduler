namespace ValheimServerScheduler.Scheduler;

internal readonly struct SchedulerRule
{
    public SchedulerAction Action { get; }
    
    public TimeSpan TimeOfDay { get; }
    
    public SchedulerRule(SchedulerAction action, TimeSpan timeOfDay)
    {
        if (timeOfDay < TimeSpan.Zero || timeOfDay >= TimeSpan.FromDays(1))
            throw new ArgumentOutOfRangeException(nameof(timeOfDay), 
                "Time of day must be between 00:00:00 and 23:59:59");
        
        Action = action;
        TimeOfDay = timeOfDay;
    }
}