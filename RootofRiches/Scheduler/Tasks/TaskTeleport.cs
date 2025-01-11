using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTeleport
    {
        internal static unsafe void Enqueue(uint aetherytID, uint targetTerritoryId, uint secondZoneID = 0, bool useSecondID = false)
        {
            P.taskManager.Enqueue(() => TeleporttoAethery(aetherytID, targetTerritoryId, secondZoneID, useSecondID), "Teleporting to Destination", DConfig);
        }

        internal static unsafe bool? TeleporttoAethery(uint aetherytID, uint targetTerritoryId, uint secondZoneID = 0, bool useSecondID = false)
        {
            if (IsInZone(targetTerritoryId) && PlayerNotBusy())
                return true;
            else if (useSecondID && IsInZone(secondZoneID) && PlayerNotBusy())
                return true;

            if (!Svc.Condition[ConditionFlag.Casting] && PlayerNotBusy() && !IsBetweenAreas && !IsInZone(targetTerritoryId))
            {
                if (EzThrottler.Throttle("Teleport Throttle", 1100))
                {
                    Telepo.Instance()->Teleport(aetherytID, 0);
                    return false;
                }
            }
            return false;
        }
    }
}
