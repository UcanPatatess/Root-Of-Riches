using ECommons.EzSharedDataManager;
using ECommons.Logging;

namespace GlobalTurnIn.Scheduler.Handlers
{
    internal static class TextAdvanceManager
    {
        private static bool WasChanged = false;
        internal static void Tick()
        {
            if (WasChanged)
            {
                if (!P.taskManager.IsBusy)
                {
                    WasChanged = false;
                    UnlockTA();
                    PluginLog.Debug($"TextAdvance unlocked");
                }
            }
            else
            {
                if (P.taskManager.IsBusy)
                {
                    WasChanged = true;
                    LockTA();
                    PluginLog.Debug($"TextAdvance locked");
                }
            }
        }
        internal static void LockTA()
        {
            if (EzSharedData.TryGet<HashSet<string>>("TextAdvance.StopRequests", out var data))
            {
                data.Add(P.Name);
            }
        }

        internal static void UnlockTA()
        {
            if (EzSharedData.TryGet<HashSet<string>>("TextAdvance.StopRequests", out var data))
            {
                data.Remove(P.Name);
            }
        }
    }
}
