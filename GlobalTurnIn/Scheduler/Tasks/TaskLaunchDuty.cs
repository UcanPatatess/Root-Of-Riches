using ECommons.Automation;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal class TaskLaunchDuty
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(Run);
        }
        internal static unsafe bool? Run()
        {
            if (TryGetAddonByName<AtkUnitBase>("ContentsFinder", out var addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("Commencing the duty", 70))
                {
                    Callback.Fire(addon, true, 12, 0);
                    return true;
                }
            }
            return false;
        }
    }
}
