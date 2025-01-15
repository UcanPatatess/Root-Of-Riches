using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using RootofRiches.Scheduler.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.Debugwindow;

internal class MiscDebug
{
    public static void Draw()
    {
        if (ImGui.Button("Teleport to Gridania"))
        {
            TaskTeleport.Enqueue(GridaniaAether, Gridania);
        }
        if (ImGui.Button("Move to Retainer Bell"))
        {
            IGameObject? gameObject = null;
            Vector3 bellPos = new Vector3();
            P.taskManager.Enqueue(() => TryGetObjectByDataId(SummoningBell, out gameObject), "Getting Summoning Bell");
            if (gameObject != null && (GetDistanceToVectorPoint(bellPos) > 3))
                TaskMoveTo.Enqueue(bellPos, "Retainer Bell", 1);
        }
    }
}
