using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven
{
    /// <summary>
    /// Not *yet* indexed item without data.
    /// Improve logic to address more cases. (targeted abils can be created through trigger spell effect tho)
    /// </summary>
    public class NoxItem
    {
        public delegate void ItemAction(NoxUnit manipulator);
        public static Dictionary<int, NoxItem> Indexer = new Dictionary<int, NoxItem>();
        /// <summary>
        /// All charged items will be stacking.
        /// </summary>
        public static bool ChargedStacking = true;
        private ItemAction PickUp;
        private ItemAction Drop;
        private ItemAction Use;
        public readonly bool ShopPurchasable;

        internal static void InitItemLogic()
        {
            trigger itemPick = CreateTrigger();
            trigger itemDrop = CreateTrigger();
            trigger itemUse = CreateTrigger();

            TriggerRegisterAnyUnitEventBJ(itemPick, EVENT_PLAYER_UNIT_PICKUP_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemDrop, EVENT_PLAYER_UNIT_PAWN_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemDrop, EVENT_PLAYER_UNIT_DROP_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemUse, EVENT_PLAYER_UNIT_USE_ITEM);

            TriggerAddAction(itemPick, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = Indexer[type];
                if (check != null)
                    check.PickUp.Invoke(GetManipulatingUnit());
            });
            TriggerAddAction(itemDrop, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = Indexer[type];
                if (check != null)
                    check.Drop.Invoke(GetManipulatingUnit());
            });
            TriggerAddAction(itemUse, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = Indexer[type];
                if (check != null)
                    check.Use.Invoke(GetManipulatingUnit());
            });
        }
        private NoxItem(ItemAction pickUp, ItemAction drop, ItemAction use, bool purchasable)
        {
            PickUp = pickUp;
            Drop = drop;
            Use = use;
            ShopPurchasable = purchasable;
        }
        /// <summary>
        /// FourcCC 4 characters
        /// </summary>
        /// <param name="itemTypeId"></param>
        /// <param name="pickUp"></param>
        /// <param name="drop"></param>
        public static void RegisterItem(int itemTypeId, ItemAction pickUp, ItemAction drop, ItemAction use, bool purchasable)
        {
            Indexer.Add(itemTypeId, new NoxItem(pickUp, drop, use, purchasable));
        }
    }
}
