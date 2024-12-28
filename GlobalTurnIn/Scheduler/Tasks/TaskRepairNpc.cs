using ECommons.DalamudServices;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskRepairNpc
    {
        internal static void Enqueue()
        {
            TaskTarget.Enqueue(InnDataID);
            TaskInteract.Enqueue(InnDataID);
            P.taskManager.Enqueue(RepairNpc);
            TaskGetOut.Enqueue();
        }
        private static ulong InnDataID = InnDict[C.InnDataID].RepairNPC ;
        internal unsafe static bool RepairNpc()
        {
            if (!NeedsRepair(C.RepairSlider))
            {
                return true;
            }
            else if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var addon) && IsAddonReady(addon)) 
            {
                Svc.Log.Debug("SelectYesno Callback");
                if (FrameThrottler.Throttle("SelectYesnoThrottle", 70))
                    Callback.Fire(addon, true, 0);
            }
            else if (TryGetAddonByName<AtkUnitBase>("Repair", out var addon4) && IsAddonReady(addon4))
            {
                    Svc.Log.Debug("Repair Callback");
                    if (FrameThrottler.Throttle("GlobalTurnInGenericThrottle", 70))
                    Callback.Fire(addon4, true, 0);
            }
            return false;
        }
    }
}
