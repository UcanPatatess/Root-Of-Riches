using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Dalamud.Game.ClientState.Conditions;
using System.Numerics;
using ECommons.Automation.NeoTaskManager;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;
using System.Data;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskA4N
    {
        internal static void Enqueue()
        {
            // some things to do:
            // Could easily have BM manage the targeting for us/moving AI. 
            // This would reduce some of the opt codes for us
            // Other option is for us to just use the good old tried and true of moving ourselves to close the gap
            // Add switch statements like you did for queueing
            // Have an 
            // -> Entrance Mode (Haven't started combat yet)
            // -> In Combat
            // -> Exit Combat -> Open Chest
            // -> Leave Duty

            P.taskManager.Enqueue(() => IsInZone(A4NMapID));
            P.taskManager.Enqueue(PlayerNotBusy);
            TaskTarget.Enqueue(RightForeleg);
            P.taskManager.Enqueue(() => MoveToCombat(RightForeLegPos), "Moving to Combat");
            P.taskManager.Enqueue(() => !Svc.Condition[ConditionFlag.InCombat], "Waiting for combat to end", DConfig);
            // if wrath installed, enable wrath
            // also if Wrath Rotation, enable the distance module in BM 
            // elseif !wrath rotation, enable BM rotation + Ai

            TaskMoveTo.Enqueue(new Vector3(-0.08f, 10.6f, -6.46f), "Center Chest", 0.5f);
            TaskOpenChest.Enqueue(A4NChest1);
            TaskOpenChest.Enqueue(A4NChest2);
            TaskOpenChest.Enqueue(A4NChest3);
            P.taskManager.Enqueue(LeaveDuty);
            P.taskManager.Enqueue(UpdateStats);
            P.taskManager.Enqueue(() => !IsInZone(A4NMapID));
        }

        private static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);
    }
}
