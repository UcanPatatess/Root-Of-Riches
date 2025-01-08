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

        internal static void TeleporttoInn()
        {
            if (C.InnOption == "Limsa")
            {
                uint Aether = InnDict[LimsaInn].MainAether;
                uint MainCity = InnDict[LimsaInn].MainCity;
                uint MainCity2 = InnDict[LimsaInn].MainCity2;
                TaskTeleport.Enqueue(Aether, MainCity, MainCity2, true);
            }
            else
            {
                uint Aether = InnDict[C.InnDataID].MainAether;
                uint MainCity = InnDict[C.InnDataID].MainCity;
                TaskTeleport.Enqueue(Aether, MainCity);
            }
        }

        private static bool TeleportToMainCity() 
        {
            uint MainCity = InnDict[C.InnDataID].MainCity;
            uint MainCity2 = InnDict[C.InnDataID].MainCity2;
            uint MainAetheryte = InnDict[C.InnDataID].MainAether;

            if (Svc.ClientState.TerritoryType == MainCity)
                return true;
            else if (C.InnOption == "Limsa")
                if (Svc.ClientState.TerritoryType == MainCity2)
                    return true;

            if (!P.lifestream.IsBusy() && PlayerNotBusy())
            {
                if (FrameThrottler.Throttle("SelectYesnoThrottle", 300))
                    P.lifestream.ExecuteCommand($"tp {C.InnOption}");
            }
            return false;
        }

    }
}
