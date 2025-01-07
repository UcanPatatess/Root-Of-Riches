
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using RootofRiches.Scheduler;

namespace RootofRiches.Ui.SettingsWindow;

internal class TurninSettingsUi
{
    private static bool MaxItem = C.MaxItem;
    private static bool MaxArmory = C.MaxArmory;
    private static int MaxArmoryFreeSlot = C.MaxArmoryFreeSlot;
    private static bool SellOilCloth = C.SellOilCloth;
    private static bool TeleportToFC = C.TeleportToFC;
    private static bool MountUp = C.EnableMountUp;
    private static string[] Options = { "Vendor Turn-in", "Gc Turn-in" };
    private static int SelectedOption = C.VendorTurnIn ? 0 : 1; // Map boolean state to dropdown index

    public static void Draw()
    {
        ImGui.BeginChild("Turnin Settings");

        // Setting up the columns for layout
        ImGui.Columns(2, "Turnin Settings Columns", false);

        // Columm 1
        // Get the window width
        float windowWidth = ImGui.GetWindowWidth();

        // Set the width of the first column to half the window width
        ImGui.SetColumnWidth(0, windowWidth / 2);

        ImGui.Text("Inventory Managment");
        ImGui.Spacing();
        ImGui.Text("Free Main Inventory Slots");
        ImGuiComponents.HelpMarker("Select how many slots you want in the inventory open.\nGood to use if you're buying multiple stack of Oilcloth for instance.");
        ImGui.PushItemWidth(130);
        if (ImGui.InputInt("##maxarmoryfreeslot", ref MaxArmoryFreeSlot))
        {
            if (MaxArmoryFreeSlot < 0) MaxArmoryFreeSlot = 0;
            C.MaxArmoryFreeSlot = MaxArmoryFreeSlot;
        }
        if (ImGui.Checkbox("Maximize Inventory##maxitem", ref MaxItem))
        {
            C.MaxItem = MaxItem;
            C.ChangeArmory = false;
        }
        ImGui.SameLine();
        ImGuiComponents.HelpMarker("Maximize inventory by buying one of a single item.");
        using (ImRaii.Disabled(!C.MaxItem))
        {
            if (MaxItem == false)
            {
                C.MaxArmory = false;
                MaxArmory = false;
            }
            if (ImGui.Checkbox("Fill Armory##maxarmory", ref MaxArmory))
            {
                C.ChangeArmory = false;
                C.MaxArmory = MaxArmory;
            }
        }

        ImGui.NewLine();

        ImGui.NextColumn();

        // Column 2
        ImGui.Text("Turn-in Managment");
        ImGui.Separator();

        // VendorTurnIn
        ImGui.PushItemWidth(130);
        ImGui.NewLine();
        if (ImGui.Combo("##turn-insettings", ref SelectedOption, Options, Options.Length))
        {
            // Update the property based on the selected option
            C.VendorTurnIn = SelectedOption == 0;
        }
        string helpMarkerContent = C.VendorTurnIn
        ? "Stay off the rankings and sell to your retainer."
        : "You will gain more gil, but be careful of the rankings.";
        ImGui.SameLine();
        ImGuiComponents.HelpMarker(helpMarkerContent);
        ImGui.PopItemWidth();
        using (ImRaii.Disabled(!C.VendorTurnIn))
        {
            if (!C.VendorTurnIn)
                TeleportToFC = false;

            if (ImGui.Checkbox("Teleport to FC##teleporttofc", ref TeleportToFC))
            {
                C.TeleportToFC = TeleportToFC;
            }
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Teleport to your Fc to sell gears.");
        }
        if (ImGui.Checkbox("Sell OilCloth Turn-in##SellOilCloth", ref SellOilCloth))
        {
            C.SellOilCloth = SellOilCloth;
        }
        // mounting setting
        if (ImGui.Checkbox("Mount Up",ref MountUp))
        {
            C.EnableMountUp = MountUp;
        }
        ImGui.SameLine();
        ImGuiComponents.HelpMarker("Mount Up while turning in.");

        ImGui.Columns(1);
        ImGui.EndChild();
    }
}
