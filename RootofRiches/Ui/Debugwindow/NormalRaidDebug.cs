using ECommons.DalamudServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameHelpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.Debugwindow;

internal class NormalRaidDebug
{
    private static string PluginName = "none";
    private static string CommandInput = "";
    private static bool RSenabled = false;
    private static int InputBox = 0;
    private static uint TestDutyID = 0;

    public static void Draw()
    {
        int zoneID = Svc.ClientState.TerritoryType;
        ImGui.Text($"Current Zone ID is: {zoneID}");
        if (ImGui.Button("Copy Current Zone ID"))
        {
            ImGui.SetClipboardText($"{zoneID}");
        }

        ImGui.InputText("Plugin Name Check", ref PluginName, 100);
        if (PluginInstalled(PluginName))
        {
            ImGui.Text($"Plugin: {PluginName} is installed");
        }
        else
        {
            ImGui.Text($"Plugin: {PluginName} is not visible");
        }
        ImGui.InputText("Command", ref CommandInput, 500);
        if (ImGui.Button("Run Command"))
        {
            RunCommand(CommandInput);
        }
        if (ImGui.Button("Add Passive Preset"))
        {
            P.bossmod.RefreshPreset("RoR Passive", Resources.BMRotations.rootPassive);
            ImGui.SetClipboardText($"{Resources.BMRotations.rootPassive}");
        }
        if (ImGui.Button(RSenabled ? "Disable" : "Enable"))
        {
            if (RSenabled)
            {
                ToggleRotation(false);
                RSenabled = false;
            }
            else
            {
                ToggleRotation(true);
                RSenabled = true;
            }
        }

        if (ARRetainersWaitingToBeProcessed())
            ImGui.Text("Available Retainers on this Character");
        else if (!ARRetainersWaitingToBeProcessed())
            ImGui.Text("No Retainers Available");
        else
            ImGui.Text("Retainer Service not available");

        if (ARSubsWaitingToBeProcessed())
            ImGui.Text("Available Deployables on this Character");
        else if (!ARSubsWaitingToBeProcessed())
            ImGui.Text("No Deployables Available");
        else
            ImGui.Text("Deployables Service not available");

        if (Player.Territory.EqualsAny([.. Houses.List]))
            ImGui.Text("In House");
        else
            ImGui.Text("Not In House");

        if (ImGui.Button("Simple Enable Wrath"))
        {
            P.taskManager.Enqueue(() => EnableWrathAuto());
        }
        if (ImGui.Button("Enable Advanced Wrath Rotation"))
        {
            P.taskManager.Enqueue(() => EnableWrathAutoAndConfigureIt());
        }
        if (ImGui.Button("Release Wrath Control"))
        {
            P.taskManager.Enqueue(() => ReleaseWrathControl());
        }
        if (ImGui.InputInt("##Test Duty Selected", ref InputBox))
        {
            if (InputBox >= 0)
            {
                TestDutyID = (uint)InputBox;
            }
        }
        ImGui.Text($"TestDutyID = {TestDutyID}");
        if (ImGui.Button("Duty Finder"))
        {
            OpenDuty(TestDutyID);
        }
        ImGui.Text($"{JournalDutyText()}");
        if (ImGui.Button("Copy Journal Text"))
        {
            ImGui.SetClipboardText($"{JournalDutyText()}");
        }
    }
}
