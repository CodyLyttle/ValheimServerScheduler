using System.Diagnostics;
using LaunchScheduler.Process;
using LaunchScheduler.Scheduler.Rules;

namespace LaunchScheduler.Scheduler;

public sealed class Scheduler
{
    private readonly ProcessManager _processManager;

    private IRuleProvider _ruleProvider;
    private Dictionary<DayOfWeek, IEnumerable<SchedulerRule>>? _ruleset;

    public IRuleProvider RuleProvider
    {
        get => _ruleProvider;
        set
        {
            if (value == _ruleProvider) return;
            _ruleProvider = value;
            _ruleset = null;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Scheduler"/> class.
    /// </summary>
    /// <param name="ruleProvider">The rule provider to obtain the scheduler rules from.</param>
    /// <param name="processManager">The process manager used to start, stop and restart the managed process.</param>
    public Scheduler(IRuleProvider ruleProvider, ProcessManager processManager)
    {
        _ruleProvider = ruleProvider;
        _processManager = processManager;
    }

    /// <summary>
    /// Runs the scheduler loop, executing scheduled actions based on the provided rules.
    /// The loop runs indefinitely until the specified cancellation token is triggered.
    /// </summary>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to cancel the loop execution.</param>
    public async Task RunSchedulerLoop(CancellationToken cancellationToken = default)
    {
        DayOfWeek today = DateTime.Today.DayOfWeek;
        Queue<SchedulerRule> remainingDailyRules = new();

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            if (_ruleset == null || _ruleProvider.HasRuleSetChanged())
            {
                // Ignore overdue rules to prevent unexpected behaviour when executing rules in quick succession.
                _ruleset = CreateRuleset();
                remainingDailyRules = GetUpcomingDailyRules();
            }

            if (today != DateTime.Today.DayOfWeek)
            {
                today = DateTime.Today.DayOfWeek;
                remainingDailyRules = GetDailyRules();
            }

            // Rule ready to execute.
            if (remainingDailyRules.TryPeek(out SchedulerRule nextRule)
                && DateTime.Now.TimeOfDay.CompareTo(nextRule.TimeOfDay) >= 0)
            {
                try
                {
                    switch (nextRule.Action)
                    {
                        case SchedulerAction.Start:
                            await Start(cancellationToken);
                            break;
                        case SchedulerAction.Stop:
                            await Stop(cancellationToken);
                            break;
                        case SchedulerAction.Restart:
                            await Restart(cancellationToken);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                remainingDailyRules.Dequeue();
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private Dictionary<DayOfWeek, IEnumerable<SchedulerRule>> CreateRuleset()
    {
        Dictionary<DayOfWeek, IEnumerable<SchedulerRule>> ruleset = _ruleProvider.GetRules()
            .OrderBy(x => x.TimeOfDay)
            .GroupBy(x => x.DayOfWeek)
            .ToDictionary(grp => grp.Key, grp => grp.AsEnumerable());

        foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
        {
            if (!ruleset.ContainsKey(dayOfWeek))
            {
                ruleset[dayOfWeek] = Enumerable.Empty<SchedulerRule>();
            }
        }

        return ruleset;
    }

    private Queue<SchedulerRule> GetUpcomingDailyRules()
    {
        Queue<SchedulerRule> dailyRules = GetDailyRules();

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

    private Queue<SchedulerRule> GetDailyRules()
    {
        Debug.Assert(_ruleset != null);
        return new Queue<SchedulerRule>(_ruleset[DateTime.Today.DayOfWeek]);
    }

    private async Task Start(CancellationToken cancellationToken = default)
    {
        Debug.WriteLine("+Start");
        await _processManager.Start(cancellationToken);
        Debug.WriteLine("-Start");
    }

    private async Task Stop(CancellationToken cancellationToken = default)
    {
        Debug.WriteLine("+Stop");
        await _processManager.StopSpawnedInstance(cancellationToken);
        Debug.WriteLine("-Stop");
    }

    private async Task Restart(CancellationToken cancellationToken = default)
    {
        Debug.WriteLine("+Restart");
        await _processManager.StopSpawnedInstance(cancellationToken);
        await _processManager.Start(cancellationToken);
        Debug.WriteLine("-Restart");
    }
}