using ECommons.EzSharedDataManager;
using ECommons.Logging;

namespace GlobalTurnIn.Scheduler.Handlers
{
    internal static class YesAlreadyManager
    {
        private static bool WasChanged = false;
        internal static void Tick()
        {
            if (WasChanged)
            {
                if (!P.taskManager.IsBusy)
                {
                    WasChanged = false;
                    Unlock();
                    PluginLog.Debug($"YesAlready unlocked");
                }
            }
            else
            {
                if (P.taskManager.IsBusy)
                {
                    WasChanged = true;
                    Lock();
                    PluginLog.Debug($"YesAlready locked");
                }
            }
        }
        internal static void Lock()
        {
            if (EzSharedData.TryGet<HashSet<string>>("YesAlready.StopRequests", out var data))
            {
                data.Add(P.Name);
            }
        }

        internal static void Unlock()
        {
            if (EzSharedData.TryGet<HashSet<string>>("YesAlready.StopRequests", out var data))
            {
                data.Remove(P.Name);
            }
        }
    }
}
