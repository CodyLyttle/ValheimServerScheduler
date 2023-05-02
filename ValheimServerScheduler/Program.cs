using LaunchScheduler;
using LaunchScheduler.Process.Lifecycle;
using LaunchScheduler.Rules;
using Microsoft.Extensions.Logging.Abstractions;

Scheduler scheduler = new SchedulerBuilder()
    .SetProcessInfo("valheim_server.exe", @"D:\Games\ValheimServer\server\StartServer.bat")
    .SetLifecycleManager(new TerminalLifecycleManager())
    .SetRuleProvider(new TestRuleProvider())
    .SetStartupGracePeriod(TimeSpan.FromSeconds(60))
    .SetShutdownGracePeriod(TimeSpan.FromSeconds(30))
    .SetLogger(NullLogger.Instance)
    .Build();

await scheduler.ProcessManager.StopAllInstances();
await scheduler.RunSchedulerLoop();
