using Dalamud.Interface.Utility.Raii;
using ECommons.DalamudServices;
using ImGuiNET;
using RootofRiches.IPC;
using RootofRiches.Scheduler;

namespace RootofRiches.Ui.MainWindow;

internal class NormalRaidFarm
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
    private static bool CopyButton = false;

    public static void Draw()
    {
        string NRaidTask = SchedulerMain.A4NTask;
        ImGui.Text($"Current Raid Task → {NRaidTask}");
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
        ImGui.Columns(3, null, false);
        ImGui.Text("Necessary Plugins");
        FancyCheckmark(P.bossmod.Installed);
        ImGui.SameLine();
        ImGui.Text("BossMod (VBM)");
        FancyCheckmark(P.navmesh.Installed);
        ImGui.SameLine();
        ImGui.Text("Navmesh");
        ImGui.NextColumn();
        ImGui.Text("Supported Combo Plugins");
        FancyCheckmark(true);
        ImGui.SameLine();
        ImGui.Text("BossMod (VBM)");
        FancyCheckmark(true);
        ImGui.SameLine();
        ImGui.Text("Wrath");
    }
}
