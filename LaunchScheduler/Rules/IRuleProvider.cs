namespace LaunchScheduler.Rules;

/// <summary>
/// Defines a contract for providing a set of scheduler rules.
/// </summary>
public interface IRuleProvider
{
    /// <summary>
    /// Determines if the set of scheduler rules has changed since the last time they were provided.
    /// </summary>
    /// <returns>True if the rule set has changed, false otherwise.</returns>
    bool HasRuleSetChanged();
    
    /// <summary>
    /// Provides a set of scheduler rules.
    /// </summary>
    /// <returns>An IEnumerable collection of SchedulerRule objects representing the provided rules.</returns>
    IEnumerable<SchedulerRule> GetRules();
}