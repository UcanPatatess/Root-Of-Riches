using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskGetOutInn
    {
        internal static void Enqueue()
        {
            TaskPluginLog.Enqueue("Leaving the Inn");
            TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnDoorPos, "Moving closer to door");
            TaskTarget.Enqueue(InnDict[C.InnDataID].InnDoor);
            TaskInteract.Enqueue(InnDict[C.InnDataID].InnDoor);
            P.taskManager.Enqueue(GetOutInn);
            P.taskManager.Enqueue(PlayerNotBusy);
        }
        internal unsafe static bool GetOutInn()
        {
            if (Svc.ClientState.TerritoryType != C.InnDataID)
            {
                return true;
            }
            else if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var addon) && IsAddonReady(addon))
            {
                Svc.Log.Debug("SelectYesno Callback");
                if (FrameThrottler.Throttle("SelectYesnoThrottle", 70))
                    Callback.Fire(addon, true, 0);
            }
            return false;
        }
    }
}
