using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Drawing.Text;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTeleportInn
    {
        internal static unsafe void Enqueue()
        {
            P.taskManager.Enqueue(() => TeleporttoAethery(), "Teleporting to Inn", DConfig);
        }

        internal static unsafe bool? TeleporttoAethery()
        {
            uint MainAether = 0;
            uint MainCity = 0;
            uint MainCity2 = 0;
            bool UseSecondZoneID = false;

            if (C.InnOption == "Limsa")
            {
                MainAether = InnDict[LimsaInn].MainAether;
                MainCity = InnDict[LimsaInn].MainCity;
                MainCity2 = InnDict[LimsaInn].MainCity2;
                UseSecondZoneID = true;
            }
            else
            {
                MainAether = InnDict[C.InnDataID].MainAether;
                MainCity = InnDict[C.InnDataID].MainCity;
            }

            if (IsInZone(MainCity) && PlayerNotBusy())
                return true;
            else if (UseSecondZoneID && IsInZone(MainCity2))
                return true;

            if (!Svc.Condition[ConditionFlag.Casting] && PlayerNotBusy() && !IsBetweenAreas && !IsInZone(MainCity))
            {
                if (EzThrottler.Throttle("Teleport Throttle", 1100))
                {
                    Telepo.Instance()->Teleport(MainAether, 0);
                    return false;
                }
            }
            return false;
        }
    }
}
