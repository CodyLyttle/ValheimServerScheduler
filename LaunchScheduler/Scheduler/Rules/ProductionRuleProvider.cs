namespace LaunchScheduler.Scheduler.Rules;


// Hardcoded production values, to eventually be replaced by a file based provider.
public class ProductionRuleProvider : IRuleProvider
{
    private static readonly TimeSpan WeekdayStartTime = DateTime.Parse("4 PM").TimeOfDay;
    private static readonly TimeSpan WeekdayStopTime = DateTime.Parse("11:59 PM").TimeOfDay;
    private static readonly TimeSpan MorningRestartTime = DateTime.Parse("4 AM").TimeOfDay;
    private static readonly TimeSpan AfternoonRestartTime = DateTime.Parse("4 PM").TimeOfDay;
    
    public bool HasRuleSetChanged()
    {
        return false;
    }

    public IEnumerable<SchedulerRule> GetRules()
    {
        return new List<SchedulerRule>
        {
            // Weekdays
            new(DayOfWeek.Monday, WeekdayStartTime, SchedulerAction.Start),
            new(DayOfWeek.Monday, WeekdayStopTime, SchedulerAction.Stop),
            new(DayOfWeek.Tuesday, WeekdayStartTime, SchedulerAction.Start),
            new(DayOfWeek.Tuesday, WeekdayStopTime, SchedulerAction.Stop),
            new(DayOfWeek.Wednesday, WeekdayStartTime, SchedulerAction.Start),
            new(DayOfWeek.Wednesday, WeekdayStopTime, SchedulerAction.Stop),
            new(DayOfWeek.Thursday, WeekdayStartTime, SchedulerAction.Start),
            new(DayOfWeek.Thursday, WeekdayStopTime, SchedulerAction.Stop),
            new(DayOfWeek.Friday, WeekdayStartTime, SchedulerAction.Start),
            // Weekends
            new(DayOfWeek.Saturday, MorningRestartTime, SchedulerAction.Restart),
            new(DayOfWeek.Saturday, AfternoonRestartTime, SchedulerAction.Restart),
            new(DayOfWeek.Sunday, MorningRestartTime, SchedulerAction.Restart),
            new(DayOfWeek.Sunday, AfternoonRestartTime, SchedulerAction.Restart),
            new(DayOfWeek.Sunday, WeekdayStopTime, SchedulerAction.Stop)
        };
    }
}