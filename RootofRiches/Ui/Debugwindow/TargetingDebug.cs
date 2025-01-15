using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using ImGuiNET;
using RootofRiches.Scheduler.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.Debugwindow;

internal class TargetingDebug
{
    private static float TargetXPos = 0;
    private static float TargetYPos = 0;
    private static float TargetZPos = 0;
    private static IGameObject? GameObject = null;
    private static uint TargetDataID = 0;
    private static string TargetDataIDString = TargetDataID.ToString();

    public static void Draw()
    {
        if (TryGetObjectByDataId(LeftForeleg, out GameObject))
        {
            var x = GameObject;
            if (x != null)
            {
                ImGui.Text($"Manip's Left Leg should appear to the right: {x.Name}");
            }
        }
        if (Svc.Targets?.Target != null)
        {
            TargetXPos = (float)Math.Round(Svc.Targets.Target.Position.X, 2);
            TargetYPos = (float)Math.Round(Svc.Targets.Target.Position.Y, 2);
            TargetZPos = (float)Math.Round(Svc.Targets.Target.Position.Z, 2);
            // Get the GameObjectId and display it in the ImGui.Text box
            ImGui.Text($"Name: {Svc.Targets.Target.Name}");
            ImGui.Text($"GameObjectId: {Svc.Targets.Target.GameObjectId}");
            ImGui.Text($"DataID: {Svc.Targets.Target.DataId}");
            if (ImGui.Button("Copy DataID to clipboard"))
            {
                ImGui.SetClipboardText($"{Svc.Targets.Target.DataId}");
            }
            if (ImGui.Button("Copy GameObjectID to clipboard"))
            {
                ImGui.SetClipboardText($"{Svc.Targets.Target.GameObjectId}");
            }
            ImGui.Text($"Target Pos: X: {TargetXPos}, Y: {TargetYPos}, Z: {TargetZPos}");
            if (ImGui.Button("Copy Target XYZ"))
            {
                ImGui.SetClipboardText($"{TargetXPos.ToString("0.00", CultureInfo.InvariantCulture)}f, " +
           $"{TargetYPos.ToString("0.00", CultureInfo.InvariantCulture)}f, " +
           $"{TargetZPos.ToString("0.00", CultureInfo.InvariantCulture)}f");
            }
        }
        else
        {
            // Optionally display a message if no target is selected
            ImGui.Text("No target selected.");
        }

        if (ImGui.Button("Summoning Bell Interact"))
        {
            TaskInteract.Enqueue(SummoningBell);
        }

        ImGui.Text($"Is in Inn? {CurrentlyInnInn()}");
        if (ImGui.InputText("##Target Dataid", ref TargetDataIDString, 100))
        {
            if (uint.TryParse(TargetDataIDString, out uint newTargetDataID))
            {
                TargetDataID = newTargetDataID;
            }
        }

        ImGui.SameLine();
        if (ImGui.Button("Target the DataID"))
        {
            TaskTarget.Enqueue(TargetDataID);
        }
    }
}
