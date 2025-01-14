using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation;

namespace RootofRiches.Scheduler.Tasks
{
    internal class TaskContentWidnowConfirm
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(ContentsFinderConfirm);
            P.taskManager.Enqueue(() => IsInZone(A4NMapID) || IsInZone(O3NMapID));
            P.taskManager.Enqueue(PlayerNotBusy);
        }
        internal static unsafe bool? ContentsFinderConfirm()
        {
            if (TryGetAddonByName<AtkUnitBase>("ContentsFinderConfirm", out var addon) && IsAddonReady(addon))
            {
                if (EzThrottler.Throttle("Commencing the duty", 10))
                {
                    Callback.Fire(addon, true, 8);
                    return true;
                }
            }
            return false;
        }
    }
}
