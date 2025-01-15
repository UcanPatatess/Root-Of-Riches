using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using ImGuiNET;
using RootofRiches.Scheduler;
using System.Numerics;

namespace RootofRiches.Ui.MainWindow;

internal class NormalRaidFarm
{
    private static string ButtonName = "Normal Raid";

    public static void Draw()
    {
        if (ImGui.RadioButton("[A4N] Alexander - Burden of the Father", C.RaidSelected == 0))
        {
            C.RaidSelected = 0;
        }

        // #2 O3N
        if (ImGui.RadioButton("[O3N] Deltascape V3.0", C.RaidSelected == 1))
        {
            C.RaidSelected = 1;
        }
        string NRaidTask = SchedulerMain.NRaidTask;
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
        if (C.RaidSelected == 0)
            ButtonName = "A4N";
        else if (C.RaidSelected == 1)
            ButtonName = "O3N";
        using (ImRaii.Disabled(!EnableNormalRaidFarm() || (P.bossmod.Installed && PluginInstalled(AltBossMod)) ))
        {
            if (ImGui.Button(SchedulerMain.DoWeTick ? "Stop" : $"Start {ButtonName}"))
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
        if (P.bossmod.Installed && PluginInstalled(AltBossMod))
        {
            ImGui.SameLine();
            ImGuiEx.InfoMarker("Hey! You seem to have 2 different versions of bossmod installed. >.> \nPlease disable one of them before you can actually run it", ImGuiColors.DalamudRed);
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
            FancyPluginUiString(P.wrath.Installed, "Wrath", "https://love.puni.sh/ment.json");
            FancyPluginUiString(P.navmesh.Installed, "Navmesh", "https://puni.sh/api/repository/veyn");
            ImGui.NextColumn();


            ImGui.Text("Supported Combo Plugins");
            FancyPluginUiString(P.bossmod.Installed, "BossMod (VBM)", "https://puni.sh/api/repository/veyn");
            FancyPluginUiString(P.wrath.Installed, "Wrath", "https://love.puni.sh/ment.json");
            ImGui.Columns(1);
        }
        if (C.ShowSettingsInWindow)
        {
            if (ImGui.BeginChild("##NRaidSettings", new Vector2(510, 230), true))
            {
                SettingsWindow.NRaidFarmSettings.Draw();
            }
            ImGui.EndChild();
        }
    }
}
