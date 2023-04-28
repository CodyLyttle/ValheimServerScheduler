using LaunchScheduler.Process;
using LaunchScheduler.Process.Lifecycle;
using LaunchScheduler.Scheduler;
using LaunchScheduler.Scheduler.Rules;

ProcessManager processManager = new(
    new ProcessInfo("valheim_server.exe", @"D:\Games\ValheimServer\server\StartServer.bat"),
    new TerminalLifecycleManager());

var scheduler = new Scheduler(
    new TestRuleProvider(), 
    processManager);

await processManager.StopAllInstances();
await scheduler.RunSchedulerLoop();