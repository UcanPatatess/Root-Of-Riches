using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskTimer
    {
        internal static void Enqueue(bool Calculate)
        {
            P.taskManager.Enqueue(() => TimerManager(Calculate), "Calculating Time");
        }

        internal unsafe static bool TimerManager(bool Calculate)
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

                // Checks to see if your fastest loop is faster than your last'
                if (elapsedTime.TotalSeconds > 1)
                {
                    if (elapsedTime < C.SessionStats.FastestRun)
                    {
                        C.SessionStats.FastestRun = elapsedTime;
                        PluginLog.Information($"Your new fastest time is: {elapsedTime.TotalSeconds:F2}");
                    }
                    if (elapsedTime < C.Stats.FastestRun)
                    {
                        C.Stats.FastestRun = elapsedTime;
                        PluginLog.Information($"Your new fastest time is: {elapsedTime.TotalSeconds:F2}");
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
