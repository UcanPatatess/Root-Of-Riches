using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using RootofRiches.Scheduler.Tasks;
using ImGuiNET;
using System.Globalization;
using System.Numerics;
using RootofRiches.IPC;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System.Security.Policy;
using Dalamud.Plugin.Services;

namespace RootofRiches.Windows;


internal class DebugWindow : Window
{
    public new static readonly string WindowName = "RoR Debug";
    public DebugWindow() : 
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
    private string addonName = "default";
    private static float XPos = 0;
    private static float YPos = 0;
    private static float ZPos = 0;
    private static int Tolerance = 0;
    private static float TargetXPos = 0;
    private static float TargetYPos = 0;
    private static float TargetZPos = 0;
    private static uint TargetDataID = 0;
    private static string TargetDataIDString = TargetDataID.ToString();
    private string pluginName = "none";
    private string commandInput = "";
    private bool rSenabled = false;
    private static ulong InnDataID = InnDict[C.InnDataID].RepairNPC;
    private string CurrentTask()
    {
        if (P.taskManager.NumQueuedTasks > 0 && P.taskManager.CurrentTask != null)
        {
            return P.taskManager.CurrentTask.Name?.ToString() ?? "None";
        }
        return "None";
    }
    private int inputBox = 0;
    private uint testDutyID = 0;
    private IGameObject? gameObject = null;

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##Debug Tabs"))
        {
            if (ImGui.BeginTabItem("Global Turnin debug"))
            {
                ImGui.Text($"General Information");
                ImGui.Text($"Current Task: {CurrentTask()}");
                ImGui.Text($"TerritoryID: " + Svc.ClientState.TerritoryType);
                ImGui.Text($"Target: " + Svc.Targets.Target);
                ImGui.InputText("##Addon Visible", ref addonName, 100);
                ImGui.SameLine();
                ImGui.Text($"Addon Visible: " + IsAddonActive(addonName));
                ImGui.Text($"Navmesh information");
                ImGui.Text($"PlayerPos: " + PlayerPosition());
                ImGui.Text($"Navmesh BuildProgress :" + P.navmesh.BuildProgress());//working ipc
                ImGui.Text("X:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(75);
                if (ImGui.InputFloat("##X Position", ref XPos))
                {
                    XPos = (float)Math.Round(XPos, 2);
                }
                ImGui.SameLine();
                ImGui.Text("Y:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(75);
                if (ImGui.InputFloat("##Y Position", ref YPos))
                {
                    YPos = (float)Math.Round(YPos, 2);
                }
                ImGui.SameLine();
                ImGui.Text("Z:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(75);
                if (ImGui.InputFloat("##Z Position", ref ZPos))
                {
                    ZPos = (float)Math.Round(ZPos, 2);
                }
                ImGui.SameLine();
                ImGui.Text("Tolerance:");
                ImGui.SameLine();
                ImGui.SetNextItemWidth(100);
                ImGui.InputInt("##Tolerance", ref Tolerance);
                if (ImGui.Button("Set to Current"))
                {
                    XPos = (float)Math.Round(GetPlayerRawXPos(), 2);
                    YPos = (float)Math.Round(GetPlayerRawYPos(), 2);
                    ZPos = (float)Math.Round(GetPlayerRawZPos(), 2);
                }
                ImGui.SameLine();
                if (ImGui.Button("Copy to clipboard"))
                {
                    string clipboardText = string.Format(CultureInfo.InvariantCulture, "{0:F2}f, {1:F2}f, {2:F2}f", XPos, YPos, ZPos);
                    ImGui.SetClipboardText(clipboardText);
                }
                if (ImGui.Button("Vnav Moveto!"))
                {
                    P.taskManager.Enqueue(() => TaskMoveTo.Enqueue(new Vector3(XPos, YPos, ZPos), "Interact string", Tolerance));
                    ECommons.Logging.InternalLog.Information("Firing off Vnav Moveto");
                }
                if (ImGui.Button("MergeInv"))
                {
                    TaskMergeInv.Enqueue();
                }
                ImGui.SameLine();
                if(ImGui.Button("RepairNpc"))
                    TaskRepairNpc.Enqueue();
                ImGui.SameLine();
                if(ImGui.Button("Abort Tasks"))
                    P.taskManager.Abort();
                if (ImGui.Button("Self Repair"))
                    TaskSelfRepair.Enqueue();
                ImGui.SameLine();
                if (ImGui.Button("Teleport Inn"))
                    TaskTeleportInn.Enqueue();
                ImGui.SameLine();
                if (ImGui.Button("TaskGetIntoInn"))
                    TaskGetIntoInn.Enqueue();
                if (ImGui.Button("Task Escape"))
                    TaskGetOut.Enqueue();
                if (ImGui.Button("EnableMulti"))
                {
                    TaskUseAutoRetainer.Enqueue();
                }
                
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Normal Raid Farm"))
            {
                int zoneID = Svc.ClientState.TerritoryType;
                ImGui.Text($"Current Zone ID is: {zoneID}");
                if (ImGui.Button("Copy Current Zone ID"))
                {
                    ImGui.SetClipboardText($"{zoneID}");
                }
                
                ImGui.InputText("Plugin Name Check", ref pluginName, 100);
                if (PluginInstalled(pluginName))
                {
                    ImGui.Text($"Plugin: {pluginName} is installed");
                }
                else
                {
                    ImGui.Text($"Plugin: {pluginName} is not visible");
                }
                ImGui.InputText("Command", ref commandInput, 500);
                if (ImGui.Button("Run Command"))
                {
                    RunCommand(commandInput);
                }
                if (ImGui.Button("Add Passive Preset"))
                {
                    P.bossmod.RefreshPreset("RoR Passive", Resources.BMRotations.rootPassive);
                    ImGui.SetClipboardText($"{Resources.BMRotations.rootPassive}");
                }
                if (ImGui.Button(rSenabled ? "Disable" : "Enable"))
                {
                    if (rSenabled)
                    {
                        ToggleRotation(false);
                        rSenabled = false;
                    }
                    else
                    {
                        ToggleRotation(true);
                        rSenabled = true;
                    }
                }

                if (ARAvailableRetainersCurrentCharacter())
                    ImGui.Text("Available Retainers on this Character");
                else if (!ARAvailableRetainersCurrentCharacter())
                    ImGui.Text("No Retainers Available");
                else
                    ImGui.Text("Retainer Service not available");
                if (ImGui.Button("Simple Enable Wrath"))
                {
                    P.taskManager.Enqueue(() => EnableWrathAuto());
                }
                if (ImGui.Button("Enable Advanced Wrath Rotation"))
                {
                    P.taskManager.Enqueue(() => EnableWrathAutoAndConfigureIt());
                }
                if (ImGui.Button("Release Wrath Control"))
                {
                    P.taskManager.Enqueue(() => ReleaseWrathControl());
                }
                if (ImGui.InputInt("##Test Duty Selected", ref inputBox))
                {
                    if (inputBox >= 0)
                    {
                        testDutyID = (uint)inputBox;
                    }
                }
                ImGui.Text($"TestDutyID = {testDutyID}");
                if (ImGui.Button("Duty Finder"))
                {
                    OpenDuty(testDutyID);
                }
                ImGui.Text($"{JournalDutyText()}");
                if(ImGui.Button("Copy Journal Text"))
                {
                    ImGui.SetClipboardText($"{JournalDutyText()}");
                }
            }
            if (ImGui.BeginTabItem("Targeting Debug"))
            {
                if (TryGetObjectByDataId(LeftForeleg, out gameObject))
                {
                    var x = gameObject;
                    if (x != null)
                    {
                        ImGui.Text($"Manip's Left Leg should appear to the right: {x.Name}");
                    }
                }
                if (Svc.Targets?.Target != null)
                {
                    TargetXPos = (float)Math.Round(Svc.Targets.Target.Position.X, 2);
                    TargetYPos = (float)Math.Round(Svc.Targets.Target.Position.Y, 2);
                    TargetZPos = (float)Math.Round(Svc.Targets.Target.Position.Z, 2);
                    // Get the GameObjectId and display it in the ImGui.Text box
                    ImGui.Text($"Name: {Svc.Targets.Target.Name}");
                    ImGui.Text($"GameObjectId: {Svc.Targets.Target.GameObjectId}");
                    ImGui.Text($"DataID: {Svc.Targets.Target.DataId}");
                    if (ImGui.Button("Copy DataID to clipboard"))
                    {
                        ImGui.SetClipboardText($"{Svc.Targets.Target.DataId}");
                    }
                    if (ImGui.Button("Copy GameObjectID to clipboard"))
                    {
                        ImGui.SetClipboardText($"{Svc.Targets.Target.GameObjectId}");
                    }
                    ImGui.Text($"Target Pos: X: {TargetXPos}, Y: {TargetYPos}, Z: {TargetZPos}");
                    if (ImGui.Button("Copy Target XYZ"))
                    {
                        ImGui.SetClipboardText($"{TargetXPos.ToString("0.00", CultureInfo.InvariantCulture)}f, " +
                   $"{TargetYPos.ToString("0.00", CultureInfo.InvariantCulture)}f, " +
                   $"{TargetZPos.ToString("0.00", CultureInfo.InvariantCulture)}f");
                    }
                }
                else
                {
                    // Optionally display a message if no target is selected
                    ImGui.Text("No target selected.");
                }

                if (ImGui.Button("Summoning Bell Interact"))
                {
                    TaskInteract.Enqueue(SummoningBell);
                }

                ImGui.Text($"Is in Inn? {CurrentlyInnInn()}");
                if (ImGui.InputText("##Target Dataid", ref TargetDataIDString, 100))
                {
                    if (uint.TryParse(TargetDataIDString, out uint newTargetDataID))
                    {
                        TargetDataID = newTargetDataID;
                    }
                }
                
                ImGui.SameLine();
                if (ImGui.Button("Target the DataID"))
                {
                    TaskTarget.Enqueue(TargetDataID);
                }
                ImGui.EndTabItem();
            }
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
