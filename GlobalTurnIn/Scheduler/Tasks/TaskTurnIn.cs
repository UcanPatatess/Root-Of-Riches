using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using GlobalTurnIn.Scheduler.Handlers;
using Lumina.Excel.Sheets;
using Serilog;
using ECommons.Automation;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal static class TaskTurnIn
    {
        public static bool AreWeTurningIn=false;
        internal unsafe static void Enqueue()
        {
            P.taskManager.Enqueue(() => UpdateCurrentTask("TaskTurnIn"));
            
        int? lastShopType = null;
        int? LastIconShopType = null;

        int[,] TableName = null!;
            if (Svc.ClientState.TerritoryType == 478)
                TableName = SabinaTable;
            if (Svc.ClientState.TerritoryType == 635)
                TableName = GelfradusTable;

            int SlotINV = GetInventoryFreeSlotCount();
            SlotINV = SlotINV - C.MaxArmoryFreeSlot;
            bool isItemPurchasedFromArmory = false;
            int lastArmoryType = -1;
            int armoryExchaneAmount =0;
            bool SelectIconStirngBool = true;
            UpdateDict();

            for (int i = 0; i < TableName.GetLength(0); i++)
            {
                int shopType = TableName[i, 0];
                int itemType = TableName[i, 1];
                int itemTypeBuy = TableName[i, 2];
                int gearItem = TableName[i, 3];
                int pcallValue = TableName[i, 4];
                int iconShopType = TableName[i, 5];

                int ItemAmount = VendorSellDict[itemType].CurrentItemCount;
                int GearAmount = GetItemCount(gearItem);
                int CanExchange = (int)Math.Floor((double)ItemAmount / itemTypeBuy);
                
                int ArmoryType = 0;
                if (ItemIdArmoryTable.TryGetValue(gearItem, out int category))
                    ArmoryType = category;

                int SlotArmoryINV = GetFreeSlotsInContainer(ArmoryType);

                if (ArmoryType != lastArmoryType)
                {
                    isItemPurchasedFromArmory = false; // Reset the flag for the new armory
                    lastArmoryType = ArmoryType; // Update the last armory type
                    armoryExchaneAmount += 1;
                }
                if (isItemPurchasedFromArmory || armoryExchaneAmount > 8)
                {
                    SlotArmoryINV = 0; // Don't consider armory slots if we've already purchased
                }
                if (CanExchange > 0 && GearAmount == 0 && (SlotINV > 0 || (SlotArmoryINV > 0 && C.MaxArmory))) // >o< looks like a emoji lol 
                {
                    if (shopType != lastShopType)
                    {
                        P.taskManager.Enqueue(CloseShop);
                        OpenShopMenu(iconShopType, shopType, SelectIconStirngBool);
                        lastShopType = shopType;
                        SelectIconStirngBool = false;
                    }
                    if (SlotArmoryINV != 0 && C.MaxArmory)
                    {
                        if (CanExchange < SlotArmoryINV)
                        {
                            Exchange(gearItem, pcallValue, CanExchange);
                            VendorSellDict[itemType].CurrentItemCount = ItemAmount - (CanExchange * itemTypeBuy);
                            isItemPurchasedFromArmory = true;
                        }
                        else
                        {
                            Exchange(gearItem, pcallValue, SlotArmoryINV);
                            VendorSellDict[itemType].CurrentItemCount = ItemAmount - (SlotArmoryINV * itemTypeBuy);
                            isItemPurchasedFromArmory = true;
                        }
                        if (LastIconShopType != null && iconShopType != LastIconShopType)
                        {
                            P.taskManager.Enqueue(CloseShop);
                        }
                        continue;
                    }
                    if (C.MaxItem)
                    {
                        if (CanExchange < SlotINV)
                        {
                            Exchange(gearItem, pcallValue, CanExchange);
                            VendorSellDict[itemType].CurrentItemCount = ItemAmount - (CanExchange * itemTypeBuy);
                            SlotINV -= CanExchange;
                        }
                        else
                        {
                            Exchange(gearItem, pcallValue, SlotINV);
                            VendorSellDict[itemType].CurrentItemCount = ItemAmount - (SlotINV * itemTypeBuy);
                            SlotINV -= 127;
                        }
                    }
                    else
                    {
                        Exchange(gearItem, pcallValue, 1);
                        VendorSellDict[itemType].CurrentItemCount = ItemAmount - (1* itemTypeBuy);
                        SlotINV -= 1;
                    }
                    if (LastIconShopType != null && iconShopType != LastIconShopType)
                    {
                        P.taskManager.Enqueue(CloseShop);
                    }
                }
            }
            P.taskManager.Enqueue(CloseShop);
            P.taskManager.EnqueueDelay(300);
            TaskGetOut.Enqueue();
            //P.taskManager.Enqueue(() => GenericHandlers.FireCallback("SelectString", true, -1));
        }
        internal unsafe static bool? CloseShop() //dükkanı kapattı biraz daha bakılması lazım
        {
            var agent = AgentShop.Instance();
            if (agent == null || agent->EventReceiver == null)
            {
                return true;
            }
            AtkValue res = default, arg = default;
            var proxy = (ShopEventHandler.AgentProxy*)agent->EventReceiver;
            proxy->Handler->CancelInteraction();
            arg.SetInt(-1);
            agent->ReceiveEvent(&res, &arg, 1, 0);
            return false;
        }
        internal static bool? TargetNpc()
        {
            string NpcName = string.Empty;
            if (Svc.ClientState.TerritoryType == 478) //Idyllshire
                NpcName = "Sabina";
            if (Svc.ClientState.TerritoryType == 635)//Rhalgr
                NpcName = "Gelfradus";
            Svc.Log.Debug("TargetNpc" + NpcName);

            var target = GetObjectByName(NpcName);
            if (target != null)
            {
                if (EzThrottler.Throttle("TargetNpc", 100))
                    Svc.Targets.Target = target;
                return true;
            }
            return false;
        }

        internal unsafe static bool? TargetInteract()
        {
            var target = Svc.Targets.Target;
            if (target != null)
            {
                if (IsAddonActive("SelectString") || IsAddonActive("SelectIconString") || IsAddonActive("ShopExchangeItem"))
                    return true;

                if (EzThrottler.Throttle("TargetInteract", 100))
                    TargetSystem.Instance()->InteractWithObject(target.Struct(), false);

                return false;
            }
            return false;
        }
        internal static void OpenShopMenu(int SelectIconString, int SelectString,bool armory = true)
        {
            Svc.Log.Debug("OpenShopMenu" + " " + SelectIconString + " " + SelectString);
            P.taskManager.EnqueueDelay(100);
            P.taskManager.Enqueue(TargetNpc);
            P.taskManager.EnqueueDelay(100);
            P.taskManager.Enqueue(TargetInteract);
            if (armory)
            {
                P.taskManager.Enqueue(() => GenericHandlers.FireCallback("SelectIconString", true, SelectIconString), "Fire Callback 1");
            }
            P.taskManager.Enqueue(() => GenericHandlers.FireCallback("SelectString", true, SelectString), "Fire Callback 2");
            P.taskManager.Enqueue(() => IsAddonActive("ShopExchangeItem"), "Addon Active");
        }
        internal static void Exchange(int gearItem, int List, int Amount)
        {
            Svc.Log.Debug($"Exchange  gearid: {gearItem} List: {List} Amount: {Amount}");
            if (Amount > 127)
                Amount = 127;

            
            P.taskManager.Enqueue(() => DoExchange(gearItem, "ShopExchangeItem", true, 0, List, Amount));
            P.taskManager.EnqueueDelay(100);
        }
        internal unsafe static bool DoExchange(int gearItem, string AddonName, bool kapkac, params int[] gibeme)
        {
            if (DidAmountChange(0,GetItemCount(gearItem)))
            {
                return true;
            }
            if (TryGetAddonByName<AtkUnitBase>("ShopExchangeItemDialog", out var addon2) && IsAddonReady(addon2))
            {
                if (FrameThrottler.Throttle("GlobalTurnInGenericThrottle", 20))
                    Callback.Fire(addon2, true, 0);
            }
            else if (TryGetAddonByName<AtkUnitBase>("SelectYesno", out var addon3) && IsAddonReady(addon3))
            {
                if (FrameThrottler.Throttle("GlobalTurnInGenericThrottleee", 20))
                    Callback.Fire(addon3, true, 0);
            }
            else if (TryGetAddonByName<AtkUnitBase>(AddonName, out var addon) && IsAddonReady(addon))
            {
                if(FrameThrottler.Throttle("GlobalTurnInGenericThrottlee", 20))
                    Callback.Fire(addon, kapkac, gibeme.Cast<object>().ToArray());
            }
            return false;
        }
    }
}
