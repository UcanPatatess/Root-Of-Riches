using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Component.GUI;
using RootofRiches.Scheduler.Tasks;
using System.Security.Permissions;

namespace RootofRiches.Scheduler
{
    internal static unsafe class SchedulerMain
    {
        internal static bool AreWeTicking;
        internal static bool DoWeTick
        {
            get => AreWeTicking;
            private set => AreWeTicking = value;
        }
        internal static bool EnablePlugin()
        {
            NRaidRun = 1;
            DoWeTick = true;
            TimerStarted = false;
            return true;
        }
        internal static bool DisablePlugin()
        {
            DoWeTick = false;
            P.taskManager.Abort();
            P.navmesh.Stop();
            RunTurnin = false;
            RunNRaid = false;
            FullRun = false;
            JustTurnin = false;
            JustSell = false;
            hasEnqueuedDutyFinder = false;
            P.stopwatch.Restart();
            P.stopwatch.Stop();
            NRaidTask = "idle";
            UpdateCurrentTask("idle");
            ToggleRotation(false);
            return true;
        }

        public static bool RunTurnin = false; // Used for Turnin Toggle
        public static bool RunNRaid = false; // Used for N-Raid Toggle
        public static bool JustTurnin = false; // Used to ONLY Turnin
        public static bool JustSell = false; // Used to ONLY Sell to vendor
        public static bool hasEnqueuedDutyFinder = false; // used for enque throtle flag
        public static string NRaidTask = "idle";
        public static int NRaidRun;
        public static bool FullRun = false;
        public static bool TimerStarted = false;
        public static bool InitatedRotation = false;
        private static uint PreviousArea = 0;
        private static int RaidSelected = 0;

        internal static void Tick()
        {
            if (DoWeTick)
            {
                if (!P.taskManager.IsBusy)
                {
                    if (RunNRaid)
                    {
                        if (IsInZone(A4NMapID) || IsInZone(O3NMapID))
                        {
                            uint ZoneID = CurrentZoneID();
                            IGameObject? gameObject = null;
                            if (TryGetClosestEnemy(out gameObject) && gameObject != null)
                            {
                                P.taskManager.Enqueue(() => NRaidTask = $"Targeted {gameObject?.Name}");
                                P.taskManager.Enqueue(PlayerNotBusy);
                                TaskTarget.Enqueue(gameObject.DataId);
                                ToggleRotation(true);
                                SetBMRange(24);
                                P.taskManager.Enqueue(() => NRaidTask = "Entering Combat");
                                P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.InCombat]);
                                P.taskManager.Enqueue(() => NRaidTask = "Waiting for combat to finish");
                                P.taskManager.Enqueue(() => InitatedRotation = true);   
                                P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.InCombat], "Waiting for combat to end", DConfig);
                                P.taskManager.Enqueue(PlayerNotBusy, "Waiting for Player to be available again", DConfig);
                            }
                            else if (TryGetObjectByDataId(NRaidDict[ZoneID].ListofChest[0], out gameObject))
                            {
                                ToggleRotation(false);
                                P.taskManager.Enqueue(() => NRaidTask = "Gathering your riches");
                                TaskMoveTo.Enqueue(NRaidDict[ZoneID].CenterofChest, "Center Chest", 0.5f);
                                for (int i = 0; i < NRaidDict[ZoneID].ListofChest.Length; i++)
                                {
                                    TaskInteract.Enqueue(NRaidDict[ZoneID].ListofChest[i]);
                                }
                                P.taskManager.Enqueue(LeaveDuty);
                                P.taskManager.Enqueue(() => UpdateStats(ZoneID), "Updating Stats");
                                P.taskManager.Enqueue(() => !IsInZone(ZoneID), "Leaving Normal Raids");
                                P.taskManager.Enqueue(PlayerNotBusy, "Waiting for Player to not be busy");
                                hasEnqueuedDutyFinder = false;
                                P.taskManager.Enqueue(() => NRaidRun = NRaidRun + 1, "Adding a Counter to Normal Raid");
                                InitatedRotation = false;
                                TaskTimer.Enqueue(false, ZoneID);
                                P.taskManager.Enqueue(() => NRaidTask = "idle", "Setting Task to Idle");
                            }
                            else
                            {
                                P.taskManager.EnqueueDelay(100);
                                // just an exit for it to catch/reset in case either of these come false (it shouldn't, but better to have a failsafe)
                            }
                        }
                        else if ((!IsInZone(A4NMapID) || !IsInZone(O3NMapID)) && (NRaidRun <= RunAmount || RunInfinite))
                        {
                            uint ZoneID = 0;
                            if (C.RaidSelected == 0)
                                ZoneID = A4NMapID;
                            else if (C.RaidSelected == 1)
                                ZoneID = O3NMapID;
                            else
                            {
                                PluginLog.Information("Somehow, you managed to get outside the range of Normal Raids Selected. Ending the task");
                                DisablePlugin();
                            }

                            if (C.EnableSubsMain && ARSubsWaitingToBeProcessed())
                            {
                                PLogInfo("Enable Subs on Main is true");
                                PLogInfo("Subs are ready to be processed, running task to send subs");
                                P.taskManager.Enqueue(() => NRaidTask = "Resending Deployables");
                                TaskDoDeployables.Enqueue();
                            }
                            else if (C.EnableReturnInn && Svc.ClientState.TerritoryType != C.InnDataID && !NeedsRepair(C.RepairSlider))
                            {
                                TaskPluginLog.Enqueue("Inn Return is enabled | No Repairs are needed | Not in Inn Already, heading to Inn");
                                P.taskManager.Enqueue(() => NRaidTask = "Heading to the Inn");
                                TaskTeleportInn.Enqueue();
                                TaskUseAethernet.Enqueue();
                                TaskPluginLog.Enqueue("Walking to the Inn NPC");
                                TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnNPCPos, "Walking to Inn Npc");
                                TaskPluginLog.Enqueue("");
                                TaskGetIntoInn.Enqueue();
                                P.taskManager.Enqueue(() => NRaidTask = "idle");
                            }
                            else if (C.EnableRepair && NeedsRepair(C.RepairSlider))
                            {
                                if (C.RepairMode == "Self Repair")
                                {
                                    P.taskManager.Enqueue(() => NRaidTask = "Self Repairing");
                                    TaskSelfRepair.Enqueue();
                                    P.taskManager.Enqueue(() => NRaidTask = "idle");
                                }
                                else if (C.RepairMode == "Repair at NPC")
                                {
                                    if (IsInZone(C.InnDataID))
                                    {
                                        P.taskManager.Enqueue(() => NRaidTask = "Leaving inn to repair");
                                        TaskGetOutInn.Enqueue();
                                        P.taskManager.Enqueue(() => NRaidTask = "Repairing at Inn NPC");
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].RepairNPCPos, "Walking to Inn Npc");
                                        TaskRepairNpc.Enqueue();
                                        P.taskManager.Enqueue(() => NRaidTask = "Heading back Inn");
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnNPCPos, "Walking to Inn Npc");
                                        TaskGetIntoInn.Enqueue();
                                        P.taskManager.Enqueue(() => NRaidTask = "idle");
                                    }
                                    else 
                                    {
                                        P.taskManager.Enqueue(() => NRaidTask = "Teleporting to Inn");
                                        TaskTeleportInn.Enqueue();
                                        TaskUseAethernet.Enqueue();
                                        P.taskManager.Enqueue(() => NRaidTask = "Walking to Inn NPC");
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].RepairNPCPos, "Walking to Inn Npc");
                                        P.taskManager.Enqueue(() => NRaidTask = "Repairing at Inn NPC");
                                        TaskRepairNpc.Enqueue();
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnNPCPos, "Walking to Inn Npc");
                                        P.taskManager.Enqueue(() => NRaidTask = "Heading back Inn");
                                        TaskGetIntoInn.Enqueue();
                                        P.taskManager.Enqueue(() => NRaidTask = "idle");
                                    }
                                }
                            }
                            else if (!ARRetainersWaitingToBeProcessed() && (TryGetAddonByName<AtkUnitBase>("RetainerList", out var RetainerAddon) && IsAddonReady(RetainerAddon)))
                            {
                                TaskGetOut.Enqueue();
                            }
                            else if (C.EnableAutoRetainer && ARRetainersWaitingToBeProcessed() && Svc.ClientState.TerritoryType == C.InnDataID)
                            {
                                P.taskManager.Enqueue(() => NRaidTask = "Resending Retainers");
                                TaskUseRetainer.Enqueue();
                                P.taskManager.Enqueue(() => NRaidTask = "idle");
                            }
                            else if (!IsAddonActive("ContentsFinder") && !hasEnqueuedDutyFinder)
                            {
                                if (!TimerStarted)
                                {
                                    TaskTimer.Enqueue(true);
                                }
                                P.taskManager.Enqueue(() => NRaidTask = "Opening Contents Finder");
                                TaskDutyFinder.Enqueue(NRaidDict[ZoneID].DutyID);
                            }
                            else if (IsAddonActive("ContentsFinder"))
                            {
                                if (!TimerStarted)
                                {
                                    TaskTimer.Enqueue(true);
                                }
                                TaskDutyFinder.Enqueue(NRaidDict[ZoneID].DutyID);
                                P.taskManager.Enqueue(() => NRaidTask = "Launching Correct Duty");
                                TaskSelectCorrectDuty.Enqueue(ZoneID);
                                TaskLaunchDuty.Enqueue();
                                hasEnqueuedDutyFinder = true;
                            }
                        }
                        else
                        {
                            DisablePlugin();
                        }
                    }
                    else if (JustTurnin)
                    {
                        if (!C.ChangeArmory)
                        {
                            TaskChangeArmorySetting.Enqueue();
                            C.ChangeArmory = true;
                        }
                        TaskMergeInv.Enqueue();
                        TaskTurnIn.Enqueue();
                        TaskMergeInv.Enqueue();
                        P.taskManager.Enqueue(() => DisablePlugin());
                    }
                    else if (JustSell)
                    {
                        TaskSellVendor.Enqueue();
                        P.taskManager.Enqueue(() => DisablePlugin());
                    }
                    else if (RunTurnin)
                    {
                        if (TotalExchangeItem != 0 && !C.VendorTurnIn)
                        {
                            TaskGcTurnIn.Enqueue();
                        }
                        else if ((TotalExchangeItem != 0 && C.VendorTurnIn) || (C.SellOilCloth && GetItemCount(10120) > 0))
                        {
                            if (C.TeleportToFC)
                            {
                                TaskFcSell.Enqueue();
                                TaskSellVendor.Enqueue();
                            }
                            else if (TotalExchangeItem != 0 || (C.SellOilCloth && GetItemCount(10120) > 0))
                            {
                                if (DeltascapeTurnInCount > 0 || IsInZone(Rhalgr))
                                {
                                    TaskTeleport.Enqueue(RhalgrAether, Rhalgr);
                                    TaskMountUp.Enqueue();
                                    TaskMoveTo.Enqueue(RandomPointInTriangle(
                                        TurnInDict[Svc.ClientState.TerritoryType].BellPos1,
                                        TurnInDict[Svc.ClientState.TerritoryType].BellPos2,
                                        TurnInDict[Svc.ClientState.TerritoryType].BellPos3), "Summoning Bell", 1);
                                    TaskSellVendor.Enqueue();
                                }
                                else if (GordianTurnInCount > 0 || AlexandrianTurnInCount > 0 || (C.SellOilCloth && GetItemCount(10120) > 0) || IsInZone(Idyllshire))
                                {
                                    TaskTeleport.Enqueue(IdyllshireAether, Idyllshire);
                                    TaskMountUp.Enqueue();
                                    TaskMoveTo.Enqueue(RandomPointInTriangle(
                                        TurnInDict[Svc.ClientState.TerritoryType].BellPos1,
                                        TurnInDict[Svc.ClientState.TerritoryType].BellPos2,
                                        TurnInDict[Svc.ClientState.TerritoryType].BellPos3), "Summoning Bell", 1);
                                    TaskSellVendor.Enqueue();
                                }
                            }
                        }
                        else if (IsThereTradeItem())
                        {
                            if (!C.ChangeArmory)
                            {
                                TaskChangeArmorySetting.Enqueue();
                                C.ChangeArmory = true;
                            }
                            if (DeltascapeTurnInCount > 0)
                            {
                                TaskTeleport.Enqueue(RhalgrAether, Rhalgr);
                                if (IsInZone(Rhalgr))
                                {
                                    TaskMountUp.Enqueue();
                                    TaskMoveTo.Enqueue(RandomPointInTriangle(
                                            TurnInDict[Svc.ClientState.TerritoryType].NpcPos1,
                                            TurnInDict[Svc.ClientState.TerritoryType].NpcPos2,
                                            TurnInDict[Svc.ClientState.TerritoryType].NpcPos3), "Moving to Omega Shop", 1);
                                    TaskMergeInv.Enqueue();
                                    TaskTurnIn.Enqueue();
                                    TaskMergeInv.Enqueue();
                                }
                                P.taskManager.Enqueue(() => PreviousArea = CurrentZoneID());
                            }
                            else if (GordianTurnInCount > 0 || AlexandrianTurnInCount > 0)
                            {
                                TaskTeleport.Enqueue(IdyllshireAether, Idyllshire);
                                if (IsInZone(Idyllshire))
                                {
                                    TaskMountUp.Enqueue();
                                    TaskMoveTo.Enqueue(RandomPointInTriangle(
                                            TurnInDict[Svc.ClientState.TerritoryType].NpcPos1,
                                            TurnInDict[Svc.ClientState.TerritoryType].NpcPos2,
                                            TurnInDict[Svc.ClientState.TerritoryType].NpcPos3), "Moving to Alexander Shop", 1);
                                    TaskMergeInv.Enqueue();
                                    TaskMergeInv.Enqueue();
                                    TaskTurnIn.Enqueue();
                                }
                                P.taskManager.Enqueue(() => PreviousArea = CurrentZoneID()); // i don't know what are we gonna do with these
                            }
                        }
                        else { DisablePlugin(); }
                    }
                }
            }
        }
    }
}
