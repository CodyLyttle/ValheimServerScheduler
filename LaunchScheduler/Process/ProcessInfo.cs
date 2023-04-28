namespace LaunchScheduler.Process;

/// <summary>
/// Represents the basic details of a process.
/// </summary>
public sealed class ProcessInfo
{
    /// <summary>
    /// Gets the name of the process.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets the file path of the process.
    /// </summary>
    public string FilePath { get; }
    
    /// <summary>
    /// Gets the arguments to use when launching the process.
    /// </summary>
    public Argument[] Args { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ProcessInfo"/> class.
    /// </summary>
    /// <param name="name">The name of the process.</param>
    /// <param name="filePath">The file path of the process.</param>
    /// <param name="args">The arguments to use when launching the process.</param>
    /// <exception cref="ArgumentException">Input arguments do not represent a valid .exe or .bat file.</exception>
    public ProcessInfo(string name, string filePath, params Argument[] args)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Process name cannot be null or empty.", name);

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", filePath);

        if (!Path.IsPathRooted(filePath))
            throw new ArgumentException("File path must be an absolute path.", filePath);

        if (!File.Exists(filePath))
            throw new ArgumentException("File path must exist and point to a file.", name);

        string fileExtension = Path.GetExtension(filePath);
        if (fileExtension != ".exe" && fileExtension != ".bat")
            throw new ArgumentException("File path must point to an executable or batch file.");

        Name = name;
        FilePath = filePath;
        Args = args;
    }
}