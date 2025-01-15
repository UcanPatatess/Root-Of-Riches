using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskDutyFinder
    {
        internal static void Enqueue(uint DutyID)
        {
            P.taskManager.Enqueue(PlayerNotBusy);
            TaskPluginLog.Enqueue("Opening Duty Finder/Opening to specific duty");
            P.taskManager.Enqueue(() => OpenDutyFinder(DutyID));
        }

        public static bool DutyFinderOpen;
        internal unsafe static bool? OpenDutyFinder(uint DutyID)
        {
            if (TryGetAddonByName<AtkUnitBase>("ContentsFinder", out var addon) && IsAddonReady(addon))
            {
                AgentContentsFinder.Instance()->OpenRegularDuty(DutyID);//relocated
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
