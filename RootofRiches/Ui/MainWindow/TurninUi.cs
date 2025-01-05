using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using RootofRiches.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;

namespace RootofRiches.Ui.MainWindow;

internal class TurninUi
{
    private static bool MaxItem = C.MaxItem;
    private static bool MaxArmory = C.MaxArmory;
    private static int MaxArmoryFreeSlot = C.MaxArmoryFreeSlot;
    private static bool SellOilCloth = C.SellOilCloth;
    private static bool TeleportToFC = C.TeleportToFC;
    private static string[] Options = { "Vendor Turn-in", "Gc Turn-in" };
    private static int SelectedOption = C.VendorTurnIn ? 0 : 1; // Map boolean state to dropdown index

    public static void Draw()
    {
        ImGui.Text($"Current TurnIn Task → {icurrentTask}");

        string[,] ExchangeTable = new string[,]
        {
            { "Exchangeable Armor", $"{TotalExchangeItem:N0}" }
        };

        string[,] NormalRaidParts = new string[,]
        {
            { "Gordian", "A1-4", $"{GordianTurnInCount:N0}" },
            { "Alexandrian", "A9-12", $"{AlexandrianTurnInCount:N0}" },
            { "Deltascape", "O1-4", $"{DeltascapeTurnInCount:N0}" },
        };

        if (ImGui.BeginTable("RoRAmountofArmorPieces", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Render table rows
            for (int i = 0; i < ExchangeTable.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = ExchangeTable[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);
                }
            }

            ImGui.EndTable();
        }
        if (ImGui.BeginTable("ROR NRaid Piece Total", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Set up columns without automatic headers
            ImGui.TableSetupColumn("Raid Series");
            ImGui.TableSetupColumn("Raid Tier");
            ImGui.TableSetupColumn("Total Armor Amount");

            // Custom header row
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            string[] headers = { "Raid Series", "Raid Tier", "Total Armor Amount" };
            for (int col = 0; col < headers.Length; col++)
            {
                ImGui.TableSetColumnIndex(col);

                // Calculate the available space and text size
                var header = headers[col];
                var textSize = ImGui.CalcTextSize(header);
                var columnWidth = ImGui.GetColumnWidth();
                var cursorPosX = ImGui.GetCursorPosX();

                // Set the cursor position to center the text
                ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                ImGui.Text(header);
            }

            // Render table rows
            for (int i = 0; i < NormalRaidParts.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 3; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = NormalRaidParts[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);
                }
            }

            ImGui.EndTable();
        }

        ImGui.BeginChild("Turnin Settings");

        // Setting up the columns for layout
        ImGui.Columns(2, "Turnin Settings Columns", false);

        // Columm 1
        // Get the window width
        float windowWidth = ImGui.GetWindowWidth();

        // Set the width of the first column to half the window width
        ImGui.SetColumnWidth(0, windowWidth / 2);

        ImGui.Text("Inventory Settings");
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
        ImGui.Text("Turn-in Settings");
        ImGui.Separator();

        // VendorTurnIn
        ImGui.PushItemWidth(130);
        ImGui.NewLine();
        if (ImGui.Combo("##turn-insettings", ref SelectedOption, Options, Options.Length))
        {
            // Update the property based on the selected option
            C.VendorTurnIn = (SelectedOption == 0);
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

        ImGui.Columns(1);

        if (ImGui.Button(SchedulerMain.DoWeTick ? "Stop" : "Start Turnin"))
        {
            if (SchedulerMain.DoWeTick)
            {
                SchedulerMain.DisablePlugin(); // Call DisablePlugin if running
            }
            else
            {
                SchedulerMain.EnablePlugin(); // Call EnablePlugin if not running
                SchedulerMain.RunTurnin = true;
            }
        }

        ImGui.EndChild();
    }
}
