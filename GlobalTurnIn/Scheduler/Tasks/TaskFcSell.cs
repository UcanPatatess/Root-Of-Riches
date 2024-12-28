using ECommons.Automation.NeoTaskManager;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskFcSell
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("Going to Fc"));
            P.taskManager.Enqueue(GoHome,configuration:LSConfig);
            P.taskManager.EnqueueDelay(1000);
            P.taskManager.Enqueue(() => UpdateCurrentTask(""));
        }
        private static void GoHome() => P.taskManager.InsertMulti([new(P.lifestream.TeleportToFC), new(() => P.lifestream.IsBusy()), new(() => !P.lifestream.IsBusy(), LSConfig)]);
        private static TaskManagerConfiguration LSConfig => new(timeLimitMS: 2 * 60 * 1000);
    } 
}
