using ECommons.Throttlers;
using GlobalTurnIn.Scheduler.Handlers;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal class TaskSelectCorrectDuty
    {
        private static int PcallValue = C.DutyFinderCallValue;
        private static bool Restart = false;
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(Run);
        }
        internal static bool Run()
        {
            if (Restart)
            {
                PcallValue = 0; // Reset to start if needed
                Restart = false;
            }
            if (CorrectDuty()) 
            {
                C.DutyFinderCallValue = PcallValue;
                return true;
            }
            if (IsAddonActive("JournalDetail"))
            {
                if (EzThrottler.Throttle("Throttling Pcall for Normal raid", 75))
                {
                    GenericHandlers.FireCallback("ContentsFinder", true, 3, PcallValue);
                    if (!CorrectDuty())
                        PcallValue += 1;

                    if (PcallValue > 102)
                    {
                        Restart = true;
                    }
                }
            }
            return false;
        }
    }
}
