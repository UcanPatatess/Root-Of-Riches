using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ECommons.SimpleGui;
using ECommons.Throttlers;
using RootofRiches.Scheduler;
using ImGuiNET;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Interface.Windowing;

namespace RootofRiches.Windows;

internal class MainWindow : Window
{
    public MainWindow() : 
        base($"Root of Riches {P.GetType().Assembly.GetName().Version}") 
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(100, 100),
            MaximumSize = new Vector2(800, 600)
        };
        P.windowSystem.AddWindow(this);
    }
    public void Dispose() 
    {
    }
    private string CurrentTask()
    {
        if (P.taskManager.NumQueuedTasks > 0 && P.taskManager.CurrentTask != null)
        {
            return P.taskManager.CurrentTask.Name?.ToString() ?? "None";
        }
        return "None";
    }
    private void DrawStatsTab()
    {
        if (ImGui.BeginTabBar("Stats"))
        {
            if (ImGui.BeginTabItem("Lifetime"))
            {
                this.DrawStatsTab(C.Stats, out bool reset);

                if (reset)
                {
                    C.Stats = new();
                    C.Save();
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Session"))
            {
                this.DrawStatsTab(C.SessionStats, out bool reset);
                if (reset)
                    C.SessionStats = new();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }
    private void DrawStatsTab(Stats stat, out bool reset)
    {   
        DrawStats(stat);

        bool isCtrlHeld = ImGui.GetIO().KeyCtrl;
        using (var _ = ImRaii.PushStyle(ImGuiStyleVar.Alpha, 0.5f, !ImGui.GetIO().KeyCtrl))
            reset = ImGui.Button("RESET STATS", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y)) && ImGui.GetIO().KeyCtrl;
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(isCtrlHeld ? "Press to reset your stats." : "Hold Ctrl to enable the button.");
    }
    private void DrawStats(Stats stat)
    {
        ImGui.BeginChild("Stats", new Vector2(0, ImGui.GetContentRegionAvail().Y - 30f), true);
        ImGui.Columns(3, null, false);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText(ImGuiColors.DalamudRed, "Root Of Riches", true);
        ImGuiHelpers.ScaledDummy(10f);
        ImGui.Columns(2, null, false);
        ImGui.NextColumn();
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("GillEarned", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("TotalA4nRuns", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.GillEarned.ToString("N0")}");
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.TotalA4nRuns.ToString("N0")}");
        ImGuiHelpers.ScaledDummy(10f);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Current Run Time", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Fastest Run Time", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.TotalRunTime.ToString(@"hh\:mm\:ss")}");
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.FastestRun.ToString(@"mm\:ss\.fff")}");

        ImGui.EndChild();
    }
    private void DrawA4N()
    {
        ImGui.Separator();

        // Define column widths
        float labelWidth = 120.0f; // Width reserved for labels
        float inputWidth = 130.0f; // Width reserved for input fields

        // Row 4: Raid Options
        ImGui.Text("Raid Options");
        ImGui.SameLine(labelWidth); // Align the next item to the right
        ImGui.PushItemWidth(inputWidth);
        if (ImGui.BeginCombo("##Root of Riches - NRaid Farm", nRaidString))
        {
            foreach (var option in nRaidOptions)
            {
                if (ImGui.Selectable(option, option == nRaidString))
                {
                    nRaidString = option;
                    C.RaidOption = nRaidString;
                }
                if (option == nRaidString)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        if (nRaidString == nRaidOptions[0])
        {
            RunInfinite = true;
        }
        else if (nRaidString == nRaidOptions[1])
        {
            RunInfinite = false;
            ImGui.SameLine();
            if (ImGui.InputInt("##Root of Riches ", ref amountToRun))
            {
                if (amountToRun < 1)
                    amountToRun = 1;
                else if (amountToRun > 5000)
                    amountToRun = 5000;
                RunAmount = amountToRun;
            }
        }
        ImGui.EndTabItem();

        // Row 1: Return Inn
        ImGui.Text("Return Inn");
        ImGui.SameLine(labelWidth); // Align the next item to the right
        ImGui.PushItemWidth(inputWidth);
        using (ImRaii.Disabled(!enableReturnInn))
        {
            if (ImGui.BeginCombo("##Root of Riches - NInn Settings", nInnString))
            {
                foreach (var option in nInnOptions)
                {
                    if (ImGui.Selectable(option, option == nInnString))
                    {
                        nInnString = option;
                        C.InnOption = nInnString;
                    }
                    if (option == nInnString)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("Enable Return Inn", ref enableReturnInn))
        {
            C.EnableReturnInn = enableReturnInn;
        }
        if (nInnString == nInnOptions[0])
            C.InnDataID = LimsaInn;
        else if (nInnString == nInnOptions[1])
            C.InnDataID = UlDahInn;
        else if (nInnString == nInnOptions[2])
            C.InnDataID = GridaniaInn;
        ImGui.PopItemWidth();

        // Row 2: Repair Mode
        ImGui.Text("Repair Mode");
        ImGui.SameLine(labelWidth); // Align the next item to the right
        ImGui.PushItemWidth(inputWidth);
        using (ImRaii.Disabled(!enableRepairMode))
        {
            if (!enableReturnInn && nRepairMode == nrepairOptions[1])
            {
                nRepairMode = nrepairOptions[0];  // Set it to "Self Repair"
                C.RepairMode = nRepairMode;        // Update the RepairMode
            }
            if (ImGui.BeginCombo("##RepairMode", nRepairMode))
            {
                foreach (var option in nrepairOptions)
                {
                    // Disable "Repair at NPC" option if "Return Inn" is unchecked
                    if (option == nrepairOptions[1] && !enableReturnInn)
                    {
                        using (ImRaii.Disabled(true)) // Disable "Repair at NPC" option
                        {
                            if (ImGui.Selectable(option, option == nRepairMode))
                            {
                                nRepairMode = option;
                                C.RepairMode = nRepairMode;
                            }
                            if (option == nRepairMode)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }
                    }
                    else
                    {
                        if (ImGui.Selectable(option, option == nRepairMode))
                        {
                            nRepairMode = option;
                            C.RepairMode = nRepairMode;
                        }
                        if (option == nRepairMode)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                }
                ImGui.EndCombo();
            }
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("Enable Repair Mode", ref enableRepairMode))
        {
            C.EnableRepair = enableRepairMode;
        }
        ImGui.PopItemWidth();
        if (nRepairMode == nrepairOptions[0])
        {
            C.RepairOption = "Self Repair";
            C.EnableRepair = true;
        }
        else if (nRepairMode == nrepairOptions[1])
        {
            C.RepairOption = "Repair at NPC";
            C.EnableRepair = true;
        }
        else if (nRepairMode == nrepairOptions[2])
        {
            C.EnableRepair = false;
        }
        ImGui.PopItemWidth();

        // Row 3: Repair Threshold
        ImGui.Text("Repair Threshold");
        ImGui.SameLine(labelWidth); // Align the next item to the right
        ImGui.PushItemWidth(inputWidth);
        using (ImRaii.Disabled(!enableRepairMode))
        {
            if (ImGui.SliderFloat("##RepairThreshold", ref repairThreshold, 0, 99))
            {
                C.RepairSlider = repairThreshold;
            }
        }
        ImGui.PopItemWidth();

        ImGui.Separator();
    }

    private static int TotalA4NGear = 0;
    private static double TotalFCPoints = 0;
    private static int TotalGCBase = 0;
    private static int TotalGC10 = 0;
    private static int TotalGC15 = 0;
    private static int OilclothBase = 0;
    private static int Oilcloth10 = 0;
    private static int Oilcloth15 = 0;
    private static int A4NJewelry = 0;
    private static int A4NHand = 0;
    private static int A4NShoes = 0;
    private static int A4NBody = 0;
    private static int A4NLeg = 0;
    private static int VendorGil = 0;
    private static int A4NiLvl = 190;
    private static int SealBase = 1093;
    private static int Seal10 = 1203;
    private static int Seal15 = 1257;
    private static int OilclothBuy = 600;
    private static int OilclothSell = 360;
    private static double FCPointCalc = 1.5;
    private static int A4NBodySell = 978;
    private static int A4NLegSell = 978;
    private static int A4NShoeSell = 587;
    private static int A4NHandSell = 587;
    private static int A4NJewelrySell = 445;
    private static int A4NBoltpR = 2;
    private static int A4NShaftpR = 2;
    private static int A4NSpringpR = 2;
    private static int A4NPedalpR = 1;
    private static int A4NCrankpR = 1;

    private void DrawItemCalculator()
    {
        string[,] testTableData = new string[,]
        {
            { "Jewelry", $"{A4NJewelry:N0}"},
            { "Hands", $"{A4NHand:N0}"},
            { "Shoes", $"{A4NShoes:N0}"},
            { "Body", $"{A4NBody:N0}"},
            { "Legs", $"{A4NLeg:N0}"},
            { "Total Gear Pieces", $"{TotalA4NGear:N0}"}
        };

        string[,] fcPointTableData = new string[,]
        {
            { "Total FC Points", $"{TotalFCPoints:N0}"},
        };

        string[,] calculationTableData = new string[,]
        {
            { "0%%", $"{TotalGCBase:N0}", $"{OilclothBase:N0}" },
            { "10%%", $"{TotalGC10:N0}", $"{Oilcloth10:N0}" },
            { "15%%", $"{TotalGC15:N0}", $"{Oilcloth15:N0}" },
        };

        string[,] sellGearVendorTableData = new string[,]
        {
            { "Vendor Gil -> Gear", $"{VendorGil:N0}" }
        };

        if (ImGui.Button("Update Table"))
        {
            A4NJewelry = GetItemCount(GordianBoltID);
            A4NHand = GetItemCount(GordianCrankID) / 2;
            A4NShoes = GetItemCount(GordianPedalID) / 2;
            A4NBody = GetItemCount(GordianShaftID) / 4;
            A4NLeg = GetItemCount(GordianSpringID) / 4;
            TotalA4NGear = A4NJewelry + A4NHand + A4NShoes + A4NBody + A4NLeg;
            TotalFCPoints = TotalA4NGear * A4NiLvl * FCPointCalc;
            TotalGCBase = TotalA4NGear * SealBase;
            TotalGC10 = TotalA4NGear * Seal10;
            TotalGC15 = TotalA4NGear * Seal15;
            OilclothBase = (TotalGCBase / OilclothBuy) * OilclothSell;
            Oilcloth10 = (TotalGC10 / OilclothBuy) * OilclothSell;
            Oilcloth15 = (TotalGC15 / OilclothBuy) * OilclothSell;
            VendorGil = (A4NJewelry * A4NJewelrySell) + (A4NHand * A4NHandSell) + (A4NShoes * A4NShoeSell) + (A4NBody * A4NBodySell) + (A4NLeg * A4NLegSell);
        }
        ImGui.NewLine();

        if (ImGui.BeginTable("TestTable", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Set up columns without automatic headers
            ImGui.TableSetupColumn("Armory Slot");
            ImGui.TableSetupColumn("Amount of Gear");

            // Custom header row
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            string[] headers = { "Armory Slot", "Amount of Gear" };
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
            for (int i = 0; i < testTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = testTableData[i, col];
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

        ImGui.Spacing();

        if (ImGui.BeginTable("Vendor Sell Gear Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Render table rows
            for (int i = 0; i < sellGearVendorTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = sellGearVendorTableData[i, col];
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

        if (ImGui.BeginTable("Fc Points Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Render table rows
            for (int i = 0; i < fcPointTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 2; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = fcPointTableData[i, col];
                    var textSize = ImGui.CalcTextSize(text);
                    var columnWidth = ImGui.GetColumnWidth();
                    var cursorPosX = ImGui.GetCursorPosX();

                    // Set the cursor position to center the text
                    ImGui.SetCursorPosX(cursorPosX + (columnWidth - textSize.X) / 2.0f);
                    ImGui.Text(text);

                    if (col == 0 && text == "Total FC Points" && ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("You only gain fc points if you turn in directly to the Grand Company!");
                        ImGui.EndTooltip();
                    }
                }
            }

            ImGui.EndTable();
        }

        ImGui.Spacing();

        if (ImGui.BeginTable("Oilcloth Tables", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            // Set up columns without automatic headers
            ImGui.TableSetupColumn("Seal Buff");
            ImGui.TableSetupColumn("Total Seals");
            ImGui.TableSetupColumn("Total Gil");

            // Custom header row
            ImGui.TableNextRow(ImGuiTableRowFlags.Headers);

            string[] headers = { "Seal Buff", "Total Seals", "Total Gil" };
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
            for (int i = 0; i < calculationTableData.GetLength(0); i++)
            {
                ImGui.TableNextRow();

                for (int col = 0; col < 3; col++)
                {
                    ImGui.TableSetColumnIndex(col);

                    // Calculate the available space and text size
                    var text = calculationTableData[i, col];
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

    }

    private void DrawAmountCalculator()
    {
        // wanna add a loop calculator here, that way you can see how many loops/
    }

    // Counter/Inputs
    private int amountToRun = RunAmount;
    private bool enableReturnInn = C.EnableReturnInn;
    private bool enableRepairMode = C.EnableRepair;
    private string nRaidString = C.RaidOption;
    private string[] nRaidOptions = { "Infinite", "Run x times" };
    private string nInnString = C.InnOption;
    private string[] nInnOptions = { "Limsa", "Ul'Dah", "Gridania" };
    private string nRepairMode = C.RepairMode;
    private string[] nrepairOptions = { "Self Repair", "Repair at NPC"};
    private float repairThreshold = C.RepairSlider;
    public override void Draw()
    {
        if (ImGui.BeginTabBar("##Main Window Tabs"))
        {
            if (ImGui.BeginTabItem("Turnin Items"))
            {
                ImGui.Text($"Current task (Ice) is: {icurrentTask}");
                ImGui.Text($"Current task is: {CurrentTask()}");
                ImGui.Text($"Number of task: {P.taskManager.NumQueuedTasks}");
                ImGui.Text($"Exchange Item Count: " + TotalExchangeItem);
                ImGui.SameLine();
                ImGui.Text($"GordianTurnIn Count: " + GordianTurnInCount);
                ImGui.Text($"AlexandrianTurnIn Count: " + AlexandrianTurnInCount);
                ImGui.SameLine();
                ImGui.Text($"DeltascapeTurnIn Count: " + DeltascapeTurnInCount);
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
                ImGui.SameLine();
                if (ImGuiEx.IconButton(FontAwesomeIcon.Wrench, "Settings"))
                    EzConfigGui.WindowSystem.Windows.FirstOrDefault(w => w.WindowName == SettingMenu.WindowName)!.IsOpen ^= true;
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Normal Raid Farm"))
            {
                TimeSpan currentTime = P.stopwatch.Elapsed;
                string currentTimeF = currentTime.ToString(@"mm\:ss\.fff");
                ImGui.Text($"Time Elapsed is: {currentTimeF}");
                if (SchedulerMain.DoWeTick && SchedulerMain.RunA4N)
                {
                    if (RunInfinite)
                    {
                        ImGui.Text($"Currently on run # {SchedulerMain.NRaidRun}");
                    }
                    else if (!RunInfinite)
                    {
                        ImGui.Text($"Currently on run {SchedulerMain.NRaidRun} / {RunAmount}");
                    }
                }
                else
                {
                    ImGui.Text("Raid mode is idle");
                }
                using (ImRaii.Disabled(!EnableNormalRaidFarm()))
                {

                    DrawA4N();

                    if (ImGui.Button(SchedulerMain.DoWeTick ? "Stop" : "Start A4N"))
                    {
                        if (SchedulerMain.DoWeTick)
                        {
                            SchedulerMain.DisablePlugin(); // Call DisablePlugin if running
                        }
                        else
                        {
                            SchedulerMain.EnablePlugin(); // Call EnablePlugin if not running
                            SchedulerMain.RunA4N = true;
                        }
                    }
                }
            }
            if (ImGui.BeginTabItem("Stats"))
            {
                DrawStatsTab();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Calculator"))
            {
                DrawItemCalculator();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }
}
