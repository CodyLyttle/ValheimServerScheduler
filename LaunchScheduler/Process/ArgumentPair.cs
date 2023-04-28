namespace LaunchScheduler.Process;

/// <summary>
/// Represents a command line argument with a key and a value.
/// </summary>
/// <typeparam name="TValue">The type of the value associated with the argument.</typeparam>
public class ArgumentPair<TValue> : Argument
{
    /// <summary>
    /// Gets the value of the argument.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentPair{TValue}"/> class.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The value of the argument.</param>
    public ArgumentPair(string key, TValue value) : base(key)
    {
        Value = value;
    }
    
    /// <summary>
    /// Returns a string representation of the argument pair, formatted as "-Key Value" or "-Key "Value"" for string values.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public override string ToString()
    {
        return Value is string
            ? $"-{Key} \"{Value}\""
            : $"-{Key} {Value}";
    }
}