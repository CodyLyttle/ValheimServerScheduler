using System.Diagnostics;

namespace ValheimServerScheduler.Process.Lifecycle;

internal interface IProcessLifecycleManager
{
    ProcessStartInfo CreateStartInfo(ProcessInfo info);
    
    void KillProcess(System.Diagnostics.Process process);
}