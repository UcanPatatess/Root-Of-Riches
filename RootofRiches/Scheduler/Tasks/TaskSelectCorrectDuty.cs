using ECommons.Throttlers;
using RootofRiches.Scheduler.Handlers;

namespace RootofRiches.Scheduler.Tasks
{
    internal class TaskSelectCorrectDuty
    {
        private static int PcallValue = 0;
        private static bool Restart = false;
        internal static void Enqueue(uint ZoneID)
        {
            P.taskManager.Enqueue(() => PcallValue = NRaidDict[ZoneID].PcallID);
            P.taskManager.Enqueue(() => Run(ZoneID));
        }
        internal static bool Run(uint ZoneID)
        {
            if (Restart)
            {
                PcallValue = 0; // Reset to start if needed
                Restart = false;
            }
            if (CorrectDuty(ZoneID)) 
            {
                if (ZoneID == A4NMapID)
                {
                    C.A4NPcallValue = PcallValue;
                    NRaidDict[ZoneID].PcallID = PcallValue;
                }
                else if (ZoneID == O3NMapID)
                {
                    C.O3NPcallValue = PcallValue;
                    NRaidDict[ZoneID].PcallID = PcallValue;
                }
                return true;
            }
            if (IsAddonActive("JournalDetail"))
            {
                if (EzThrottler.Throttle("Throttling Pcall for Normal raid", 75))
                {
                    GenericHandlers.FireCallback("ContentsFinder", true, 3, PcallValue);
                    if (!CorrectDuty(ZoneID))
                        PcallValue += 1;

                    if (PcallValue > 127)
                    {
                        Restart = true;
                    }
                }
            }
            return false;
        }
    }
}
