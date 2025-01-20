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
using RootofRiches.IPC;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


namespace RootofRiches;

public static unsafe class Util
{
    public static uint GetClassJobId() => Svc.ClientState.LocalPlayer!.ClassJob.RowId;

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
                    ECommons.Logging.PluginLog.Information($"Setting the target to {x.DataId}");
                }
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

    public static void UpdateStats(uint zoneID)
    {
        if (zoneID == A4NMapID)
        {
            C.SessionStats.TotalA4nRuns = C.SessionStats.TotalA4nRuns + 1;
            C.Stats.TotalA4nRuns = C.Stats.TotalA4nRuns + 1;
        }
        else if (zoneID == O3NMapID)
        {
            C.SessionStats.TotalO3nRuns = C.SessionStats.TotalO3nRuns + 1;
            C.Stats.TotalO3nRuns = C.Stats.TotalO3nRuns + 1;
        }
    }

    public static unsafe bool CorrectDuty(uint ZoneID) // first actual function I made that returns a true/false statement in C#... man this was a pain to learn about xD(ice)
    {
        if (TryGetAddonByName<AtkUnitBase>("ContentsFinder", out var addon) && IsAddonReady(addon))
        {
            if (ZoneID == A4NMapID)
            {
                //var mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNodeSpan[0].Value->NodeText.ToString();
                var AlexText = "Alexander - The Burden of the Father";
                if (Svc.Data.Language == ClientLanguage.Japanese)
                {
                    AlexText = "機工城アレキサンダー：起動編4";
                }
                else if (Svc.Data.Language == ClientLanguage.English)
                {
                    AlexText = "Alexander - The Burden of the Father";
                }
                else if (Svc.Data.Language == ClientLanguage.German)
                {
                    AlexText = "Alexander - Last des Vaters";
                }
                else if (Svc.Data.Language == ClientLanguage.French)
                {
                    AlexText = "Alexander - Le Fardeau du Père";
                }
                var mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNode[0].Value->NodeText.ToString();
                return mainAddon == AlexText;
            }
            else if (ZoneID == O3NMapID)
            {
                //var mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNodeSpan[0].Value->NodeText.ToString();
                var OmegaText = "Deltascape V3.0";
                if (Svc.Data.Language == ClientLanguage.Japanese)
                {
                    OmegaText = "次元の狭間オメガ：デルタ編3";
                }
                else if (Svc.Data.Language == ClientLanguage.English)
                {
                    OmegaText = "Deltascape V3.0";
                }
                else if (Svc.Data.Language == ClientLanguage.German)
                {
                    OmegaText = "Deltametrie 3.0";
                }
                else if (Svc.Data.Language == ClientLanguage.French)
                {
                    OmegaText = "Deltastice v3.0";
                }
                var mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNode[0].Value->NodeText.ToString();
                return mainAddon == OmegaText;
            }
        }
        return false;
    }

    public static uint GetCurrentWorld() => Svc.ClientState.LocalPlayer?.CurrentWorld.RowId ?? 0;
    public static uint GetHomeWorld() => Svc.ClientState.LocalPlayer?.HomeWorld.RowId ?? 0;

    public static bool IsBetweenAreas => (Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51]);
    public static unsafe uint GetGil() => InventoryManager.Instance()->GetGil();
    internal static unsafe float GetDistanceToPlayer(Vector3 v3) => Vector3.Distance(v3, Player.GameObject->Position);
    internal static unsafe float GetDistanceToPlayer(IGameObject gameObject) => GetDistanceToPlayer(gameObject.Position);
    internal static IGameObject? GetObjectByName(string name) => Svc.Objects.OrderBy(GetDistanceToPlayer).FirstOrDefault(o => o.Name.TextValue.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    public static float GetDistanceToPoint(float x, float y, float z) => Vector3.Distance(Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero, new Vector3(x, y, z));
    public static float GetDistanceToVectorPoint(Vector3 location) => Vector3.Distance(Svc.ClientState.LocalPlayer?.Position ?? Vector3.Zero, location);
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
               && Player.Object.IsTargetable
               && !Player.IsAnimationLocked;
    }
    internal static bool GenericThrottle => FrameThrottler.Throttle("RootofRichesGenericThrottle", 20);
    public static uint CurrentZoneID() => Svc.ClientState.TerritoryType;
    public static bool IsInZone(uint zoneID) => Svc.ClientState.TerritoryType == zoneID;
    public static bool CurrentlyInnInn()
    {
        return innZones.Contains(CurrentZoneID());
    }

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
        return P.navmesh.Installed
               && (P.bossmod.Installed || (PluginInstalled(AltBossMod) && WrathIPC.IsEnabled));
    }
    public static bool EnableTurnIn()
    {
        if (C.TeleportToFC)
        {
            return P.navmesh.Installed
            && P.autoRetainer.Installed
            && P.deliveroo.Installed
            && P.lifestream.Installed
            ;
        }
        else
        {
            return P.navmesh.Installed
            && P.autoRetainer.Installed
            && P.deliveroo.Installed
            ;
        }
    }

    public static bool IsOnHomeWorld()
    {
        if (C.VendorTurnIn || C.SellOilCloth)
        {
            return GetCurrentWorld() == GetHomeWorld();
        }
        else
        {
            return true;
        }
    }
    public static void ToggleRotation(bool enable)
    {
        if (enable)
        {
            float range = 3;
            int altrange = 2;
            var j = Player.JobId;
            if (Svc.Data.GetExcelSheet<ClassJob>().TryGetRow(j, out var row))
            {
                switch (row.ClassJobCategory.RowId)
                {
                    case 30:
                        // Physical DPS Class;
                        range = 2.8f;
                        altrange = 2;
                        break;
                    case 31:
                        // Magicic DPS Class
                        range = 24.0f;
                        altrange = 24;
                        break;
                    default:
                        range = 2.8f;
                        break;
                }
            }

            if (PluginInstalled("WrathCombo"))
            {
                EnableWrathAuto();

                if (PluginInstalled("BossMod")) // If you have Veyns BossMod and Wrath Installed at the same time
                {
                    P.bossmod.AddPreset("ROR Passive", Resources.BMRotations.rootPassive);
                    P.bossmod.SetPreset("ROR Passive");
                    P.bossmod.SetRange(range);
                    RunCommand("vbm ai on");
                }
                if (PluginInstalled(AltBossMod)) // If you have... alternative bossmod installed & also Wrath
                {
                    RunCommand($"vbmai maxdistancetarget {altrange}");
                    RunCommand("vbmai on");
                    RunCommand("vbmai followtarget on");
                    RunCommand("vbmai followcombat on");
                }
            }
            else if (P.bossmod.Installed) // If you have ONLY Veyn's BossMod
            {
                RunCommand("vbm ai on");
                P.bossmod.AddPreset("RoR Boss", Resources.BMRotations.rootBoss);
                P.bossmod.SetPreset("RoR Boss");
                P.bossmod.SetRange(range);
            }
        }
        else if (!enable)
        {
            if (PluginInstalled("WrathCombo"))
            {
                //RunCommand("wrath auto off");
                P.bossmod.DisablePresets();
                ReleaseWrathControl();
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

    #region Wrath
    public static void EnableWrathAuto()
    {
        if (!WrathIPC.IsEnabled) return;
        try
        {
            var lease = (Guid)WrathIPC.CurrentLease!;
            // enable Wrath Combo Auto-Rotation
            
            WrathIPC.SetAutoRotationState(lease, true);
            // make sure the job is ready for Auto-Rotation
            
            WrathIPC.SetCurrentJobAutoRotationReady(lease);
            // if the job is ready, all the user's settings are locked
            // if the job is not ready, it turns on the job's simple modes, or if those don't
            // exist, it turns on the job's advanced modes with all options enabled
        }
        catch (Exception e)
        {
            PluginLog.Error("Unknown Wrath IPC error," +
                            "probably inability to register a lease." +
                            "\n" + e.Message);
        }
    }

    public static void EnableWrathAutoAndConfigureIt()
    {
        if (!WrathIPC.IsEnabled) return;
        try
        {
            var lease = (Guid)WrathIPC.CurrentLease!;
            WrathIPC.SetAutoRotationState(lease, true);
            WrathIPC.SetCurrentJobAutoRotationReady(lease);
            WrathIPC.SetAutoRotationConfigState(lease,
                WrathIPC.AutoRotationConfigOption.InCombatOnly, false);
            WrathIPC.SetAutoRotationConfigState(lease,
                WrathIPC.AutoRotationConfigOption.AutoRez, true);
            WrathIPC.SetAutoRotationConfigState(lease,
                WrathIPC.AutoRotationConfigOption.SingleTargetHPP, 60);
        }
        catch (Exception e)
        {
            PluginLog.Error("Unknown Wrath IPC error," +
                            "probably inability to register a lease." +
                            "\n" + e.Message);
        }
    }

    public static void ReleaseWrathControl()
    {
        if (!WrathIPC.IsEnabled) return;
        try
        {
            WrathIPC.ReleaseControl((Guid)WrathIPC.CurrentLease!);
            WrathIPC.RoRLease = null;
        }
        catch (Exception e)
        {
            PluginLog.Error("Unknown Wrath IPC error," +
                            "probably inability to register a lease." +
                            "\n" + e.Message);
        }
    }
    #endregion

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

    public static unsafe void OpenDuty(uint DutyID)
    {
        AgentContentsFinder.Instance()->OpenRegularDuty(DutyID);//relocated
    }

    public static unsafe string JournalDutyText() // first actual function I made that returns a true/false statement in C#... man this was a pain to learn about xD(ice)
    {
        string mainAddon = "";

        if (TryGetAddonByName<AtkUnitBase>("ContentsFinder", out var addon) && IsAddonReady(addon))
        {
            mainAddon = ((AddonContentsFinder*)addon)->SelectedDutyTextNode[0].Value->NodeText.ToString();
        }
        else
        {
            mainAddon = "Addon isn't ready";
        }

        return mainAddon;
    }

    #region AutoRetainer
    public static int ToUnixTimestamp(this DateTime value) => (int)Math.Truncate(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    public static bool ARAvailableRetainersCurrentCharacter() => P.autoRetainer.AreAnyRetainersAvailableForCurrentChara(); // old check gonna use the below now
    private static unsafe ParallelQuery<ulong> GetAllEnabledCharacters() => P.autoRetainerApi.GetRegisteredCharacters().AsParallel().Where(c => P.autoRetainerApi.GetOfflineCharacterData(c).Enabled);

    public static unsafe bool ARRetainersWaitingToBeProcessed(bool allCharacters = false)
    {
        return !allCharacters
            ? P.autoRetainerApi.GetOfflineCharacterData(Svc.ClientState.LocalContentId).RetainerData.AsParallel().Any(x => x.HasVenture && x.VentureEndsAt <= DateTime.Now.ToUnixTimestamp())
            : GetAllEnabledCharacters().Any(character => P.autoRetainerApi.GetOfflineCharacterData(character).RetainerData.Any(x => x.HasVenture && x.VentureEndsAt <= DateTime.Now.ToUnixTimestamp()));
    }

    public static unsafe bool ARSubsWaitingToBeProcessed(bool allCharacters = false)
    {
        return !allCharacters
            ? P.autoRetainerApi.GetOfflineCharacterData(Svc.ClientState.LocalContentId).OfflineSubmarineData.AsParallel().Any(x => x.ReturnTime <= DateTime.Now.ToUnixTimestamp())
            : GetAllEnabledCharacters().Any(c => P.autoRetainerApi.GetOfflineCharacterData(c).OfflineSubmarineData.Any(x => x.ReturnTime <= DateTime.Now.ToUnixTimestamp()));
    }

    #endregion
}
