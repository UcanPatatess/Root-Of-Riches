using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskUseAethernet
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.Enqueue(UseAether);
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.EnqueueDelay(1000);

        }
        private static bool UseAether()
        {
            if (C.InnOption == "Limsa")
            {
                if (Svc.ClientState.TerritoryType == InnDict[C.InnDataID].MainCity2)
                    return true;
            }
            if (C.InnOption == "Ul'Dah")
            {
                if (GetDistanceToPoint(64.23f, 4.53f, -115.31f) < 6)
                {
                    return true;
                }
            }
            if (C.InnOption == "Gridania")
            {
                return true;
            }
            if (!P.lifestream.IsBusy() && PlayerNotBusy())
            {
                if (FrameThrottler.Throttle("SelectYesnoThrottle", 300))
                    P.lifestream.ExecuteCommand($"{InnDict[C.InnDataID].AethernetCrystal}");
            }
            return false;
        }
    }
}
