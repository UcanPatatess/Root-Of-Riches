using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using RootofRiches.Scheduler.Tasks;
using ImGuiNET;
using System.Numerics;

namespace RootofRiches.Windows;


internal class DebugWindowOld : Window
{
    public new static readonly string WindowName = "RoR Debug###RORDebugWindow";
    public DebugWindowOld() : 
        base(WindowName)
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

    private static ulong InnDataID = InnDict[C.InnDataID].RepairNPC;
    private string CurrentTask()
    {
        if (P.taskManager.NumQueuedTasks > 0 && P.taskManager.CurrentTask != null)
        {
            return P.taskManager.CurrentTask.Name?.ToString() ?? "None";
        }
        return "None";
    }
    private IGameObject? gameObject = null;

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##Debug Tabs"))
        {
            if (ImGui.BeginTabItem("Time Test"))
            {
                TimeTestUi();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Misc Testing"))
            {
                if (ImGui.Button("Teleport to Gridania"))
                {
                    TaskTeleport.Enqueue(GridaniaAether, Gridania);
                }
            }
            ImGui.EndTabBar();
        }
    }

    private void TimeTestUi()
    {
        ImGui.Text($"Task is: {CurrentTask()}");
    }
}
