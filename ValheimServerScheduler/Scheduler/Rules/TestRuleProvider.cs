namespace ValheimServerScheduler.Scheduler.Rules;

/// <summary>
/// An implementation of IRuleProvider that provides a set of rules for quickly testing the scheduler.
/// </summary>
internal class TestRuleProvider : IRuleProvider
{
    public bool HasRuleSetChanged()
    {
        return false;
    }

    /// <summary>
    /// Generates a set of test rules that utilize each scheduler action.
    /// The rules are scheduled to begin shortly after the current time, with minimal delay between rules.
    /// </summary>
    /// <returns>A list of newly generated test rules.</returns>
    public IEnumerable<SchedulerRule> GetRules()
    {
        // Use the current time as the starting point for scheduling rules.
        DateTime sharedTime = DateTime.Now;

        return new List<SchedulerRule>
        {
            CreateRule(ref sharedTime, 10, SchedulerAction.Start),
            CreateRule(ref sharedTime, 30, SchedulerAction.Stop),
            CreateRule(ref sharedTime, 30, SchedulerAction.Start),
            CreateRule(ref sharedTime, 30, SchedulerAction.Restart),
            CreateRule(ref sharedTime, 60, SchedulerAction.Stop)
        };
    }

    private static SchedulerRule CreateRule(ref DateTime lastRuleTime, int delayInSeconds, SchedulerAction action)
    {
        lastRuleTime = lastRuleTime.AddSeconds(delayInSeconds);
        TimeSpan scheduledTime = lastRuleTime.TimeOfDay;

        return new SchedulerRule(lastRuleTime.DayOfWeek, scheduledTime, action);
    }
}