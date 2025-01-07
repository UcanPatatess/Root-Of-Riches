using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using RootofRiches.Scheduler;

namespace RootofRiches.Ui.MainWindow;

internal class NormalRaidFarm
{
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

        using (ImRaii.Disabled(!EnableNormalRaidFarm()))
        {
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
        if (!EnableNormalRaidFarm())
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Red color (RGBA)
            ImGui.Text("You Are Missing Some Plugins");
            ImGui.PopStyleColor();

            ImGui.Columns(2, null, false);
            ImGui.Text("Necessary Plugins");
            FancyPluginUiString(P.bossmod.Installed, "BossMod (VBM)", "https://puni.sh/api/repository/veyn");
            FancyPluginUiString(P.navmesh.Installed, "Navmesh", "https://puni.sh/api/repository/veyn");
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
}
