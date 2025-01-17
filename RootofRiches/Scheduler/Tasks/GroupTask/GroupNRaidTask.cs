using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Scheduler.Tasks.GroupTask
{
    internal static class GroupNRaidTask
    {
        private static ulong[] Entities;

        internal unsafe static void Enqueue()
        {
            uint ZoneID = CurrentZoneID();
            IGameObject? gameObject = null;

            if (ZoneID == A4NMapID)
                Entities = A4NEnemies;
            else if (ZoneID == O3NMapID)
                Entities = O3NEntities;

            if (TryGetClosestEnemy(out gameObject) && gameObject != null)
            {
                TaskTarget.Enqueue(gameObject.DataId);
            }
        }
    }
}
