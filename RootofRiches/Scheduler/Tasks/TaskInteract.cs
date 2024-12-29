using Dalamud.Game.ClientState.Objects.Types;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskInteract
    {
        public static void Enqueue(ulong dataID)
        {
            IGameObject? gameObject = null;
            P.taskManager.EnqueueDelay(1000);
            P.taskManager.Enqueue(() => TryGetObjectByDataId(dataID, out gameObject));
            P.taskManager.Enqueue(() => InteractWithObject(gameObject));
        }
    }
}
