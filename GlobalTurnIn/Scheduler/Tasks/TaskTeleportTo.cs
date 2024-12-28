using ECommons.Automation.NeoTaskManager;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskTeleportTo
    {
        internal static int WhereToTeleportInt()
        {
            var where = 478;// idyllshire
            if (DeltascapeTurnInCount > 0)
            {
                where = 635;// Rhalgr
                return where;
            }
            return where;
        }
        internal static string WhereToTeleportString()
        {
            var where = "idyllshire";
            if (DeltascapeTurnInCount > 0)
            {
                where = "Rhalgr";
                return where;
            }
            return where;
        }
        internal static void Enqueue()
        {
            if (IsInZone(WhereToTeleportInt())) { return; }
            else
            {
                P.taskManager.Enqueue(() => UpdateCurrentTask("Teleporting"), "Teleporting");
                P.taskManager.Enqueue(Teleport);
                P.taskManager.EnqueueDelay(1000);
                P.taskManager.Enqueue(() => UpdateCurrentTask(""), "Updating Task");
            } 
        }
        private static TaskManagerConfiguration LSConfig => new(timeLimitMS: 2 * 60 * 1000);
        private static void Teleport() => P.taskManager.InsertMulti([new(() => P.lifestream.ExecuteCommand("tp " + WhereToTeleportString())), new(() => !IsInZone(WhereToTeleportInt())), new(() => IsInZone(WhereToTeleportInt()), LSConfig)]);
    }
}
