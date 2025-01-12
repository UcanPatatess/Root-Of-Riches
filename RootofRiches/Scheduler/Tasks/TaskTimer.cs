using ECommons.Logging;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTimer
    {
        internal static void Enqueue(bool Calculate, uint ZoneID = 0)
        {
            P.taskManager.Enqueue(() => TimerManager(Calculate, ZoneID), "Calculating Time");
        }

        internal unsafe static bool TimerManager(bool Calculate, uint ZoneID)
        {
            if (Calculate)
            {
                P.stopwatch.Reset();
                P.stopwatch.Start();
            }
            else if (!Calculate)
            {
                P.stopwatch.Stop();
                TimeSpan elapsedTime = P.stopwatch.Elapsed;

                // Updates the timer in the config
                C.SessionStats.TotalRunTime += elapsedTime;
                C.Stats.TotalRunTime += elapsedTime;
                if (ZoneID == A4NMapID)
                {
                    C.SessionStats.TotalTimeA4N += elapsedTime;
                    C.Stats.TotalTimeA4N += elapsedTime;
                }
                else if (ZoneID == O3NMapID)
                {
                    C.SessionStats.TotalTimeO3N += elapsedTime;
                    C.Stats.TotalTimeO3N += elapsedTime;
                }

                // Checks to see if your fastest loop is faster than your last'
                if (elapsedTime.TotalSeconds > 1)
                {
                    if (ZoneID == A4NMapID)
                    {
                        if (elapsedTime < C.SessionStats.FastestA4NRun)
                        {
                            C.SessionStats.FastestA4NRun = elapsedTime;
                            PluginLog.Information($"Your new fastest time is: {elapsedTime.TotalSeconds:F2}");
                        }
                        if (elapsedTime < C.Stats.FastestA4NRun)
                        {
                            C.Stats.FastestA4NRun = elapsedTime;
                            PluginLog.Information($"Your new fastest time is: {elapsedTime.TotalSeconds:F2}");
                        }
                    }
                    else if (ZoneID == O3NMapID)
                    {
                        if (elapsedTime < C.SessionStats.FastestO3NRun)
                        {
                            C.SessionStats.FastestO3NRun = elapsedTime;
                            PluginLog.Information($"Your new fastest time is: {elapsedTime.TotalSeconds:F2}");
                        }
                        if (elapsedTime < C.Stats.FastestO3NRun)
                        {
                            C.Stats.FastestO3NRun = elapsedTime;
                            PluginLog.Information($"Your new fastest time is: {elapsedTime.TotalSeconds:F2}");
                        }
                    }
                }
                else
                {
                    PluginLog.Information($"You completed the run in: {elapsedTime.TotalSeconds:F2}");
                }
                PluginLog.Information($"Total Run time (Session): {C.SessionStats.TotalRunTime.TotalSeconds:F2}");
                PluginLog.Information($"Total Run time (Session): {C.Stats.TotalRunTime.TotalSeconds:F2}");
                C.Save();
            }
            return true;
        }
    }
}
