using FFXIVClientStructs.FFXIV.Component.GUI;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskUseAutoRetainer
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            if (!P.autoRetainer.GetOptionRetainerSense())
                P.taskManager.Enqueue(() => P.autoRetainer.SetOptionRetainerSense(true));
            P.taskManager.Enqueue(RetainerOpened);
            P.taskManager.Enqueue(() => !ARAvailableRetainersCurrentCharacter());
            P.taskManager.Enqueue(() => P.autoRetainer.SetOptionRetainerSense(false));
            P.taskManager.Enqueue(() => P.autoRetainer.IsBusy());
            P.taskManager.Enqueue(() => !P.autoRetainer.IsBusy());
            TaskGetOut.Enqueue();
            P.taskManager.Enqueue(PlayerNotBusy);
        }
        internal unsafe static bool RetainerOpened()
        {
            if (TryGetAddonByName<AtkUnitBase>("RetainerList", out var addon) && IsAddonReady(addon))
                return true;
            return false;
        }
    }
}
