using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.Throttlers;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskTeleportInn
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.Enqueue(TeleportToMainCity);
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.EnqueueDelay(1000);
        }
        private static bool TeleportToMainCity() 
        {
            if (Svc.ClientState.TerritoryType == InnDict[C.InnDataID].MainCity)
                return true;
            else if (C.InnOption == "Limsa")
                if (Svc.ClientState.TerritoryType == InnDict[C.InnDataID].MainCity2)
                    return true;

            if (!P.lifestream.IsBusy() && PlayerNotBusy())
            {
                if (FrameThrottler.Throttle("SelectYesnoThrottle", 300))
                    P.lifestream.ExecuteCommand($"tp {C.InnOption}");
            }
            return false;
        }

    }
}
