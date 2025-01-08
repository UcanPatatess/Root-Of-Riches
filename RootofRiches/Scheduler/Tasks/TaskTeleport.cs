using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using RootofRiches.Scheduler.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskTeleport
    {
        internal static unsafe void Enqueue(uint aetherytID, int targetTerritoryId)
        {
            P.taskManager.Enqueue(() => TeleporttoAethery(aetherytID, targetTerritoryId), "Teleporting to Destination", DConfig);
        }

        internal static unsafe bool? TeleporttoAethery(uint aetherytID, int targetTerritoryId)
        {
            if (IsInZone(targetTerritoryId) && PlayerNotBusy())
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
