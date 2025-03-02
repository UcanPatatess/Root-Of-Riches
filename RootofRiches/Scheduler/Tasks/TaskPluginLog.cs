using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskPluginLog
    {
        internal static void Enqueue(string message)
        {
            P.taskManager.Enqueue(() => PLogInfo(message));
        }
    }
}
