using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.Ui.SettingsWindow;

internal class NRaidFarmSettings
{
    // Counter/Inputs
    private static int AmountToRun = RunAmount;
    private static bool EnableMainSubs = C.EnableSubsMain;
    private static bool EnableReturnInn = C.EnableReturnInn;
    private static bool EnableRepairMode = C.EnableRepair;
    private static bool EnableAutoRetainer = C.EnableAutoRetainer;
    private static string NRaidString = C.RaidOption;
    private static string[] NRaidOptions = { "Infinite", "Run x times" };
    private static string NInnString = C.InnOption;
    private static string[] NInnOptions = { "Limsa", "Ul'Dah", "Gridania" };
    private static string NRepairMode = C.RepairMode;
    private static string[] NrepairOptions = { "Self Repair", "Repair at NPC" };
    private static float RepairThreshold = C.RepairSlider;
    private static bool ShowInWindow = C.ShowSettingsInWindow;

    public static void Draw()
    {
        if (ImGui.BeginTable("RoR NRaid Settings", 3))
        {
            // Headers
            ImGui.TableSetupColumn("Text Descriptions", ImGuiTableColumnFlags.WidthFixed, 160);
            ImGui.TableSetupColumn("Selectable Input Columns", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("2ndary Options", ImGuiTableColumnFlags.WidthFixed, 200);

            // First Row
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            ImGui.Text("Raid Options");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(150);
            if (ImGui.BeginCombo("##Root of Riches - NRaid Farm", NRaidString))
            {
                foreach (var option in NRaidOptions)
                {
                    if (ImGui.Selectable(option, option == NRaidString))
                    {
                        NRaidString = option;
                        C.RaidOption = NRaidString;
                    }
                    if (option == NRaidString)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.TableSetColumnIndex(2);
            if (NRaidString == NRaidOptions[0])
            {
                RunInfinite = true;
            }
            else if (NRaidString == NRaidOptions[1])
            {
                RunInfinite = false;
                ImGui.SetNextItemWidth(100);
                if (ImGui.InputInt("##Root of Riches ", ref AmountToRun))
                {
                    if (AmountToRun < 1)
                        AmountToRun = 1;
                    else if (AmountToRun > 5000)
                        AmountToRun = 5000;
                    RunAmount = AmountToRun;
                }
            }
            ImGui.TableNextRow();

            // Row #2, Return to Inn
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("Return Inn");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(150);
            using (ImRaii.Disabled(!EnableReturnInn))
            {
                if (ImGui.BeginCombo("##Root of Riches - NInn Settings", NInnString))
                {
                    foreach (var option in NInnOptions)
                    {
                        if (ImGui.Selectable(option, option == NInnString))
                        {
                            NInnString = option;
                            C.InnOption = NInnString;
                        }
                        if (option == NInnString)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                    ImGui.EndCombo();
                }
            }

            ImGui.TableSetColumnIndex(2);
            if (ImGui.Checkbox("##Root of Riches - Enable Return Inn", ref EnableReturnInn))
            {
                C.EnableReturnInn = EnableReturnInn;
            }
            if (NInnString == NInnOptions[0])
                C.InnDataID = LimsaInn;
            else if (NInnString == NInnOptions[1])
                C.InnDataID = UlDahInn;
            else if (NInnString == NInnOptions[2])
                C.InnDataID = GridaniaInn;

            ImGui.TableNextRow();

            // Row #3, Repairing Mode
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("Repair Mode");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(150);
            using (ImRaii.Disabled(!EnableRepairMode))
            {
                if (!EnableReturnInn && NRepairMode == NrepairOptions[1])
                {
                    NRepairMode = NrepairOptions[0];  // Set it to "Self Repair"
                    C.RepairMode = NRepairMode;        // Update the RepairMode
                }
                if (ImGui.BeginCombo("##RepairMode", NRepairMode))
                {
                    foreach (var option in NrepairOptions)
                    {
                        // Disable "Repair at NPC" option if "Return Inn" is unchecked
                        if (option == NrepairOptions[1] && !EnableReturnInn)
                        {
                            using (ImRaii.Disabled(true)) // Disable "Repair at NPC" option
                            {
                                if (ImGui.Selectable(option, option == NRepairMode))
                                {
                                    NRepairMode = option;
                                    C.RepairMode = NRepairMode;
                                }
                                if (option == NRepairMode)
                                {
                                    ImGui.SetItemDefaultFocus();
                                }
                            }
                        }
                        else
                        {
                            if (ImGui.Selectable(option, option == NRepairMode))
                            {
                                NRepairMode = option;
                                C.RepairMode = NRepairMode;
                            }
                            if (option == NRepairMode)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }
                    }
                    ImGui.EndCombo();
                }
            }

            ImGui.TableSetColumnIndex(2);

            if (ImGui.Checkbox("##Root of Riches - Enable Repair Mode", ref EnableRepairMode))
            {
                C.EnableRepair = EnableRepairMode;
            }
            if (NRepairMode == NrepairOptions[0])
            {
                C.RepairOption = "Self Repair";
                C.EnableRepair = true;
            }
            else if (NRepairMode == NrepairOptions[1])
            {
                C.RepairOption = "Repair at NPC";
                C.EnableRepair = true;
            }
            else if (NRepairMode == NrepairOptions[2])
            {
                C.EnableRepair = false;
            }

            ImGui.TableNextRow();

            // Row #4, Repair Threshold
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("Repair Threshold");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(150);
            using (ImRaii.Disabled(!EnableRepairMode))
            {
                if (ImGui.SliderFloat("##RepairThreshold", ref RepairThreshold, 0, 99))
                {
                    C.RepairSlider = RepairThreshold;
                }
            }

            ImGui.TableNextRow();

            // Row #5, Use Auto Retainer
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("Enable Retainers");
            ImGuiComponents.HelpMarker("Enable Retainers on this character. \nMulti Mode Retainers are coming soon");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(150);
            using (ImRaii.Disabled(GetCurrentWorld() != GetHomeWorld() || !EnableReturnInn || !P.autoRetainer.Installed))
            {
                if (ImGui.Checkbox("##MainRetCheckbox", ref EnableAutoRetainer))
                {
                }
                if (P.autoRetainer.Installed)
                    C.EnableAutoRetainer = EnableAutoRetainer;
                else
                    C.EnableAutoRetainer = false;
                if (!C.EnableReturnInn)
                {
                    EnableAutoRetainer = false;
                    C.EnableAutoRetainer = false;
                }
                if(GetCurrentWorld() != GetHomeWorld())
                {
                    C.EnableAutoRetainer = false;
                }
            }
            ImGuiEx.PluginAvailabilityIndicator([new("AutoRetainer")]);
            ImGui.TableNextRow();

            // Row #6, Enable Subs on Main
            ImGui.TableSetColumnIndex(0);
            ImGui.Text("Enable Subs");
            ImGuiComponents.HelpMarker("Enable subs on this character. \nMulti Mode subs are coming soon");
            
            ImGui.TableSetColumnIndex(1);
            using (ImRaii.Disabled(GetCurrentWorld() != GetHomeWorld() || !P.autoRetainer.Installed))
            {
                if (ImGui.Checkbox("##MainSubCheckbox", ref EnableMainSubs))
                {
                }
                if (P.autoRetainer.Installed)
                    C.EnableSubsMain = EnableMainSubs;
                else
                    C.EnableSubsMain = false;
            }
            ImGuiEx.PluginAvailabilityIndicator([new("AutoRetainer")]);
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            if (ImGui.Checkbox("Show Settings In Main Window", ref ShowInWindow))
            {
                if (ShowInWindow)
                    C.ShowSettingsInWindow = ShowInWindow;
                else C.ShowSettingsInWindow = false;
            }
        }
        ImGui.EndTable();
    }
}
