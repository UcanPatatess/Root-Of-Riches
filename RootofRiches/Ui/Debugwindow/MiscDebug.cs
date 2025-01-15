using ImGuiNET;
using RootofRiches.Scheduler.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
