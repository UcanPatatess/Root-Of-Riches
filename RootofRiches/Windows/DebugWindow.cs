using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Utility.Table;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using RootofRiches.Scheduler.Handlers;
using RootofRiches.Scheduler.Tasks;
using ImGuiNET;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using static FFXIVClientStructs.FFXIV.Client.UI.RaptureAtkHistory.Delegates;

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
    private string pluginName = "none";
    private string commandInput = "";
    private bool rSenabled = false;
    private static ulong InnDataID = InnDict[C.InnDataID].RepairNPC;
    public override void Draw()
    {
        if (ImGui.BeginTabBar("##Debug Tabs"))
        {
            if (ImGui.BeginTabItem("Global Turnin debug"))
            {
                ImGui.Text($"General Information");
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
                    string clipboardText = $"{XPos}f, {YPos}f, {ZPos}f";
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
                {
                    TaskGetOut.Enqueue();
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
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Targeting Debug"))
            {
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

                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Time Test"))
            {
                TimeTestUi();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void A4NGuiUi()
    {
        ImGui.Text($"A4n Duty is selected {CorrectDuty()}");
        ImGui.Text("Open A4NMapID in the duty selection");
        if (ImGui.Button("A4NMapID"))
        {
            TaskDutyFinder.Enqueue();
        }
        if (ImGui.Button("SelectDuty"))
        {
            TaskSelectCorrectDuty.Enqueue();
        }
        if (ImGui.Button("TaskLaunchDuty"))
        {
            TaskLaunchDuty.Enqueue();
        }
        if (ImGui.Button("TaskContentWidnowConfirm"))
        {
            TaskContentWidnowConfirm.Enqueue();
        }
        if (ImGui.Button("GetInA4n"))
        {
            TaskDutyFinder.Enqueue();
            TaskSelectCorrectDuty.Enqueue();
            TaskLaunchDuty.Enqueue();
            TaskContentWidnowConfirm.Enqueue();
        }
        if (ImGui.Button("Chest Task"))
        {
            TaskMoveTo.Enqueue(A4NChestCenter, "Center Chest", 0.5f);
            TaskOpenChest.Enqueue(A4NChest1);
            TaskOpenChest.Enqueue(A4NChest2);
            TaskOpenChest.Enqueue(A4NChest3);
        }
        if (ImGui.Button("Full Inside A4N"))
        {
            TaskA4N.Enqueue();
        }

        if (ImGui.Button("Full A4N Loop"))
        {
            TaskDutyFinder.Enqueue();
            TaskSelectCorrectDuty.Enqueue();
            TaskLaunchDuty.Enqueue();
            TaskContentWidnowConfirm.Enqueue();
            TaskA4N.Enqueue();
        }

        ImGui.Text($"Are we available/not busy? = {PlayerNotBusy()}");
        ImGui.SameLine();
        ImGui.Text($"PluginInstalled : {PluginInstalled("BurningDowntheHouse")}");

        IGameObject? gameObject = null;
        if (TryGetObjectByDataId(LeftForeleg, out gameObject))
        {
            ImGui.Text("Left leg is Targetable");
        }
        else if (TryGetObjectByDataId(A4NChest1, out gameObject))
        {
            ImGui.Text("Chest are Targetable");
        }
        else
        {
            ImGui.Text("Nothing is possible to target!");
        }
    }

    private void TimeTestUi()
    {
        /*
        bool timeReset;
        TimeSpan currentTime = P.stopwatch.Elapsed;
        string currentTimeF = currentTime.ToString(@"mm\:ss\.fff");
        TimeSpan totalTime = C.TotalRunTime;
        string totalTimeF = totalTime.ToString(@"hh\:mm\:ss");
        TimeSpan fastestTime = C.fastestRun;
        string fastestTimeF = fastestTime.ToString(@"mm\:ss\.fff");

        ImGui.Text($"Time Elapsed is: {currentTimeF}");
        ImGui.Text($"Current total loop time is: {totalTimeF}");
        ImGui.Text($"Fastest time is: {fastestTimeF}");
        bool isCtrlHeld = ImGui.GetIO().KeyCtrl;
        using (var _ = ImRaii.PushStyle(ImGuiStyleVar.Alpha, 0.5f, !ImGui.GetIO().KeyCtrl))
            timeReset = ImGui.Button("RESET STATS", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y)) && ImGui.GetIO().KeyCtrl;

        if (timeReset)
        {
            C.TotalRunTime = TimeSpan.Zero;
            C.fastestRun = TimeSpan.MaxValue;
        }
        */
    }
}
