using ECommons.DalamudServices;
using ECommons.Throttlers;
using RootofRiches.Scheduler.Handlers;

namespace RootofRiches.Scheduler.Tasks
{
    internal unsafe static class TaskUseAethernet
    {
        internal static void Enqueue(bool forceuse = false)
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            if (forceuse && Svc.ClientState.TerritoryType != InnDict[C.InnDataID].MainCity2)// used for gc turn in limsa
            {
                TaskMoveTo.Enqueue(new(-84.03f, 20.77f, 0.02f), "Moving To aetheryte ", 7);
                TaskTarget.Enqueue(InnDict[LimsaInn].MainAether);
                TaskInteract.Enqueue(InnDict[LimsaInn].MainAether);
                P.taskManager.Enqueue(() => GenericHandlers.FireCallback("SelectString", true, 0));
                P.taskManager.Enqueue(() => GenericHandlers.FireCallback("TelepotTown", true, 11, 1));
                P.taskManager.Enqueue(() => GenericHandlers.FireCallback("TelepotTown", true, 11, 1));
                P.taskManager.Enqueue(() => !PlayerNotBusy());
                P.taskManager.Enqueue(PlayerNotBusy);
            }
            else if (!UseAether())
            {
                if (C.InnOption == "Limsa")
                {
                    TaskMoveTo.Enqueue(new(-84.03f, 20.77f, 0.02f), "Moving To aetheryte ", 7);
                    TaskTarget.Enqueue(InnDict[C.InnDataID].MainAether);
                    TaskInteract.Enqueue(InnDict[C.InnDataID].MainAether);
                    P.taskManager.Enqueue(() => GenericHandlers.FireCallback("SelectString", true, 0));
                    P.taskManager.Enqueue(() => GenericHandlers.FireCallback("TelepotTown", true, 11, 1));
                    P.taskManager.Enqueue(() => GenericHandlers.FireCallback("TelepotTown", true, 11, 1));
                    P.taskManager.Enqueue(() => !PlayerNotBusy());
                    P.taskManager.Enqueue(PlayerNotBusy);
                }
                else if (C.InnOption == "Ul'Dah")
                {
                    TaskTarget.Enqueue(InnDict[C.InnDataID].MainAether);
                    TaskInteract.Enqueue(InnDict[C.InnDataID].MainAether);
                    P.taskManager.Enqueue(() => GenericHandlers.FireCallback("SelectString", true, 0));
                    P.taskManager.Enqueue(() => GenericHandlers.FireCallback("TelepotTown", true, 11, 1));
                    P.taskManager.Enqueue(() => GenericHandlers.FireCallback("TelepotTown", true, 11, 1));
                    P.taskManager.Enqueue(() => !PlayerNotBusy());
                    P.taskManager.Enqueue(PlayerNotBusy);
                }
            }
            P.taskManager.Enqueue(UseAether);
            P.taskManager.Enqueue(PlayerNotBusy);
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
            return false;
        }
    }
}
