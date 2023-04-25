using ValheimServerScheduler.Process;
using ValheimServerScheduler.Process.Lifecycle;
using ValheimServerScheduler.Scheduler;

ProcessManager processManager = new(
    new ProcessInfo("valheim_server.exe", @"D:\Games\ValheimServer\server\StartServer.bat"),
    new VisibleProcessLifecycleManager());

TimeSpan estimatedStartupTime = TimeSpan.FromSeconds(30);
TimeSpan estimatedShutdownTime = TimeSpan.FromSeconds(20);
ServerScheduler scheduler = new(processManager, estimatedStartupTime, estimatedShutdownTime);

SchedulerRule weekDayStart = new SchedulerRule(SchedulerAction.Start, DateTime.Parse("4 PM").TimeOfDay);
SchedulerRule weekDayStop = new SchedulerRule(SchedulerAction.Stop, DateTime.Parse("11:59 PM").TimeOfDay);
SchedulerRule morningRestart = new SchedulerRule(SchedulerAction.Restart, DateTime.Parse("4 AM").TimeOfDay);
SchedulerRule afternoonRestart = new SchedulerRule(SchedulerAction.Restart, DateTime.Parse("4 PM").TimeOfDay);

scheduler.AddRule(DayOfWeek.Monday, weekDayStart)
    .AddRule(DayOfWeek.Monday, weekDayStop)
    ///////////////////////////////////////////////
    .AddRule(DayOfWeek.Tuesday, weekDayStart)
    .AddRule(DayOfWeek.Tuesday, weekDayStop)
    ///////////////////////////////////////////////
    .AddRule(DayOfWeek.Wednesday, weekDayStart)
    .AddRule(DayOfWeek.Wednesday, weekDayStop)
    ///////////////////////////////////////////////
    .AddRule(DayOfWeek.Thursday, weekDayStart)
    .AddRule(DayOfWeek.Thursday, weekDayStop)
    ///////////////////////////////////////////////
    .AddRule(DayOfWeek.Friday, weekDayStart)
    ///////////////////////////////////////////////
    .AddRule(DayOfWeek.Saturday, morningRestart)
    ///////////////////////////////////////////////
    .AddRule(DayOfWeek.Sunday, morningRestart)
    .AddRule(DayOfWeek.Sunday, afternoonRestart)
    .AddRule(DayOfWeek.Sunday, weekDayStop);

// Program.
processManager.KillAllInstances();
await scheduler.Run();