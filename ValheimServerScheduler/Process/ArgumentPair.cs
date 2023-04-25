namespace ValheimServerScheduler.Process;

internal class ArgumentPair<TValue> : Argument
{
    public TValue Value { get; }

    public ArgumentPair(string key, TValue value) : base(key)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value is string
            ? $"-{Key} \"{Value}\""
            : $"-{Key} {Value}";
    }
}