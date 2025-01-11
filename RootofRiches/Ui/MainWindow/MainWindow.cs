using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;
using Dalamud.Interface;

namespace RootofRiches.Ui.MainWindow;

internal class MainWindow : Window
{
    public MainWindow() :
    base($"Root of Riches {P.GetType().Assembly.GetName().Version}")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(430, 300),
            MaximumSize = new(9999, 9999)
        };
        TitleBarButtons.Add(new()
        {
            Click = (m) => { if (m == ImGuiMouseButton.Left) P.settingsWindow.IsOpen = !P.settingsWindow.IsOpen; },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Open settings window"),
        });
        P.windowSystem.AddWindow(this);
    }

    public void Dispose()
    {
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

        // Get the window height
        float windowHeight = ImGui.GetWindowHeight();

        // Set the cursor position for the button
        float buttonYPos = windowHeight - 30 - ImGui.GetStyle().WindowPadding.Y; // Subtract button height and bottom padding
        ImGui.SetCursorPosY(buttonYPos);

        bool isCtrlHeld = ImGui.GetIO().KeyCtrl;
        using (var _ = ImRaii.PushStyle(ImGuiStyleVar.Alpha, 0.5f, !ImGui.GetIO().KeyCtrl))
        {
            reset = ImGui.Button("RESET STATS", new Vector2(ImGui.GetContentRegionAvail().X, 30)) && ImGui.GetIO().KeyCtrl;
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(isCtrlHeld ? "Press to reset your stats." : "Hold Ctrl to enable the button.");
    }
    private void DrawStats(Stats stat)
    {
        if (C.HasUpdatedStats == false)
        {
            stat.FastestA4NRun = stat.FastestRun;
            stat.TotalTimeA4N = stat.TotalRunTime;
            C.HasUpdatedStats = true;
        }

        ImGui.Columns(3, null, false);

        // Top Middle Section
        ImGui.NextColumn();

        ImGuiEx.CenterColumnText(ImGuiColors.DalamudRed, "Root Of Riches", true);
        ImGuiHelpers.ScaledDummy(10f);

        // Setting up Next Columns for Stats
        ImGui.Columns(2, null, false);

        // Gil Earned / Total Time [Overall]
        ImGuiEx.CenterColumnText("Gil Earned", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Total Time [Overall]", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.GillEarned:N0}");
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText(stat.TotalRunTime.ToString(@"hh\:mm\:ss"));
        ImGuiHelpers.ScaledDummy(10f);


        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Total A4N Runs", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Total O3N Runs", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.TotalA4nRuns:N0}");
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText($"{stat.TotalO3nRuns:N0}");
        ImGuiHelpers.ScaledDummy(10f);

        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Total Time (A4N)", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Total Time (O3N)", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText(stat.TotalTimeA4N.ToString(@"hh\:mm\:ss"));
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText(stat.TotalTimeO3N.ToString(@"hh\:mm\:ss"));
        ImGuiHelpers.ScaledDummy(10f);

        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Fastest A4N Run", true);
        ImGui.NextColumn();
        ImGuiEx.CenterColumnText("Fastest O3N Run", true);
        ImGui.NextColumn();
        if (stat.FastestA4NRun == TimeSpan.MaxValue)
        {
            ImGuiEx.CenterColumnText("No Recorded Time!");
        }
        else
        {
            ImGuiEx.CenterColumnText(stat.FastestA4NRun.ToString(@"hh\:mm\:ss"));
        }
        ImGui.NextColumn();
        if (stat.FastestO3NRun == TimeSpan.MaxValue)
        {
            ImGuiEx.CenterColumnText("No Recorded Time!");
        }
        else
        {
            ImGuiEx.CenterColumnText(stat.FastestO3NRun.ToString(@"hh\:mm\:ss"));
        }

        ImGui.Columns(1, null, false);
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0, 20));
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("RoR tabbar",
                        ("Turnin Items", TurninUi.Draw, null, true),
                        ("Normal Raid Farm", NormalRaidFarm.Draw, null, true),
                        ("Stats", DrawStatsTab, null, true),
                        ("Calculator", CalculatorUi.Draw, null, true),
                        ("About", AboutUi.Draw, null, true)
                        );
    }
}
