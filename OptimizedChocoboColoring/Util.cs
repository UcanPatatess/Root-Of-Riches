using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Colors;
using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.Reflection;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.Sheets;
using OptimizedChocoboColoring.IPC;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


namespace OptimizedChocoboColoring;

public static unsafe class Util
{
    #region Player Information

    public static uint GetCurrentWorld() => Svc.ClientState.LocalPlayer?.CurrentWorld.RowId ?? 0;
    public static uint GetHomeWorld() => Svc.ClientState.LocalPlayer?.HomeWorld.RowId ?? 0;
    public static bool IsBetweenAreas => (Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51]);
    public static unsafe uint GetGil() => InventoryManager.Instance()->GetGil();
    internal static unsafe float GetDistanceToPlayer(Vector3 v3) => Vector3.Distance(v3, Player.GameObject->Position);
    internal static unsafe float GetDistanceToPlayer(IGameObject gameObject) => GetDistanceToPlayer(gameObject.Position);
    public static GameObject* LPlayer() => GameObjectManager.Instance()->Objects.IndexSorted[0].Value;
    public static uint GetClassJobId() => Svc.ClientState.LocalPlayer!.ClassJob.RowId;
    public static uint CurrentZoneID() => Svc.ClientState.TerritoryType;
    public static bool IsInZone(uint zoneID) => Svc.ClientState.TerritoryType == zoneID;
    public static byte GetPlayerGC() => UIState.Instance()->PlayerState.GrandCompany;

    private static unsafe uint GetSpellActionId(uint actionId) => ActionManager.Instance()->GetAdjustedActionId(actionId);
    public static unsafe float GetRecastTime(uint actionId) => ActionManager.Instance()->GetRecastTime(ActionType.Action, GetSpellActionId(actionId));
    public static unsafe float GetRealRecastTime(uint actionId) => ActionManager.Instance()->GetRecastTime(ActionType.Action, actionId);
    public static unsafe float GetRecastTimeElapsed(uint actionId) => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Action, GetSpellActionId(actionId));
    public static unsafe float GetRealRecastTimeElapsed(uint actionId) => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Action, actionId);
    public static float GetSpellCooldown(uint actionId) => Math.Abs(GetRecastTime(GetSpellActionId(actionId)) - GetRecastTimeElapsed(GetSpellActionId(actionId)));
    public static unsafe void ExecuteAction(uint actionID) => ActionManager.Instance()->UseAction(ActionType.Action, actionID);
    public static unsafe void ExecuteGeneralAction(uint actionID) => ActionManager.Instance()->UseAction(ActionType.GeneralAction, actionID);
    public static unsafe void ExecuteAbility(uint actionID) => ActionManager.Instance()->UseAction(ActionType.Ability, actionID);

    private static unsafe bool IsMoving()
    {
        return AgentMap.Instance()->IsPlayerMoving == 1;
    }
    public static Vector3 PlayerPosition()
    {
        var player = LPlayer();
        return player != null ? player->Position : default;
    }
    public static float GetPlayerRawXPos()
    {
        return Svc.ClientState.LocalPlayer!.Position.X;
    }
    public static float GetPlayerRawYPos()
    {
        return Svc.ClientState.LocalPlayer!.Position.Y;
    }
    public static float GetPlayerRawZPos()
    {
        return Svc.ClientState.LocalPlayer!.Position.Z;
    }
    public static unsafe int GetInventoryFreeSlotCount()
    {
        InventoryType[] types = [InventoryType.Inventory1, InventoryType.Inventory2, InventoryType.Inventory3, InventoryType.Inventory4];
        var slots = 0;
        foreach (var x in types)
        {
            var cont = InventoryManager.Instance()->GetInventoryContainer(x);
            for (var i = 0; i < cont->Size; i++)
                if (cont->Items[i].ItemId == 0)
                    slots++;
        }
        return slots;
    }
    public static unsafe bool NeedsRepair(float below = 0)
    {
        var im = InventoryManager.Instance();
        if (im == null)
        {
            Svc.Log.Error("InventoryManager was null");
            return false;
        }

        var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
        if (equipped == null)
        {
            Svc.Log.Error("InventoryContainer was null");
            return false;
        }

        if (equipped->Loaded == 0)
        {
            Svc.Log.Error($"InventoryContainer is not loaded");
            return false;
        }

        for (var i = 0; i < equipped->Size; i++)
        {
            var item = equipped->GetInventorySlot(i);
            if (item == null)
                continue;

            var itemCondition = Convert.ToInt32(Convert.ToDouble(item->Condition) / 30000.0 * 100.0);

            if (itemCondition <= below)
                return true;
        }

        return false;
    }
    public static bool PlayerNotBusy()
    {
        return Player.Available
               && Player.Object.CastActionId == 0
               && !IsOccupied()
               && !Svc.Condition[ConditionFlag.Jumping]
               && Player.Object.IsTargetable
               && !Player.IsAnimationLocked;
    }

    public static string GetRoleByNumber()
    {
        uint number = GetClassJobId();
        switch (number)
        {
            // Tanks
            case 19: // PLD
            case 21: // WAR
            case 32: // DRK
            case 37: // GNB
            // Melees
            case 20: // MNK
            case 22: // DRG
            case 30: // NIN
            case 39: // RPR
            case 41: // VPR
            // Range
            case 23: // BRD
            case 31: // MCH
            case 38: // DNC
                return "Melee";
            // Healer
            case 24: // WHM
            case 28: // SCH
            case 33: // AST
            case 40: // SGE
            // Caster
            case 25: // BLM
            case 27: // SMN
            case 35: // RDM
            case 42: // PCT
                return "Caster";

            default:
                return "Unknown";
        }
    }

    #endregion

    #region Distance Functions

    public static float GetDistanceToPoint(float x, float y, float z) => Vector3.Distance(Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero, new Vector3(x, y, z));
    public static float GetDistanceToVectorPoint(Vector3 location) => Vector3.Distance(Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero, location);
    public static bool PlayerInRange(Vector3 dest, float dist)
    {
        var d = dest - PlayerPosition();
        return d.X * d.X + d.Z * d.Z <= dist * dist;
    }
    private static float Distance(this Vector3 v, Vector3 v2)
    {
        return new Vector2(v.X - v2.X, v.Z - v2.Z).Length();
    }

    #endregion


    public static string icurrentTask = "idle";
    public static void UpdateCurrentTask(string task)
    {
        icurrentTask = task;
    }

    internal static bool TryGetObjectByDataId(ulong dataId, out IGameObject? gameObject) => (gameObject = Svc.Objects.OrderBy(GetDistanceToPlayer).FirstOrDefault(x => x.DataId == dataId)) != null;
    internal static bool TryGetClosestEnemy(out IGameObject? gameObject) => (gameObject = Svc.Objects.OrderBy(GetDistanceToPlayer).FirstOrDefault(x => x.IsTargetable == true && x.IsHostile() == true)) != null;
    internal static unsafe void InteractWithObject(IGameObject? gameObject)
    {
        try
        {
            if (gameObject == null || !gameObject.IsTargetable)
                return;
            var gameObjectPointer = (GameObject*)gameObject.Address;
            TargetSystem.Instance()->InteractWithObject(gameObjectPointer, false);
        }
        catch (Exception ex)
        {
            Svc.Log.Info($"InteractWithObject: Exception: {ex}");
        }
    }

    internal static bool? TargetByID(IGameObject? gameObject)
    {
        var x = gameObject;
        if (Svc.Targets.Target != null && Svc.Targets.Target.DataId == x.DataId)
            return true;

        if (!IsOccupied())
        {
            if (x != null)
            {
                if (EzThrottler.Throttle($"Throttle Targeting {x.DataId}"))
                {
                    Svc.Targets.SetTarget(x);
                    PluginLog.Information($"Setting the target to {x.DataId}");
                }
            }
        }
        return false;
    }

    internal unsafe static bool? MoveToCombat(Vector3 targetPosition, float toleranceDistance = 3f)
    {
        if (targetPosition.Distance(Player.GameObject->Position) <= toleranceDistance || Svc.Condition[ConditionFlag.InCombat])
        {
            P.navmesh.Stop();
            return true;
        }
        if (!P.navmesh.IsReady()) { UpdateCurrentTask("Waiting Navmesh"); return false; }
        if (P.navmesh.PathfindInProgress() || P.navmesh.IsRunning() || IsMoving()) return false;

        P.navmesh.PathfindAndMoveTo(targetPosition, false);
        P.navmesh.SetAlignCamera(true);
        return false;
    }


    internal static IGameObject? GetObjectByName(string name) => Svc.Objects.OrderBy(GetDistanceToPlayer).FirstOrDefault(o => o.Name.TextValue.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    public static unsafe int GetItemCount(int itemID, bool includeHQ = true)
       => includeHQ ? InventoryManager.Instance()->GetInventoryItemCount((uint)itemID, true) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID + 500_000)
       : InventoryManager.Instance()->GetInventoryItemCount((uint)itemID) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID + 500_000);
    public static bool PluginInstalled(string name)
    {
        return DalamudReflector.TryGetDalamudPlugin(name, out _, false, true);
    }

    public static bool DidAmountChange(int arg, int argg)
    {
        if (arg == argg)
            return false;
        else
            return true;
    }
    public static int GetFreeSlotsInContainer(int container)
    {
        var inv = InventoryManager.Instance();
        var cont = inv->GetInventoryContainer((InventoryType)container);
        var slots = 0;
        for (var i = 0; i < cont->Size; i++)
            if (cont->Items[i].ItemId == 0)
                slots++;
        return slots;
    }
    internal static bool GenericThrottle => FrameThrottler.Throttle("OptimizedChocoboColoringGenericThrottle", 20);

    public static bool IsAddonActive(string AddonName) // bunu kullan
    {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
        return addon != null && addon->IsVisible && addon->IsReady;
    }
    public static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);

    public static void RunCommand(string command)
    {
        ECommons.Automation.Chat.Instance.ExecuteCommand($"/{command}");
    }

    public static void PLogInfo(string message)
    {
        PluginLog.Information(message);
    }
    public static void PLogDebug(string message)
    {
        PluginLog.Debug(message);
    }

    public static void FancyCheckmark(bool enabled)
    {
        if (!enabled)
        {
            FontAwesome.Print(ImGuiColors.DalamudRed, FontAwesome.Cross);
        }
        else if (enabled)
        {
            FontAwesome.Print(ImGuiColors.HealerGreen, FontAwesome.Check);
        }
    }
    public static void FancyPluginUiString(bool PluginInstalled,string Text,string Url)
    {
        FancyCheckmark(PluginInstalled);
        ImGui.SameLine();
        if (ImGui.Selectable(Text))
        {
            // Copy the repo URL to the clipboard
            ImGui.SetClipboardText(Url);
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Click to copy repo URL");
            ImGui.EndTooltip();
        }
    }
    internal static Random random = new Random();
    public static Vector3 RandomPointInTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float r1 = (float)random.NextDouble();
        float r2 = (float)random.NextDouble();

        // Ensure the point is inside the triangle
        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        // Calculate the random point
        return (1 - r1 - r2) * p1 + r1 * p2 + r2 * p3;
    }
}
