using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskUseAutoRetainer
    {
        internal static void Enqueue()
        {
            IGameObject? gameObject = null;
            TryGetObjectByDataId(SummoningBell, out gameObject);
            TaskPluginLog.Enqueue("Using Auto Retainer Task");
            P.taskManager.Enqueue(PlayerNotBusy);
            if (!P.autoRetainer.GetOptionRetainerSense())
                P.taskManager.Enqueue(() => P.autoRetainer.SetOptionRetainerSense(true));
            TaskTarget.Enqueue(SummoningBell);
            if (gameObject != null && GetDistanceToVectorPoint(Svc.Targets.Target.Position) > 3)
                TaskMoveTo.Enqueue(Svc.Targets.Target.Position, "Retainer Bell", 3);
            P.taskManager.Enqueue(RetainerOpened);
            P.taskManager.Enqueue(() => !ARRetainersWaitingToBeProcessed());
            P.taskManager.Enqueue(() => P.autoRetainer.SetOptionRetainerSense(false));
            P.taskManager.Enqueue(() => P.autoRetainer.IsBusy());
            P.taskManager.Enqueue(() => !P.autoRetainer.IsBusy());
            TaskGetOut.Enqueue();
        }
        internal unsafe static bool RetainerOpened()
        {
            if (TryGetAddonByName<AtkUnitBase>("RetainerList", out var addon) && IsAddonReady(addon))
                return true;
            return false;
        }
    }
}
