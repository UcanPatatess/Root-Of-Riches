using RootofRiches.Scheduler.Handlers;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskChangeArmorySetting
    {
        internal static void Enqueue()
        {
            TaskPluginLog.Enqueue("Changing Armory Setting");
            P.taskManager.Enqueue(() => UpdateCurrentTask("Changing Armory Setting"));
            P.taskManager.Enqueue(GenericHandlers.OpenCharaSettings);
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, 10, 0, 20));
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, 18, 300, C.MaxArmory ? 1 : 0));
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, 0));
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, -1));
            P.taskManager.Enqueue(() => UpdateCurrentTask("idle"));
            TaskPluginLog.Enqueue("Armory Setting Updated");
        }
    }
}
