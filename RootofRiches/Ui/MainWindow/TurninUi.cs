using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using ImGuiNET;
using RootofRiches.Scheduler;
using RootofRiches.Scheduler.Tasks;

namespace RootofRiches.Ui.MainWindow;

internal class TurninUi
{
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

        string sellItem = string.Empty;

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
        using (ImRaii.Disabled(!EnableTurnIn() || !IsOnHomeWorld()))
        {
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
        }
        if (!IsOnHomeWorld())
        {
            if (C.VendorTurnIn)
            {
                sellItem = "Armor pieces";
            }
            else if (C.SellOilCloth)
            {
                sellItem = "Oil Cloth";
            }
            ImGui.SameLine();
            FontAwesome.Print(ImGuiColors.DalamudRed, FontAwesome.Cross);
            ImGui.SameLine();
            ImGui.TextWrapped($"You're not on your homeworld! Can't sell {sellItem} to retainers. Please change settings or return back to main world to start.");
        }
        using (ImRaii.Disabled(!ClosetoVendor()))
        {
            if (ImGui.Button(P.taskManager.NumQueuedTasks > 0 ? "Stop" : "Turnin to Vendor"))
            {
                if (P.taskManager.NumQueuedTasks > 0)
                    P.taskManager.Abort();
                else
                {
                    if (!C.ChangeArmory)
                    {
                        TaskChangeArmorySetting.Enqueue();
                        C.ChangeArmory = true;
                    }
                    TaskTurnIn.Enqueue();
                }
            }
        }
        if (!EnableTurnIn())
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Red color (RGBA)
            ImGui.Text("You Are Missing Some Plugins");
            ImGui.PopStyleColor();

            ImGui.Columns(2, null, false);
            ImGui.Text("Necessary Plugins");
            FancyPluginUiString(P.navmesh.Installed, "Navmesh", "https://puni.sh/api/repository/veyn");
            FancyPluginUiString(P.lifestream.Installed, "Lifestream", "https://github.com/NightmareXIV/MyDalamudPlugins/raw/main/pluginmaster.json");
            ImGui.NextColumn();
            ImGui.Dummy(new(0, 20));
            FancyPluginUiString(P.autoRetainer.Installed, "AutoRetainer", "https://love.puni.sh/ment.json");
            FancyPluginUiString(P.deliveroo.Installed, "Deliveroo", "https://plugins.carvel.li");
            ImGui.Columns(1);
        }
    }
}
