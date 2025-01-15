using Dalamud.Game.ClientState.Objects.Types;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskOpenChest
    {
        public static void Enqueue(ulong GameObjectID)
        {
            IGameObject? gameObject = null;
            P.taskManager.Enqueue(() => TryGetObjectByDataId(GameObjectID, out gameObject), "Getting Object by ObjectID");
            P.taskManager.Enqueue(PlayerNotBusy);
            TaskPluginLog.Enqueue("Interacting with Chest");
            P.taskManager.Enqueue(() => InteractWithObject(gameObject), "Interacting w/ Object");
        }
    }
}
