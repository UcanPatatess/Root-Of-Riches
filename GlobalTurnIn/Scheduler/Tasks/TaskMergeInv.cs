using Lumina.Excel.Sheets;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace GlobalTurnIn.Scheduler.Tasks
{
    internal class TaskMergeInv
    {
        private static Dictionary<uint, Item> Sheet { get; set; } = null!;
        private static readonly List<InventorySlot> inventorySlots = [];
        public class InventorySlot
        {
            public InventoryType Container { get; set; }
            public ushort Slot { get; set; }
            public uint ItemID { get; set; }
            public bool ItemHQ { get; set; }
        }
        private static unsafe void MergeStacks()
        {
            Sheet = GetSheet<Item>().ToDictionary(x => x.RowId, x => x);
            if (IsOccupied()) return;

            inventorySlots.Clear();
            var inv = InventoryManager.Instance();
            for (var container = InventoryType.Inventory1; container <= InventoryType.Inventory4; container++)
            {
                var invContainer = inv->GetInventoryContainer(container);
                for (var i = 1; i <= invContainer->Size; i++)
                {
                    var item = invContainer->GetInventorySlot(i - 1);
                    if (item->Flags.HasFlag(InventoryItem.ItemFlags.Collectable)) continue;
                    if (item->Quantity == Sheet[item->ItemId].StackSize || item->ItemId == 0) continue;
                    var slot = new InventorySlot()
                    {
                        Container = container,
                        ItemID = item->ItemId,
                        Slot = (ushort)item->Slot,
                        ItemHQ = item->Flags.HasFlag(InventoryItem.ItemFlags.HighQuality)
                    };

                    inventorySlots.Add(slot);
                }
            }

            foreach (var item in inventorySlots.GroupBy(x => new { x.ItemID, x.ItemHQ }).Where(x => x.Count() > 1))
            {
                var firstSlot = item.First();
                for (var i = 1; i < item.Count(); i++)
                {
                    var slot = item.ToList()[i];
                    inv->MoveItemSlot(slot.Container, slot.Slot, firstSlot.Container, firstSlot.Slot, 1);
                }
            }
        }
        internal static void Enqueue()
        {
            P.taskManager.Enqueue(MergeStacks);
            P.taskManager.EnqueueDelay(200);
        }
    }
}
