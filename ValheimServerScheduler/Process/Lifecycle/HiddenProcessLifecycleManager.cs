using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ValheimServerScheduler.Process.Lifecycle;

// Manages the lifecycle of a process with no GUI.
internal class HiddenProcessLifecycleManager : IProcessLifecycleManager
{
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

    public void KillProcess(System.Diagnostics.Process process)
    {
        ExitGracefully(process.Id);
        process.Kill();
    }

    // BUG: App unexpectedly closes shortly after the target process exits.
    // Is our application responding to the cancel signal sent to the target process?
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