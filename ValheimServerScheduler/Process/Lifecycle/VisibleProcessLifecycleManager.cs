using System.Diagnostics;

namespace ValheimServerScheduler.Process.Lifecycle;

using Process = System.Diagnostics.Process;

// Manages the lifecycle of a process with a GUI or terminal window.
internal class VisibleProcessLifecycleManager : IProcessLifecycleManager
{
    public ProcessStartInfo CreateStartInfo(ProcessInfo info)
    {
        return new ProcessStartInfo
        {
            FileName = info.FilePath,
            Arguments = string.Join(" ", info.Args.Select(arg => arg.ToString())),
            UseShellExecute = false
        };
    }

    public void KillProcess(Process process)
    {
        process.CloseMainWindow();
        process.Kill();
    }
}