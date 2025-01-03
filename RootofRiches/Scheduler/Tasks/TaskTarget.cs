using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTarget
    {
        public static void Enqueue(ulong objectID)
        {
            Svc.Log.Debug($"Targeting {objectID}");
            IGameObject? gameObject = null;
            P.taskManager.Enqueue(() => TryGetObjectByDataId(objectID, out gameObject), "Getting Object");
            P.taskManager.Enqueue(() => TargetByID(gameObject), "Targeting Object");
        }
    }
}
