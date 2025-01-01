using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;
using ECommons.DalamudServices;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameHelpers;
using ECommons.Reflection;
using Serilog;
using Dalamud.Utility;
using ECommons.Throttlers;
using Lumina.Excel.Sheets;
using ECommons;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ECommons.DalamudServices.Legacy;
using System.Runtime.InteropServices;
using ECommons.Automation.NeoTaskManager;
using System.Globalization;
using ECommons.Logging;
using RootofRiches.Scheduler;


namespace RootofRiches;

public static unsafe class Util
{
    public static uint GetClassJobId() => Svc.ClientState.LocalPlayer!.ClassJob.RowId;

    public static string icurrentTask = "";
    public static void UpdateCurrentTask(string task)
    {
        icurrentTask = task;
    }

    internal static bool TryGetObjectByDataId(ulong dataId, out IGameObject? gameObject) => (gameObject = Svc.Objects.OrderBy(GetDistanceToPlayer).FirstOrDefault(x => x.DataId == dataId)) != null;
    internal static unsafe void InteractWithObject(IGameObject? gameObject)
    {
        try
        {
            if (gameObject == null || !gameObject.IsTargetable)
                return;
            var gameObjectPointer = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)gameObject.Address;
            TargetSystem.Instance()->InteractWithObject(gameObjectPointer, false);
        }
        catch (Exception ex)
        {
            Svc.Log.Info($"InteractWithObject: Exception: {ex}");
        }
    }

    internal static bool? TargetByID(IGameObject? gameObject)
    {
        if (!IsOccupied())
        {
            var x = gameObject;
            if (x != null)
            {
                Svc.Targets.SetTarget(x);
                ECommons.Logging.PluginLog.Information($"Setting the target to {x.DataId}");
                return true;
            }
        }
        return false;
    }

    private static float Distance(this Vector3 v, Vector3 v2)
    {
        return new Vector2(v.X - v2.X, v.Z - v2.Z).Length();
    }
    private static unsafe bool IsMoving()
    {
        return AgentMap.Instance()->IsPlayerMoving == 1;
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

    public static void UpdateStats()
    {
        C.SessionStats.TotalA4nRuns = C.SessionStats.TotalA4nRuns + 1;
        C.Stats.TotalA4nRuns = C.Stats.TotalA4nRuns + 1;
    }

    public static unsafe bool CorrectDuty() // first actual function I made that returns a true/false statement in C#... man this was a pain to learn about xD(ice)
    {
        if (TryGetAddonByName<AtkUnitBase>("ContentsFinder", out var addon) && IsAddonReady(addon))
        {
            //var mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNodeSpan[0].Value->NodeText.ToString();
            var mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNode[0].Value->NodeText.ToString();
            var AlexText = "Alexander - The Burden of the Father";
            return mainAddon == AlexText;
        }
        return false;
    }
    public static unsafe uint GetGil() => InventoryManager.Instance()->GetGil();
    internal static unsafe float GetDistanceToPlayer(Vector3 v3) => Vector3.Distance(v3, Player.GameObject->Position);
    internal static unsafe float GetDistanceToPlayer(IGameObject gameObject) => GetDistanceToPlayer(gameObject.Position);
    internal static IGameObject? GetObjectByName(string name) => Svc.Objects.OrderBy(GetDistanceToPlayer).FirstOrDefault(o => o.Name.TextValue.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    public static float GetDistanceToPoint(float x, float y, float z) => Vector3.Distance(Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero, new Vector3(x, y, z));
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
    public static unsafe int GetItemCount(int itemID, bool includeHQ = true)
       => includeHQ ? InventoryManager.Instance()->GetInventoryItemCount((uint)itemID, true) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID + 500_000)
       : InventoryManager.Instance()->GetInventoryItemCount((uint)itemID) + InventoryManager.Instance()->GetInventoryItemCount((uint)itemID + 500_000);
    public static bool PluginInstalled(string name)
    {
        return DalamudReflector.TryGetDalamudPlugin(name, out _, false, true);
    }
    public static GameObject* LPlayer() => GameObjectManager.Instance()->Objects.IndexSorted[0].Value;

    public static Vector3 PlayerPosition()
    {
        var player = LPlayer();
        return player != null ? player->Position : default;
    }

    public static bool PlayerInRange(Vector3 dest, float dist)
    {
        var d = dest - PlayerPosition();
        return d.X * d.X + d.Z * d.Z <= dist * dist;
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
    public static byte GetPlayerGC() => UIState.Instance()->PlayerState.GrandCompany;
    public static bool PlayerNotBusy()
    {
        return Player.Available
               && Player.Object.CastActionId == 0
               && !IsOccupied()
               && !Svc.Condition[ConditionFlag.Jumping]
               && Player.Object.IsTargetable;
    }
    internal static bool GenericThrottle => FrameThrottler.Throttle("RootofRichesGenericThrottle", 20);
    public static uint CurrentTerritory() => Svc.ClientState.TerritoryType;
    public static bool IsInZone(int zoneID) => Svc.ClientState.TerritoryType == zoneID;
    public static bool IsAddonActive(string AddonName) // bunu kullan
    {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName(AddonName);
        return addon != null && addon->IsVisible && addon->IsReady;
    }
    public static void UpdateDict()
    {
        for (var i = 0; i < RaidItemIDs.Length; i++)
        {
            var itemID = RaidItemIDs[i];
            VendorSellDict[itemID].CurrentItemCount = GetItemCount(itemID);
        }
    }
    public static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);

    private static readonly AbandonDuty ExitDuty = Marshal.GetDelegateForFunctionPointer<AbandonDuty>(Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 43 28 41 B2 01"));

    private delegate void AbandonDuty(bool a1);

    public static void LeaveDuty() => ExitDuty(false);

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

    public static bool EnableNormalRaidFarm()
    {
        return PluginInstalled("BossMod")
               || (PluginInstalled(AltBossMod) && PluginInstalled("WrathCombo"));
    }

    public static void ToggleRotation(bool enable)
    {
        if (enable)
        {
            if (PluginInstalled("WrathCombo"))
            {
                RunCommand("wrath auto on");
                if (PluginInstalled("BossMod"))
                {
                    P.bossmod.AddPreset("ROR Passive", Resources.BMRotations.rootPassive);
                    P.bossmod.SetPreset("ROR Passive");
                    P.bossmod.SetRange(2.5f);
                    RunCommand("vbm ai on");
                }
                if (PluginInstalled(AltBossMod))
                {
                    RunCommand("vbm maxdistancetarget 2.6");
                    RunCommand("vbm followtarget");
                    RunCommand("vbm follow combat on");
                    RunCommand("vbmai on");
                }
            }
            else
            {
                RunCommand("vbm ai on");
                if (PluginInstalled("BossMod"))
                {
                    P.bossmod.AddPreset("RoR Boss", Resources.BMRotations.rootBoss);
                    P.bossmod.SetPreset("RoR Boss");
                }
            }
        }
        else if (!enable)
        {
            if (PluginInstalled("WrathCombo"))
            {
                RunCommand("wrath auto off");
                P.bossmod.DisablePresets();
                RunCommand("vbm ai off");
                if (PluginInstalled(AltBossMod))
                {
                    RunCommand("vbmai off");
                }
            }
            else
            {
                RunCommand("vbm ai off");
                P.bossmod.DisablePresets();
            }
        }
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

    public static void SetBMRange(float range)
    {
        if (GetRoleByNumber() == "Melee")
            P.bossmod.SetRange(3);
        else if (GetRoleByNumber() == "Caster")
            P.bossmod.SetRange(range);
        else
            P.bossmod.SetRange(2.5f);
    }
}
