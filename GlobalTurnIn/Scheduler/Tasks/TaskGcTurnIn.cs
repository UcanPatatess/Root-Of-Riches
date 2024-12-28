using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal unsafe static class TaskGcTurnIn
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("Moving to GC"));
            P.taskManager.Enqueue(GoToGC, configuration: LSConfig);
            P.taskManager.EnqueueDelay(1000);
            P.taskManager.Enqueue(Deliveroo, configuration: DConfig);
            P.taskManager.Enqueue(() => UpdateCurrentTask("Turning in at GC"));
            P.taskManager.EnqueueDelay(1000);
            P.taskManager.Enqueue(() => UpdateCurrentTask(""));
        }
        private static TaskManagerConfiguration LSConfig => new(timeLimitMS: 2 * 60 * 1000);
        private static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);
        private static void GoToGC() => P.taskManager.InsertMulti([new(PlayerNotBusy),new(() => P.lifestream.ExecuteCommand("gc")), new(() => P.lifestream.IsBusy()), new(() => !P.lifestream.IsBusy(), LSConfig)]);
        private static void Deliveroo() => P.taskManager.InsertMulti([new(() => Svc.Commands.ProcessCommand("/deliveroo enable")), new(() => P.deliveroo.IsTurnInRunning()), new(() => !P.deliveroo.IsTurnInRunning(), DConfig)]);
    }
}
