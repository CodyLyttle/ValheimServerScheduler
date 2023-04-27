using ValheimServerScheduler.Process;
using ValheimServerScheduler.Process.Lifecycle;
using ValheimServerScheduler.Scheduler;
using ValheimServerScheduler.Scheduler.Rules;

ProcessManager processManager = new(
    new ProcessInfo("valheim_server.exe", @"D:\Games\ValheimServer\server\StartServer.bat"),
    new TerminalLifecycleManager());

ServerScheduler scheduler = new ServerScheduler(
    new TestRuleProvider(), 
    processManager);

await processManager.StopAllInstances();
await scheduler.RunSchedulerLoop();