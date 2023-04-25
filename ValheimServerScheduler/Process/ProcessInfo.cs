namespace ValheimServerScheduler.Process;

internal sealed class ProcessInfo
{
    public string ProcessName { get; }
    public string FilePath { get; }
    public Argument[] Args { get; }

    public ProcessInfo(string processName, string filePath, params Argument[] args)
    {
        if (string.IsNullOrEmpty(processName))
            throw new ArgumentException("Process name cannot be null or empty.", processName);

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", filePath);

        if (!Path.IsPathRooted(filePath))
            throw new ArgumentException("File path must be an absolute path.", filePath);

        if (!File.Exists(filePath))
            throw new ArgumentException("File path must exist and point to a file.", processName);

        string fileExtension = Path.GetExtension(filePath);
        if (fileExtension != ".exe" && fileExtension != ".bat")
            throw new ArgumentException("File path must point to an executable or batch file.");

        ProcessName = processName;
        FilePath = filePath;
        Args = args;
    }
}