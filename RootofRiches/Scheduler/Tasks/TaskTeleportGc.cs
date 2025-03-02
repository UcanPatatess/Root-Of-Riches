using GrandCompany = ECommons.ExcelServices.GrandCompany;
using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ECommons.GameHelpers;


namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTeleportGc
    {
        internal static unsafe void Enqueue()
        {
            TaskPluginLog.Enqueue("Teleporting to the GC");
            P.taskManager.Enqueue(TeleporttoAethery, "Teleporting to GC", DConfig);
        }
        internal static unsafe bool? TeleporttoAethery()
        {
            uint MainAether = 0;
            uint MainCity = 0;
            uint MainCity2 = 0;
            bool UseSecondZoneID = false;

            if (Player.GrandCompany == GrandCompany.Maelstrom)
            {
                MainAether = InnDict[LimsaInn].MainAether;
                MainCity = InnDict[LimsaInn].MainCity;
                MainCity2 = InnDict[LimsaInn].MainCity2;
                UseSecondZoneID = true;
            }
            else if (Player.GrandCompany == GrandCompany.ImmortalFlames)
            {
                MainAether = InnDict[UlDahInn].MainAether;
                MainCity = InnDict[UlDahInn].MainCity;
            }
            else if (Player.GrandCompany == GrandCompany.TwinAdder)
            {
                MainAether = InnDict[GridaniaInn].MainAether;
                MainCity = InnDict[GridaniaInn].MainCity;
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
