using System.Diagnostics;

namespace ValheimServerScheduler.Process.Lifecycle;

using Process = System.Diagnostics.Process;

/// <summary>
/// Manages the lifecycle of terminal-based processes.
/// </summary>
internal sealed class TerminalLifecycleManager : IProcessLifecycleManager
{
    /// <summary>
    /// Creates ProcessStartInfo for terminal-based processes based on the provided ProcessInfo.
    /// </summary>
    /// <param name="info">Details about the process to be started.</param>
    /// <returns>ProcessStartInfo configured for terminal-based processes.</returns>
    public ProcessStartInfo CreateStartInfo(ProcessInfo info)
    {
        return new ProcessStartInfo
        {
            FileName = info.FilePath,
            Arguments = string.Join(" ", info.Args.Select(arg => arg.ToString())),
            UseShellExecute = false
        };
    }
    
    /// <summary>
    /// Attempts to gracefully terminate the specified terminal-based process by closing its main window.
    /// </summary>
    /// <param name="process">The process to be gracefully terminated.</param>
    public void AttemptGracefulTermination(Process process)
    {
        process.CloseMainWindow();
    }

    /// <summary>
    /// Terminates the specified terminal-based process.
    /// </summary>
    /// <param name="process">The process to be terminated.</param>
    public void KillProcess(Process process)
    {
        process.Kill();
    }
}