using ECommons.Automation.NeoTaskManager;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using GrandCompany = ECommons.ExcelServices.GrandCompany;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;


namespace RootofRiches.Scheduler.Tasks
{
    internal unsafe static class TaskGcTurnIn
    {
        internal static void Enqueue(GrandCompany? companyNullable = null)
        {
            TaskPluginLog.Enqueue("Heading to your Grand Company");
            P.taskManager.Enqueue(() => UpdateCurrentTask("Moving to GC"));
            companyNullable = Player.GrandCompany;
            if (companyNullable == GrandCompany.Unemployed)
            {
                DuoLog.Error("Player is unemployed and can not do Gc TurnIn Stopping Turn in");
                SchedulerMain.DisablePlugin();
            }
            var company = companyNullable.Value;
            var pos = CompanyNPCPoints[company];
            if (Player.GrandCompany == company && InventoryManager.Instance()->GetInventoryItemCount(CompanyItem[company]) > 0)
            {
                P.taskManager.Enqueue(() =>
                {
                    if (Player.IsAnimationLocked) return false;
                    if (EzThrottler.Throttle("GCUseTicket", 1000))
                    {
                        AgentInventoryContext.Instance()->UseItem(CompanyItem[company]);
                    }
                    if (Player.Object.IsCasting) return true;
                    return false;
                });
                P.taskManager.Enqueue(()=>!PlayerNotBusy());
                P.taskManager.Enqueue(PlayerNotBusy);
                TaskMoveTo.Enqueue(pos, "Moving to Gc", 2);
            }
            else 
            {
                TaskTeleportGc.Enqueue();
                if (Player.GrandCompany == GrandCompany.Maelstrom)
                {
                    TaskUseAethernet.Enqueue(true);
                }
                TaskMoveTo.Enqueue(pos, "Moving to Gc", 2);
            }
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.Enqueue(Deliveroo, configuration: DConfig);
            P.taskManager.Enqueue(() => UpdateCurrentTask("Turning in at GC"));
            P.taskManager.EnqueueDelay(1000);
            P.taskManager.Enqueue(() => UpdateCurrentTask("idle"));
        }
        private static TaskManagerConfiguration DConfig => new(timeLimitMS: 10 * 60 * 1000, abortOnTimeout: false);
        private static void Deliveroo() => P.taskManager.InsertMulti([new(() => Svc.Commands.ProcessCommand("/deliveroo enable")), new(() => P.deliveroo.IsTurnInRunning()), new(() => !P.deliveroo.IsTurnInRunning(), DConfig)]);
    

    }
}
