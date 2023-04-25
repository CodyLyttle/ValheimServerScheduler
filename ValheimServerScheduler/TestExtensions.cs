using ValheimServerScheduler.Scheduler;

namespace ValheimServerScheduler;

internal static class TestExtensions
{
    public static void AddTestRules(this ServerScheduler scheduler)
    {
        DateTime time = DateTime.Now;
        
        AddRule(SchedulerAction.Start, 10);
        AddRule(SchedulerAction.Stop, 30);
        AddRule(SchedulerAction.Start, 30);
        AddRule(SchedulerAction.Restart, 30);
        AddRule(SchedulerAction.Stop, 60);

        void AddRule(SchedulerAction action, int secondsUntilAction)
        {
            time = time.AddSeconds(secondsUntilAction);
            scheduler.AddRule(time.DayOfWeek, new SchedulerRule(action, time.TimeOfDay));
        }
    }
}