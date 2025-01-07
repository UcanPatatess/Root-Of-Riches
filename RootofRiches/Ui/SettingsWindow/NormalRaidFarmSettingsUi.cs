
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace RootofRiches.Ui.SettingsWindow;

internal class NormalRaidFarmSettingsUi
{
    // Counter/Inputs
    private static int AmountToRun = RunAmount;
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

    public static void Draw()
    {
        using (ImRaii.Disabled(!EnableNormalRaidFarm()))
        {

            float windowWidth = ImGui.GetWindowWidth();
            // Define column widths
            float labelWidth = windowWidth / 3; // Width reserved for labels
            float inputWidth = windowWidth / 3; // Width reserved for input fields

            // Row 1: Raid Options
            ImGui.Text("Raid Options");
            ImGui.SameLine(labelWidth); // Align the next item to the right
            ImGui.PushItemWidth(inputWidth);
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
            if (NRaidString == NRaidOptions[0])
            {
                RunInfinite = true;
            }
            else if (NRaidString == NRaidOptions[1])
            {
                RunInfinite = false;
                ImGui.SameLine();
                if (ImGui.InputInt("##Root of Riches ", ref AmountToRun))
                {
                    if (AmountToRun < 1)
                        AmountToRun = 1;
                    else if (AmountToRun > 5000)
                        AmountToRun = 5000;
                    RunAmount = AmountToRun;
                }
            }
            ImGui.EndTabItem();

            // Row 2: Return Inn
            ImGui.Text("Return Inn");
            ImGui.SameLine(labelWidth); // Align the next item to the right
            ImGui.PushItemWidth(inputWidth);
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
            ImGui.SameLine();
            if (ImGui.Checkbox("Enable Return Inn", ref EnableReturnInn))
            {
                C.EnableReturnInn = EnableReturnInn;
            }
            if (NInnString == NInnOptions[0])
                C.InnDataID = LimsaInn;
            else if (NInnString == NInnOptions[1])
                C.InnDataID = UlDahInn;
            else if (NInnString == NInnOptions[2])
                C.InnDataID = GridaniaInn;
            ImGui.PopItemWidth();

            // Row 3: Repair Mode
            ImGui.Text("Repair Mode");
            ImGui.SameLine(labelWidth); // Align the next item to the right
            ImGui.PushItemWidth(inputWidth);
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
            ImGui.SameLine();
            if (ImGui.Checkbox("Enable Repair Mode", ref EnableRepairMode))
            {
                C.EnableRepair = EnableRepairMode;
            }
            ImGui.PopItemWidth();
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
            ImGui.PopItemWidth();

            // Row 4: Repair Threshold
            ImGui.Text("Repair Threshold");
            ImGui.SameLine(labelWidth); // Align the next item to the right
            ImGui.PushItemWidth(inputWidth);
            using (ImRaii.Disabled(!EnableRepairMode))
            {
                if (ImGui.SliderFloat("##RepairThreshold", ref RepairThreshold, 0, 99))
                {
                    C.RepairSlider = RepairThreshold;
                }
            }
            ImGui.PopItemWidth();

            // Row 5: Use Auto Retainer
            ImGui.Text("Use Auto Retainer");
            ImGui.SameLine(labelWidth); // Align the next item to the right
            ImGui.PushItemWidth(inputWidth);
            using (ImRaii.Disabled(!EnableReturnInn))
            {
                if (ImGui.Checkbox("", ref EnableAutoRetainer))
                    C.EnableAutoRetainer = EnableAutoRetainer;
                if (!C.EnableReturnInn)
                {
                    EnableAutoRetainer = false;
                    C.EnableAutoRetainer = false;
                }

            }
            ImGui.PopItemWidth();
        }
    }
}
