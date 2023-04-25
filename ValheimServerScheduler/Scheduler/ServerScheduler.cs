using System.Diagnostics;
using ValheimServerScheduler.Process;

namespace ValheimServerScheduler.Scheduler;

internal sealed class ServerScheduler
{
    private readonly ProcessManager _processManager;
    private readonly Dictionary<DayOfWeek, List<SchedulerRule>> _rules;

    // Estimates used for delay.
    public TimeSpan EstimatedStartupTime { get; }
    public TimeSpan EstimatedShutdownTime { get; }

    public ServerScheduler(ProcessManager processManager, TimeSpan estimatedStartupTime, TimeSpan estimatedShutdownTime)
    {
        _processManager = processManager;
        EstimatedStartupTime = estimatedStartupTime;
        EstimatedShutdownTime = estimatedShutdownTime;

        // Initialize an empty list of rules for each day of the week.
        _rules = new Dictionary<DayOfWeek, List<SchedulerRule>>();
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            _rules[day] = new List<SchedulerRule>();
        }
    }

    public ServerScheduler AddRule(DayOfWeek day, SchedulerAction action, TimeSpan timeOfDay)
        => AddRule(day, new SchedulerRule(action, timeOfDay));

    public ServerScheduler AddRule(DayOfWeek day, SchedulerRule rule)
    {
        if (!Enum.IsDefined(typeof(DayOfWeek), day))
        {
            throw new ArgumentException("Invalid day of the week", nameof(day));
        }

        List<SchedulerRule> dayRules = _rules[day];
        if (dayRules.Any(r => r.TimeOfDay == rule.TimeOfDay))
        {
            throw new ArgumentException($"A scheduling rule already exists for day {day} and time {rule.TimeOfDay}",
                nameof(rule));
        }

        dayRules.Add(rule);
        dayRules.Sort((ruleA, ruleB) => ruleA.TimeOfDay.CompareTo(ruleB.TimeOfDay));

        return this;
    }

    public async Task Run()
    {
        DayOfWeek today = DateTime.Today.DayOfWeek;

        // Begin with only the remaining rules for the day.
        // Executing a series of overdue rules sequentially could result in unexpected behaviour.
        Queue<SchedulerRule> remainingDailyRules = GetUpcomingDailyRules();

        while (true)
        {
            if (today != DateTime.Today.DayOfWeek)
            {
                today = DateTime.Today.DayOfWeek;
                remainingDailyRules = GetAllDailyRules();
            }

            // Rule ready to execute.
            if (remainingDailyRules.TryPeek(out SchedulerRule nextRule)
                && DateTime.Now.TimeOfDay.CompareTo(nextRule.TimeOfDay) >= 0)
            {
                switch (nextRule.Action)
                {
                    case SchedulerAction.Start:
                        await Start();
                        break;
                    case SchedulerAction.Stop:
                        await Stop();
                        break;
                    case SchedulerAction.Restart:
                        await Restart();
                        break;
                }

                remainingDailyRules.Dequeue();
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private Queue<SchedulerRule> GetUpcomingDailyRules()
    {
        Queue<SchedulerRule> dailyRules = GetAllDailyRules();

        while (dailyRules.TryPeek(out SchedulerRule nextRule))
        {
            // Time of day is later then scheduled rule.
            if (DateTime.Now.TimeOfDay.CompareTo(nextRule.TimeOfDay) > 0)
            {
                dailyRules.Dequeue();
            }
            else
            {
                break;
            }
        }

        return dailyRules;
    }

    private Queue<SchedulerRule> GetAllDailyRules()
    {
        return new Queue<SchedulerRule>(_rules[DateTime.Today.DayOfWeek]);
    }

    private async Task Start()
    {
        Debug.WriteLine("+Start");
        _processManager.Start();
        await Task.Delay(EstimatedStartupTime);
        Debug.WriteLine("-Start");
    }

    private async Task Stop()
    {
        Debug.WriteLine("+Stop");
        _processManager.KillSpawnedInstance();
        await Task.Delay(EstimatedShutdownTime);
        Debug.WriteLine("-Stop");
    }

    private async Task Restart()
    {
        Debug.WriteLine("+Restart");
        await Stop();
        await Start();
        Debug.WriteLine("-Restart");
    }
}