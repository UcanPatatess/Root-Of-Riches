using Dalamud.Game.ClientState.Objects.Types;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskTarget
    {
        public static void Enqueue(ulong objectID)
        {
            IGameObject? gameObject = null;
            P.taskManager.Enqueue(() => TryGetObjectByDataId(objectID, out gameObject), "Getting Object");
            P.taskManager.Enqueue(() => TargetByID(gameObject), "Targeting Object");
        }
    }
}
