using ECommons.DalamudServices;
using ImGuiNET;
using RootofRiches.Scheduler.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.Debugwindow;

internal class TurninDebug
{
    private static string AddonName = "default";
    private static float XPos = 0;
    private static float YPos = 0;
    private static float ZPos = 0;
    private static int Tolerance = 0;

    private static string CurrentTask()
    {
        if (P.taskManager.NumQueuedTasks > 0 && P.taskManager.CurrentTask != null)
        {
            return P.taskManager.CurrentTask.Name?.ToString() ?? "None";
        }
        return "None";
    }

    public static void Draw()
    {
        ImGui.Text($"Free Inventory Slots: {GetInventoryFreeSlotCount()}");
        ImGui.Text($"General Information");
        ImGui.Text($"Current Task: {CurrentTask()}");
        ImGui.Text($"TerritoryID: " + Svc.ClientState.TerritoryType);
        ImGui.Text($"Target: " + Svc.Targets.Target);
        ImGui.InputText("##Addon Visible", ref AddonName, 100);
        ImGui.SameLine();
        ImGui.Text($"Addon Visible: " + IsAddonActive(AddonName));
        ImGui.Text($"Navmesh information");
        ImGui.Text($"PlayerPos: " + PlayerPosition());
        ImGui.Text($"Navmesh BuildProgress :" + P.navmesh.BuildProgress());//working ipc
        ImGui.Text("X:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.InputFloat("##X Position", ref XPos))
        {
            XPos = (float)Math.Round(XPos, 2);
        }
        ImGui.SameLine();
        ImGui.Text("Y:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.InputFloat("##Y Position", ref YPos))
        {
            YPos = (float)Math.Round(YPos, 2);
        }
        ImGui.SameLine();
        ImGui.Text("Z:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(75);
        if (ImGui.InputFloat("##Z Position", ref ZPos))
        {
            ZPos = (float)Math.Round(ZPos, 2);
        }
        ImGui.SameLine();
        ImGui.Text("Tolerance:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        ImGui.InputInt("##Tolerance", ref Tolerance);
        if (ImGui.Button("Set to Current"))
        {
            XPos = (float)Math.Round(GetPlayerRawXPos(), 2);
            YPos = (float)Math.Round(GetPlayerRawYPos(), 2);
            ZPos = (float)Math.Round(GetPlayerRawZPos(), 2);
        }
        ImGui.SameLine();
        if (ImGui.Button("Copy to clipboard"))
        {
            string clipboardText = string.Format(CultureInfo.InvariantCulture, "{0:F2}f, {1:F2}f, {2:F2}f", XPos, YPos, ZPos);
            ImGui.SetClipboardText(clipboardText);
        }
        if (ImGui.Button("Vnav Moveto!"))
        {
            P.taskManager.Enqueue(() => TaskMoveTo.Enqueue(new Vector3(XPos, YPos, ZPos), "Interact string", Tolerance));
            ECommons.Logging.InternalLog.Information("Firing off Vnav Moveto");
        }
        if (ImGui.Button("MergeInv"))
            TaskMergeInv.Enqueue();

        ImGui.SameLine();
        if (ImGui.Button("RepairNpc"))
            TaskRepairNpc.Enqueue();

        ImGui.SameLine();
        if (ImGui.Button("Abort Tasks"))
            P.taskManager.Abort();

        if (ImGui.Button("Self Repair"))
            TaskSelfRepair.Enqueue();

        ImGui.SameLine();
        if (ImGui.Button("Teleport Inn"))
            TaskTeleportInn.Enqueue();

        ImGui.SameLine();
        if (ImGui.Button("TaskGetIntoInn"))
            TaskGetIntoInn.Enqueue();

        if (ImGui.Button("Task Escape"))
            TaskGetOut.Enqueue();

        if (ImGui.Button("Resend Retainers"))
            TaskUseRetainer.Enqueue();

        ImGui.SameLine();
        if (ImGui.Button("Resend Deployables"))
        {
            TaskDoDeployables.Enqueue();
        }
    }
}
