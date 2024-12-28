using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskDutyFinder
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.Enqueue(OpenDutyFinder);
        }

        public static bool DutyFinderOpen;
        internal unsafe static bool? OpenDutyFinder()
        {
            if (TryGetAddonByName<AtkUnitBase>("ContentsFinder", out var addon) && IsAddonReady(addon))
            {
                AgentContentsFinder.Instance()->OpenRegularDuty(115);//relocated
                DutyFinderOpen = true;
                return true;
            }

            if (EzThrottler.Throttle("Open Duty Finder", 1000))
            { // Throttle to prevent spamming
                AgentContentsFinder.Instance()->AgentInterface.Show(); // Opens the duty finder
                ContentsFinder.Instance()->IsUnrestrictedParty = true; // Sets the DF to unsync
            }
            return false;
        }
    }
}
