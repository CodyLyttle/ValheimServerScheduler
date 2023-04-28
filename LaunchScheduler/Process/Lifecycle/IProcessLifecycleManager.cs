using System.Diagnostics;

namespace LaunchScheduler.Process.Lifecycle;

/// <summary>
/// Defines a contract for managing the lifecycle of a process.
/// </summary>
public interface IProcessLifecycleManager
{
    /// <summary>
    /// Creates a ProcessStartInfo object from the provided ProcessInfo.
    /// </summary>
    /// <param name="info">Details about the process to be started.</param>
    /// <returns>The configured ProcessStartInfo.</returns>
    ProcessStartInfo CreateStartInfo(ProcessInfo info);

    /// <summary>
    /// Attempts to gracefully terminate a process by sending an appropriate signal or event.
    /// </summary>
    /// <param name="process">The process to be gracefully terminated.</param>
    void AttemptGracefulTermination(System.Diagnostics.Process process);
    
    /// <summary>
    /// Terminates the specified process.
    /// </summary>
    /// <param name="process">The process to be terminated.</param>
    void KillProcess(System.Diagnostics.Process process);
}