using ECommons.Logging;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal class TaskContentWidnowConfirm
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(ContentsFinderConfirm);
            P.taskManager.Enqueue(() => IsInZone(A4NMapID));
            P.taskManager.Enqueue(PlayerNotBusy);
        }
        internal static unsafe bool? ContentsFinderConfirm()
        {
            if (TryGetAddonByName<AtkUnitBase>("ContentsFinderConfirm", out var addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("Commencing the duty", 70))
                {
                    Callback.Fire(addon, true, 8);
                    return true;
                }
            }
            return false;
        }
    }
}
