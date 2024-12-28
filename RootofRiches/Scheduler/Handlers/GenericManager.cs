using FFXIVClientStructs.FFXIV.Component.GUI;
using ECommons.Automation;
using ECommons.Automation.LegacyTaskManager;
using FFXIVClientStructs.FFXIV.Client.UI;
using ECommons.UIHelpers.AddonMasterImplementations;
using ECommons.DalamudServices;


namespace GlobalTurnIn.Scheduler.Handlers
{
    internal static unsafe class GenericManager
    {
        internal static TaskManager taskManager = new();
        static TaskManager TaskManager => taskManager;
        private static List<int> SlotsFilled { get; set; } = new();

        private static int PreviousGil = (int)GetGil(); 
        private static void CheckGill()
        {
            int currentGill = (int)GetGil();

            if (currentGill > PreviousGil) 
            {
                int earnedGill = currentGill - PreviousGil;
                C.UpdateStats(Stats =>
                {
                    Stats.GillEarned += earnedGill;
                });
                C.Save();
            }
            PreviousGil = currentGill;
        }
        private static bool? ConfirmOrAbort(AddonRequest* addon)
        {
            if (addon->HandOverButton != null && addon->HandOverButton->IsEnabled)
            {
                new AddonMaster.Request((IntPtr)addon).HandOver();
                return true;
            }
            return false;
        }
        private static bool? TryClickItem(AddonRequest* addon, int i)
        {
            if (SlotsFilled.Contains(i)) return true;

            var contextMenu = (AtkUnitBase*)Svc.GameGui.GetAddonByName("ContextIconMenu", 1);

            if (contextMenu is null || !contextMenu->IsVisible)
            {
                var slot = i - 1;
                var unk = (44 * i) + (i - 1);

                Callback.Fire(&addon->AtkUnitBase, false, 2, slot, 0, 0);

                return false;
            }
            else
            {
                Callback.Fire(contextMenu, false, 0, 0, 1021003, 0, 0);
                Svc.Log.Debug($"Filled slot {i}");
                SlotsFilled.Add(i);
                return true;
            }
        }
        internal static void Tick()
        {
            if (SchedulerMain.DoWeTick)
            {
                if (SchedulerMain.RunTurnin)
                {
                    //To update gill amounth
                    CheckGill();
                    //by Taurenkey https://github.com/PunishXIV/PandorasBox/blob/24a4352f5b01751767c7ca7f1d4b48369be98711/PandorasBox/Features/UI/AutoSelectTurnin.cs
                    if (TryGetAddonByName<AddonRequest>("Request", out var addon3))
                    {
                        for (var i = 1; i <= addon3->EntryCount; i++)
                        {
                            if (SlotsFilled.Contains(addon3->EntryCount)) ConfirmOrAbort(addon3);
                            if (SlotsFilled.Contains(i)) return;
                            var val = i;
                            TaskManager.DelayNext($"ClickTurnin{val}", 10);
                            TaskManager.Enqueue(() => TryClickItem(addon3, val));
                        }
                    }
                    else
                    {
                        SlotsFilled.Clear();
                        TaskManager.Abort();
                    }
                }
            }

        }
    }
}
