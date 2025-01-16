using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskUseRetainer
    {
        internal static void Enqueue()
        {
            IGameObject? gameObject = null;
            TryGetObjectByDataId(SummoningBell, out gameObject);
            TaskPluginLog.Enqueue("Using Auto Retainer Task");
            P.taskManager.Enqueue(PlayerNotBusy);
            TaskMoveTo.Enqueue(gameObject.Position, "Retainer Bell", 4);
            TaskTarget.Enqueue(SummoningBell);
            TaskInteract.Enqueue(SummoningBell);
            P.taskManager.Enqueue(RetainerOpened);
            P.taskManager.Enqueue(() => !ARRetainersWaitingToBeProcessed());
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
