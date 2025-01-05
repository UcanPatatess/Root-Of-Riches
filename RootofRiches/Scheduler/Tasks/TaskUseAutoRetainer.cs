using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskUseAutoRetainer
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            if (!P.autoRetainer.GetOptionRetainerSense())
                P.autoRetainer.SetOptionRetainerSense(true);
            P.taskManager.Enqueue(WaitAndClose);
            TaskGetOut.Enqueue();
            P.taskManager.Enqueue(() => P.autoRetainer.SetOptionRetainerSense(false));
        }
        private static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);
        private static void WaitAndClose() => P.taskManager.InsertMulti([ new(() => P.autoRetainer.IsBusy()), new(() => !P.autoRetainer.IsBusy(), DConfig)]);
    }
}
