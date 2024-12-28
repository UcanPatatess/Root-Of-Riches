using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using GlobalTurnIn.IPC;
using System.Numerics;


namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskMoveTo
    {
        private static float Distance(this Vector3 v, Vector3 v2)
        {
            return new Vector2(v.X - v2.X, v.Z - v2.Z).Length();
        }
        private static unsafe bool IsMoving()
        {
            return AgentMap.Instance()->IsPlayerMoving == 1;
        }
        internal unsafe static void Enqueue(Vector3 targetPosition, string destination, float toleranceDistance = 3f)
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("Moving to location"), "Task Update");
            P.taskManager.Enqueue(() => MoveTo(targetPosition, toleranceDistance),destination);
            P.taskManager.Enqueue(() => UpdateCurrentTask(""), "Task Update");
        }
        internal unsafe static bool? MoveTo(Vector3 targetPosition, float toleranceDistance = 3f)
        {
            if (targetPosition.Distance(Player.GameObject->Position) <= toleranceDistance)
            {
                P.navmesh.Stop();
                return true;
            }
            if (!P.navmesh.IsReady()) { UpdateCurrentTask("Waiting Navmesh"); return false; }
            if (P.navmesh.PathfindInProgress() || P.navmesh.IsRunning() || IsMoving()) return false;

            P.navmesh.PathfindAndMoveTo(targetPosition, false);
            P.navmesh.SetAlignCamera(true);
            return false;
        }
    }
}
