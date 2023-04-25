namespace ValheimServerScheduler.Process;

internal class Argument
{
    public string Key { get; }

    public Argument(string key)
    {
        Key = key;
    }

    public override string ToString()
    {
        return $"-{Key}";
    }
}