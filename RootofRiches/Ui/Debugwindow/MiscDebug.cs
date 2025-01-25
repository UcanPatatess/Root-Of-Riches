using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using ImGuiNET;
using Lumina.Excel.Sheets;
using RootofRiches.Scheduler.Tasks;
using System.Numerics;


namespace RootofRiches.Ui.Debugwindow;

internal class MiscDebug
{
    private static int InputBox = 0;
    private static int DictionaryValue = 0;
    private static uint SkillID = 0;

    private static string CurrentTask()
    {
        if (P.taskManager.NumQueuedTasks > 0 && P.taskManager.CurrentTask != null)
        {
            return P.taskManager.CurrentTask.Name?.ToString() ?? "None";
        }
        return "None";
    }

    public static void Draw()
    {
        if (ImGui.Button("Teleport to Gridania"))
        {
            TaskTeleport.Enqueue(GridaniaAether, Gridania);
        }
        if (ImGui.Button("Move to Retainer Bell"))
        {
            IGameObject? gameObject = null;
            Vector3 bellPos = new Vector3();
            P.taskManager.Enqueue(() => TryGetObjectByDataId(SummoningBell, out gameObject), "Getting Summoning Bell");
            if (gameObject != null && (GetDistanceToVectorPoint(bellPos) > 3))
                TaskMoveTo.Enqueue(bellPos, "Retainer Bell", 1);
        }
        FancyPluginUiString(P.pandora.Installed, "Pandora IPC Test", "");
        var isenabled = (P.pandora.GetFeatureEnabled("Auto-select Turn-ins") ?? false)
                && (P.pandora.GetConfigEnabled("Auto-select Turn-ins", "AutoConfirm") ?? false);

        FancyPluginUiString(isenabled, "Pandora Auto-select Turn-ins Test", "");

        ImGui.Text($"Job type: {GetJobType()}");

        if (ImGui.Button("Quest Toast"))
        {
            Svc.Toasts.ShowQuest("Test Toast");
        }
        if (ImGui.Button("Normal Toast"))
        {
            Svc.Toasts.ShowNormal("Normal Toast");
        }
        if (ImGui.Button("Error Toast"))
        {
            Svc.Toasts.ShowError("Error Toast");
        }

        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("##SpellID", ref InputBox))
        {
            if (InputBox >= 0)
            {
                SkillID = (uint)InputBox;
            }
        }
        ImGui.Text($"Current Recast Time: {GetRecastTime(SkillID)}");
        ImGui.Text($"Real Recast Time: {GetRealRecastTime(SkillID)}");
        ImGui.Text($"Recast Time Elasped: {GetRecastTimeElapsed(SkillID)}");
        ImGui.Text($"Spell Cooldown: {GetSpellCooldown(SkillID)}");
        if (ImGui.Button("Execute Action"))
        {
            ExecuteAction(SkillID);
        }
        if (ImGui.Button("Execute OGCD"))
        {
            ExecuteAbility(SkillID);
        }
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("##DictionaryID", ref DictionaryValue))
        {
            PluginLog.Log($"Dictionary value is: {DictionaryValue}");
        };
        if (ImGui.Button("Test Run of MCH Dictionary"))
        {
            TaskUseAction.Enqueue(DictionaryValue);
        }
        ImGui.Text($"Current Task: {P.taskManager.NumQueuedTasks}");


    }

    private static string GetJobType()
    {
        string Test = string.Empty;
        var j = Player.JobId;
        if (Svc.Data.GetExcelSheet<ClassJob>().TryGetRow(j, out var row))
        {
            switch (row.ClassJobCategory.RowId)
            {
                case 30:
                    Test = "Physical DPS Class";
                    break;
                case 31:
                    Test = "Magicic DPS Class";
                    break;
                case 32:
                    Test = "Gathering Class";
                    break;
                case 33:
                    Test = "Crafting Class";
                    break;
                default:
                    Test = "Somehow... not a class??";
                    break;
            }
        }

        return Test;
    }
}
