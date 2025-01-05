using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.DalamudServices;
using RootofRiches.Scheduler.Tasks;
using System.Numerics;

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
            return true;
        }
        internal static bool DisablePlugin()
        {
            DoWeTick = false;
            P.taskManager.Abort();
            P.navmesh.Stop();
            RunTurnin = false;
            RunA4N = false;
            hasEnqueuedDutyFinder = false;
            P.stopwatch.Restart();
            P.stopwatch.Stop();
            FullRun = false;
            A4NTask = "idle";
            UpdateCurrentTask("idle");
            return true;
        }

        public static bool RunTurnin = false; // Used for Turnin Toggle
        public static bool RunA4N = false; // Used for N-Raid Toggle
        public static bool hasEnqueuedDutyFinder = false; // used for enque throtle flag
        public static string A4NTask = "idle";
        public static int NRaidRun;
        public static bool FullRun = false;
        private static int PreviousArea = 0;

        internal static void Tick()
        {
            if (DoWeTick)
            {
                if (!P.taskManager.IsBusy)
                {
                    if (RunA4N)
                    {
                        if (IsInZone(A4NMapID))
                        {
                            if (!Svc.Condition[ConditionFlag.InCombat])
                            {
                                IGameObject? gameObject = null;
                                if (TryGetObjectByDataId(A4NChest1, out gameObject))
                                {
                                    if (GetRoleByNumber() == "Caster")
                                    {

                                    }
                                    ToggleRotation(false);
                                    P.taskManager.Enqueue(() => A4NTask = "Gathering your riches");
                                    TaskMoveTo.Enqueue(new Vector3(-0.08f, 10.6f, -6.46f), "Center Chest", 0.5f);
                                    TaskOpenChest.Enqueue(A4NChest1);
                                    TaskOpenChest.Enqueue(A4NChest2);
                                    TaskOpenChest.Enqueue(A4NChest3);
                                    P.taskManager.Enqueue(LeaveDuty);
                                    P.taskManager.Enqueue(UpdateStats);
                                    P.taskManager.Enqueue(() => !IsInZone(A4NMapID), "Leaving A4N");
                                    hasEnqueuedDutyFinder = false;
                                    P.taskManager.Enqueue(() => NRaidRun = NRaidRun + 1);
                                    TaskTimer.Enqueue(false);
                                    P.taskManager.Enqueue(() => A4NTask = "idle");
                                }
                                else if (TryGetObjectByDataId(LeftForeleg, out gameObject) || TryGetObjectByDataId(RightForeleg, out gameObject))
                                {
                                    P.taskManager.Enqueue(() => A4NTask = "Targeted Left Foreleg");
                                    // Left Leg is targetable... which means you aren't in combat/you haven't initiated it yet
                                    P.taskManager.Enqueue(PlayerNotBusy);
                                    TaskTarget.Enqueue(RightForeleg);
                                    ToggleRotation(true);
                                    SetBMRange(24);
                                    if (GetRoleByNumber() == "Tank")
                                    {
                                        P.taskManager.Enqueue(() => A4NTask = "Entering Combat");
                                        P.taskManager.Enqueue(() => MoveToCombat(RightForeLegPos), "Moving to Combat");
                                    }
                                    else
                                    {
                                        P.taskManager.Enqueue(() => A4NTask = "Entering Combat");
                                        P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.InCombat]);

                                    }
                                    // If Left Leg is Targetable, enable the following

                                    // BM ai (to move to the target while in combat)
                                    // if Wrath installed, enable wrath + BM ai Limited
                                    // if RSR installed, 
                                    P.taskManager.Enqueue(() => A4NTask = "Waiting for combat to finish");
                                    P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.InCombat], "Waiting for combat to end", DConfig);
                                    P.taskManager.Enqueue(() => Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent], "Waiting for Cutscene", DConfig);
                                    P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent], "Waiting for Cutscene", DConfig);
                                }
                                else
                                {
                                    P.taskManager.EnqueueDelay(100);
                                    // just an exit for it to catch/reset in case either of these come false (it shouldn't, but better to have a failsafe)
                                }
                            }
                        }
                        else if (!IsInZone(A4NMapID) && (NRaidRun <= RunAmount || RunInfinite))
                        {
                            if (C.EnableReturnInn && Svc.ClientState.TerritoryType != C.InnDataID && !NeedsRepair(C.RepairSlider))
                            {
                                P.taskManager.Enqueue(() => A4NTask = "Heading to the Inn");
                                TaskTeleportInn.Enqueue();
                                TaskUseAethernet.Enqueue();
                                TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnNPCPos, "Walking to Inn Npc");
                                TaskGetIntoInn.Enqueue();
                                P.taskManager.Enqueue(() => A4NTask = "idle");
                            }
                            else if (C.EnableRepair && NeedsRepair(C.RepairSlider))
                            {
                                if (C.RepairMode == "Self Repair")
                                {
                                    P.taskManager.Enqueue(() => A4NTask = "Self Repairing");
                                    TaskSelfRepair.Enqueue();
                                    P.taskManager.Enqueue(() => A4NTask = "idle");
                                }
                                else if (C.RepairMode == "Repair at NPC")
                                {
                                    if (IsInZone(C.InnDataID))
                                    {
                                        P.taskManager.Enqueue(() => A4NTask = "Leaving inn to repair");
                                        TaskGetOutInn.Enqueue();
                                        P.taskManager.Enqueue(() => A4NTask = "Repairing at Inn NPC");
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].RepairNPCPos, "Walking to Inn Npc");
                                        TaskRepairNpc.Enqueue();
                                        P.taskManager.Enqueue(() => A4NTask = "Heading back Inn");
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnNPCPos, "Walking to Inn Npc");
                                        TaskGetIntoInn.Enqueue();
                                        P.taskManager.Enqueue(() => A4NTask = "idle");
                                    }
                                    else 
                                    {
                                        P.taskManager.Enqueue(() => A4NTask = "Teleporting to Inn");
                                        TaskTeleportInn.Enqueue();
                                        TaskUseAethernet.Enqueue();
                                        P.taskManager.Enqueue(() => A4NTask = "Walking to Inn NPC");
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].RepairNPCPos, "Walking to Inn Npc");
                                        P.taskManager.Enqueue(() => A4NTask = "Repairing at Inn NPC");
                                        TaskRepairNpc.Enqueue();
                                        TaskMoveTo.Enqueue(InnDict[C.InnDataID].InnNPCPos, "Walking to Inn Npc");
                                        P.taskManager.Enqueue(() => A4NTask = "Heading back Inn");
                                        TaskGetIntoInn.Enqueue();
                                        P.taskManager.Enqueue(() => A4NTask = "idle");
                                    }
                                }
                            }
                            else if (!IsAddonActive("ContentsFinder") && !hasEnqueuedDutyFinder)
                            {
                                TaskTimer.Enqueue(true);
                                P.taskManager.Enqueue(() => A4NTask = "Opening Contents Finder");
                                TaskDutyFinder.Enqueue();
                            }
                            else if (IsAddonActive("ContentsFinder"))
                            {
                                P.taskManager.Enqueue(() => A4NTask = "Launching Correct Duty");
                                TaskSelectCorrectDuty.Enqueue();
                                TaskLaunchDuty.Enqueue();
                                hasEnqueuedDutyFinder = true;
                            }
                            else if (IsAddonActive("ContentsFinderConfirm"))
                            {
                                P.taskManager.Enqueue(() => A4NTask = "Confirming the duty");
                                TaskContentWidnowConfirm.Enqueue();
                                P.taskManager.Enqueue(() => A4NTask = "idle");
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
                                if (IsInZone(Rhalgr) || DeltascapeTurnInCount > 0)
                                {
                                    TaskTeleportTo.Enqueue(Rhalgr);
                                    TaskMountUp.Enqueue();
                                    TaskMoveTo.Enqueue(new Vector3(-57.27f, 0f, 48.57f), "Summoning Bell");
                                    TaskSellVendor.Enqueue();
                                }
                                else if (IsInZone(Idyllshire) || (GordianTurnInCount > 0 || AlexandrianTurnInCount > 0))
                                {
                                    TaskTeleportTo.Enqueue(Idyllshire);
                                    TaskMountUp.Enqueue();
                                    TaskMoveTo.Enqueue(new Vector3(34, 208, -51), "Summoning Bell");
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
                                //logic is added but it needs to be tested
                                TaskTeleportTo.Enqueue(Rhalgr);
                                TaskMountUp.Enqueue();
                                TaskMoveTo.Enqueue(new Vector3(125.88f, 0.68f, 40.67f), "Omega Shop NPC", 1);
                                TaskMergeInv.Enqueue();
                                TaskTurnIn.Enqueue();
                                P.taskManager.Enqueue(() => PreviousArea = CurrentZoneID());
                            }
                            else if (GordianTurnInCount > 0 || AlexandrianTurnInCount > 0)
                            {
                                TaskTeleportTo.Enqueue(Idyllshire);
                                TaskMountUp.Enqueue();
                                TaskMoveTo.Enqueue(new Vector3(-19, 211, -36), "Alexander Shop NPC", 1);
                                TaskMergeInv.Enqueue();
                                TaskTurnIn.Enqueue();
                                P.taskManager.Enqueue(() => PreviousArea = CurrentZoneID());
                            }
                        }
                        else { DisablePlugin(); }
                    }
                }
            }
        }
    }
}
