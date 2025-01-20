using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ImGuiNET;
using Lumina.Excel.Sheets;
using RootofRiches.Scheduler.Tasks;
using System.Numerics;


namespace RootofRiches.Ui.Debugwindow;

internal class MiscDebug
{
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
