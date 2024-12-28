using GlobalTurnIn.Scheduler.Handlers;


namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskChangeArmorySetting
    {
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("Changing Armory Setting"));
            P.taskManager.Enqueue(GenericHandlers.OpenCharaSettings);
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, 10, 0, 20));
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, 18, 300, C.MaxArmory ? 1 : 0));
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, 0));
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("ConfigCharacter", true, -1));
            P.taskManager.Enqueue(() => UpdateCurrentTask(""));
        }
    }
}
