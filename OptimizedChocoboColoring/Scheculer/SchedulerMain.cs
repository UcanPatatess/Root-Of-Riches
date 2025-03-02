using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizedChocoboColoring.Scheculer
{
    internal static unsafe class SchedulerMain
    {
        internal static bool AreWeTicking;
        internal static bool DoWeTick
        {
            get => AreWeTicking;
            private set => AreWeTicking = value;
        }
        internal static bool EnablePlugin()
        { return true; }
        internal static bool DisablePlugin()
        { return true; }
        internal static void Tick()
        {
            if (DoWeTick)
            {
                if (!P.taskManager.IsBusy)
                {

                }
            }
        }
    }
}
