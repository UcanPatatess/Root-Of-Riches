using ECommons.Throttlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class Task_MoveTo
    {
        public static void Enqueue(List<Vector3> List)
        {
            P.taskManager.Enqueue(() => StartRoute(List), "Starting the route");
            P.taskManager.Enqueue(() => WaitForRoute(), "Waiting for navmesh to finish", DConfig);
        }

        internal static unsafe bool? StartRoute(List<Vector3> List)
        {
            if (P.navmesh.IsRunning())
            {
                return true;
            }
            else
            {
                if (EzThrottler.Throttle("Starting route"))
                {
                    P.navmesh.MoveTo(new List<Vector3>(List), false);
                }
            }
            return false;
        }

        internal static unsafe bool? WaitForRoute()
        {
            if (!P.navmesh.IsRunning())
            {
                return true;
            }
            return false;
        }
    }
}
