using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;
using NoxRaven.UnitAgents;
using NoxRaven.Events;

namespace NoxRaven
{
    /// <summary>
    /// Not *yet* indexed item without data.
    /// Improve logic to address more cases. (targeted abils can be created through trigger spell effect tho)
    /// </summary>
    public class NoxItem
    {
        public delegate void ItemAction(NoxUnit manipulator);
        public static Dictionary<int, NoxItem> s_index = new Dictionary<int, NoxItem>();
        /// <summary>
        /// All charged items will be stacking.
        /// </summary>
        public static bool s_chargedStack = true;
        private ItemAction _pickUp = null;
        private ItemAction _drop = null;
        private ItemAction _use = null;

        private Modifier mod = null;
        private List<IPriorityBehaviour> _behaviours = null;

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
                NoxItem check = s_index[type];
                if (check != null)
                    check._pickUp.Invoke(GetManipulatingUnit());
            });
            TriggerAddAction(itemDrop, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = s_index[type];
                if (check != null)
                    check._drop.Invoke(GetManipulatingUnit());
            });
            TriggerAddAction(itemUse, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = s_index[type];
                if (check != null)
                    check._use.Invoke(GetManipulatingUnit());
            });
        }
        private NoxItem(ItemAction pickUp, ItemAction drop, ItemAction use)
        {
            _pickUp = pickUp;
            _drop = drop;
            _use = use;
        }
    }
}
