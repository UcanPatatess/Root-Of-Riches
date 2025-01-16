using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using ECommons.Logging;
using RootofRiches.Scheduler.Tasks;

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
            hasEnqueuedDutyFinder = false;
            P.stopwatch.Restart();
            P.stopwatch.Stop();
            FullRun = false;
            NRaidTask = "idle";
            UpdateCurrentTask("idle");
            ToggleRotation(false);
            return true;
        }

        public static bool RunTurnin = false; // Used for Turnin Toggle
        public static bool RunNRaid = false; // Used for N-Raid Toggle
        public static bool hasEnqueuedDutyFinder = false; // used for enque throtle flag
        public static string NRaidTask = "idle";
        public static int NRaidRun;
        public static bool FullRun = false;
        public static bool TimerStarted = false;
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
                            if (!Svc.Condition[ConditionFlag.InCombat])
                            {
                                uint ZoneID = CurrentZoneID();
                                IGameObject? gameObject = null;
                                if (TryGetObjectByDataId(NRaidDict[ZoneID].ListofChest[0], out gameObject))
                                {
                                    if (GetRoleByNumber() == "Caster")
                                    {

                                    }
                                    ToggleRotation(false);
                                    P.taskManager.Enqueue(() => NRaidTask = "Gathering your riches");
                                    TaskMoveTo.Enqueue(NRaidDict[ZoneID].CenterofChest, "Center Chest", 0.5f);
                                    for (int i = 0; i < NRaidDict[ZoneID].ListofChest.Length; i++)
                                    {
                                        TaskInteract.Enqueue(NRaidDict[ZoneID].ListofChest[i]);
                                    }
                                    P.taskManager.Enqueue(LeaveDuty);
                                    P.taskManager.Enqueue(() => UpdateStats(ZoneID));
                                    P.taskManager.Enqueue(() => !IsInZone(ZoneID), "Leaving Normal Raids");
                                    P.taskManager.Enqueue(PlayerNotBusy);
                                    hasEnqueuedDutyFinder = false;
                                    P.taskManager.Enqueue(() => NRaidRun = NRaidRun + 1);
                                    if (TimerStarted)
                                        TaskTimer.Enqueue(false, ZoneID);
                                    P.taskManager.Enqueue(() => NRaidTask = "idle");
                                }
                                else if (TryGetObjectByDataId(NRaidDict[ZoneID].BossID, out gameObject))
                                {
                                    P.taskManager.Enqueue(() => NRaidTask = $"Targeted {gameObject?.Name}");
                                    // Left Leg is targetable... which means you aren't in combat/you haven't initiated it yet
                                    P.taskManager.Enqueue(PlayerNotBusy);
                                    TaskTarget.Enqueue(NRaidDict[ZoneID].BossID);
                                    ToggleRotation(true);
                                    SetBMRange(24);
                                    if (GetRoleByNumber() == "Tank")
                                    {
                                        P.taskManager.Enqueue(() => NRaidTask = "Entering Combat");
                                        P.taskManager.Enqueue(() => MoveToCombat(RightForeLegPos), "Moving to Combat");
                                    }
                                    else
                                    {
                                        P.taskManager.Enqueue(() => NRaidTask = "Entering Combat");
                                        P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.InCombat]);

                                    }
                                    P.taskManager.Enqueue(() => NRaidTask = "Waiting for combat to finish");
                                    P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.InCombat], "Waiting for combat to end", DConfig);
                                    P.taskManager.Enqueue(PlayerNotBusy, "Waiting for Cutscene", DConfig);
                                }
                                else
                                {
                                    P.taskManager.EnqueueDelay(100);
                                    // just an exit for it to catch/reset in case either of these come false (it shouldn't, but better to have a failsafe)
                                }
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
                            else if (C.EnableAutoRetainer && ARRetainersWaitingToBeProcessed() && Svc.ClientState.TerritoryType == C.InnDataID)
                            {
                                P.taskManager.Enqueue(() => NRaidTask = "Resending Retainers");
                                TaskUseAutoRetainer.Enqueue();
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
