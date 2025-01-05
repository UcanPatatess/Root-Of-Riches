using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;

namespace RootofRiches.Ui.MainWindow;

internal class MainWindow : Window
{
    public MainWindow() :
    base($"Root of Riches {P.GetType().Assembly.GetName().Version}")
    {
        Flags = ImGuiWindowFlags.NoCollapse;
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
