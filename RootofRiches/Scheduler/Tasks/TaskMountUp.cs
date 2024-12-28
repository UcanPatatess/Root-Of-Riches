using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;


namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskMountUp
    {
        public static void Enqueue()
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("Mounting Up"));
            P.taskManager.Enqueue(MountUp);
            P.taskManager.Enqueue(() => UpdateCurrentTask(""));
        }
        internal unsafe static bool? MountUp()
        {
            if (Svc.Condition[ConditionFlag.Mounted] && PlayerNotBusy()) return true;

            if (CurrentTerritory() == 478 || CurrentTerritory() == 635)
            {
                if (!Svc.Condition[ConditionFlag.Casting] && !Svc.Condition[ConditionFlag.Unknown57] && PlayerNotBusy())
                {
                    ActionManager.Instance()->UseAction(ActionType.GeneralAction, 24);
                }
            }
            return false;
        }
    }
}
