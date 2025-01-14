using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation.NeoTaskManager;
using ECommons.Automation.NeoTaskManager.Tasks;
using ECommons.DalamudServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using RootofRiches.Scheduler.Handlers;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace RootofRiches.Scheduler.Tasks
{
    internal static class TaskDoDeployables
    {
        //2005274	voyage control panel	0	voyage control panels	0	0	1	0	0
        internal static string PanelName => Svc.Data.GetExcelSheet<EObjName>().GetRow(2005274).Singular.ExtractText();
        internal static string[] AdditionalChambersEntrance =>
[
    Svc.Data.GetExcelSheet<EObjName>().GetRow(2004353).Singular.ExtractText(),
        Regex.Replace(Svc.Data.GetExcelSheet<EObjName>().GetRow(2004353).Singular.ExtractText(), @"\[.*?\]", "")
];
        internal static IGameObject GetWorkshopEntrance() => Svc.Objects.FirstOrDefault(x => x.IsTargetable && x.Name.ToString().EqualsIgnoreCaseAny(AdditionalChambersEntrance));
        internal static bool TryGetNearestVoyagePanel(out IGameObject obj)
        {
            //Data ID: 2007820
            if (Svc.Objects.TryGetFirst(x => x.Name.ToString().EqualsIgnoreCaseAny(PanelName) && x.IsTargetable, out var o))
            {
                obj = o;
                return true;
            }
            obj = default;
            return false;
        }
        internal static void Enqueue()
        {
            P.taskManager.BeginStack();
            GoFc();
            P.taskManager.Enqueue(GoFcAndWorkshopInOrder);
            P.taskManager.Enqueue(InteractWithNearestPanel);
            P.taskManager.Enqueue(() => !ARSubsWaitingToBeProcessed());
            P.taskManager.Enqueue(() => P.autoRetainer.IsBusy());
            P.taskManager.Enqueue(() => !P.autoRetainer.IsBusy());
            P.taskManager.Enqueue(PlayerNotBusy);
            P.taskManager.InsertStack();
        }
        internal static bool? WaitUntilLeavingZone()
        {
            return !ResidentalAreas.List.Contains(Svc.ClientState.TerritoryType);
        }
        internal static void GoIntoWorkshop()
        {
            if (P.lifestream.CanMoveToWorkshop())
            {
                if (GetWorkshopEntrance() != null)
                {
                    P.taskManager.InsertMulti
                    ([
                        new(P.lifestream.MoveToWorkshop),
                        new(() => !P.lifestream.IsBusy()),
                        NeoTasks.ApproachObjectViaAutomove(GetWorkshopEntrance, 4f),
                        NeoTasks.InteractWithObject(GetWorkshopEntrance),
                        new(() => GenericHandlers.FireCallback("SelectString",true,0)),
                        new(() => Workshops.Contains(Svc.ClientState.TerritoryType), "Wait Until entered workshop"),
                        NeoTasks.WaitForScreenAndPlayer(),
                    ]);
                }
            }
        }
        internal static bool InteractWithNearestPanel()
        {
            if (TryGetNearestVoyagePanel(out var obj))
                if (obj != null)
                {
                    P.taskManager.BeginStack();
                    TaskMoveTo.Enqueue(obj.Position, "Moving to Panel", 4);
                    P.taskManager.Enqueue(() => TargetByID(obj), "Targeting Object");
                    P.taskManager.Enqueue(() => InteractWithObject(obj));
                    P.taskManager.Enqueue(() => !P.lifestream.IsBusy());
                    P.taskManager.InsertStack();
                    return true;
                }
                return false;
        }
        internal static bool GoFcAndWorkshopInOrder()
        {
            if (Player.Territory.EqualsAny(Workshops)) return true;
            var data = P.lifestream.GetHousePathData(Player.CID);
            var info = P.lifestream.GetCurrentPlotInfo();

            if (data.FC != null && data.FC.PathToEntrance.Count > 0)
            {
                return Process();
            }


            return false;

            bool Process(){
                var pathData = data.FC;
                if (info != null
                && info.Value.Plot == pathData.Plot
                && info.Value.Ward == pathData.Ward
                && info.Value.Kind == pathData.ResidentialDistrict)
                {
                    PLogDebug("Found path Processing");
                    P.taskManager.InsertMulti
                    (
                        NeoTasks.WaitForNotOccupied(),
                        new(() => P.taskManager.Enqueue(() => Workshops.Contains(Svc.ClientState.TerritoryType), "Wait Until entered House")),
                        NeoTasks.WaitForScreenAndPlayer(),
                        new(GoIntoWorkshop)
                    );
                    return true; //already here
                }
                return false;
            }
        }
        private static void GoFc() => P.taskManager.InsertMulti([new(P.lifestream.TeleportToFC), new(() => P.lifestream.IsBusy()), new(() => !P.lifestream.IsBusy(), LSConfig)]);
        private static TaskManagerConfiguration LSConfig => new(timeLimitMS: 2 * 60 * 1000);
    }
}
