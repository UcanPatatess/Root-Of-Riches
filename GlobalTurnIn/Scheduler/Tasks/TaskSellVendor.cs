using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskSellVendor
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("Selling to vendor"));
            P.taskManager.Enqueue(SellVendor);
            TaskGetOut.Enqueue();
            P.taskManager.EnqueueDelay(1000);
            P.taskManager.Enqueue(() => UpdateCurrentTask(""));
        }
        private static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);
        private static void SellVendor() => P.taskManager.InsertMulti([new(PlayerNotBusy),new(() => Svc.Commands.ProcessCommand("/ays itemsell")), new(() => P.autoRetainer.IsBusy()), new(() => !P.autoRetainer.IsBusy(), DConfig)]);
    }
}
