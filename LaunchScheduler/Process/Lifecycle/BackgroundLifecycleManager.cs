using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LaunchScheduler.Process.Lifecycle;

using Process = System.Diagnostics.Process;

/// <summary>
/// Handles the lifecycle of background processes.
/// </summary>
public sealed class BackgroundLifecycleManager : IProcessLifecycleManager
{
    /// <summary>
    /// Creates ProcessStartInfo for background processes based on the provided ProcessInfo.
    /// </summary>
    /// <param name="info">Details about the process to be started.</param>
    /// <returns>ProcessStartInfo configured for background processes.</returns>
    public ProcessStartInfo CreateStartInfo(ProcessInfo info)
    {
        return new ProcessStartInfo
        {
            FileName = info.FilePath,
            Arguments = string.Join(" ", info.Args.Select(arg => arg.ToString())),
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };
    }
    
    /// <summary>
    /// Attempts to gracefully terminate the specified background process by sending a CTRL+C event
    /// </summary>
    /// <param name="process">The process to be terminated.</param>
    public void AttemptGracefulTermination(Process process)
    {
        ExitGracefully(process.Id);
    }
    
    /// <summary>
    /// Terminates the specified background process.
    /// </summary>
    /// <param name="process">The process to be terminated.</param>
    public void KillProcess(Process process)
    {
        process.Kill();
    }

    // BUG: App closes abruptly after the target process exits.
    // Is the cancel signal affecting the app, or is it an uncaught native exception?
    private static void ExitGracefully(int processId)
    {
        if (AttachConsole((uint) processId))
        {
            try
            {
                GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);
            }
            finally
            {
                FreeConsole();
            }
        }
        else
        {
            Console.WriteLine("Failed to attach to the target process's console.");
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

    private enum CtrlTypes : uint
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT = 1,
        CTRL_CLOSE_EVENT = 2,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT = 6
    }
}