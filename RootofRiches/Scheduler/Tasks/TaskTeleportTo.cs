using ECommons.Automation.NeoTaskManager;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTeleportTo
    {
        internal static string WhereToTeleportString(uint ZoneID)
        {
            string where = string.Empty;
            if (ZoneID == Idyllshire)
                where = "idyllshire";
            if (ZoneID == Rhalgr)
                where = "Rhalgr";
            return where;
        }
        internal static void Enqueue(uint ZoneID)
        {
            if (IsInZone(ZoneID)) { return; }
            else
            {
                P.taskManager.Enqueue(() => UpdateCurrentTask("Teleporting"), "Teleporting");
                P.taskManager.Enqueue(() => P.lifestream.ExecuteCommand("tp " + WhereToTeleportString(ZoneID)));
                P.taskManager.Enqueue(() => !IsInZone(ZoneID));
                P.taskManager.Enqueue(() => IsInZone(ZoneID), LSConfig);
                P.taskManager.EnqueueDelay(1000);
                P.taskManager.Enqueue(() => UpdateCurrentTask("idle"), "Updating Task");
            } 
        }
        private static TaskManagerConfiguration LSConfig => new(timeLimitMS: 2 * 60 * 1000);
    }
}
