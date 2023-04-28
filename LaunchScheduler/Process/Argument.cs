namespace LaunchScheduler.Process;

/// <summary>
/// Represents a command line argument with a key.
/// </summary>
public class Argument
{
    /// <summary>
    /// Gets the key of the argument.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Argument"/> class.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    public Argument(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Returns a string representation of the argument, formatted as "-Key".
    /// </summary>
    /// <returns>The formatted string.</returns>
    public override string ToString()
    {
        return $"-{Key}";
    }
}