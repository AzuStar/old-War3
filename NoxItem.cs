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
        protected ItemAction pickUp = null;
        protected ItemAction drop = null;
        protected ItemAction use = null;

        private IModifier mod = null;
        private List<IPriorityBehaviour> _behaviours = null;

        internal static void InitItemLogic()
        {
            trigger itemPick = CreateTrigger();
            trigger itemDrop = CreateTrigger();
            trigger itemUse = CreateTrigger();

            TriggerRegisterAnyUnitEventBJ(itemPick, EVENT_PLAYER_UNIT_PICKUP_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemDrop, EVENT_PLAYER_UNIT_PAWN_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemDrop, EVENT_PLAYER_UNIT_DROP_ITEM);
            // TriggerRegisterAnyUnitEventBJ(itemUse, EVENT_PLAYER_UNIT_USE_ITEM);

            TriggerAddAction(itemPick, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = s_index[type];
                if (check != null)
                    NoxUnit.Cast(GetManipulatingUnit()).AddModifier(check.mod);
            });
            TriggerAddAction(itemDrop, () =>
            {
                int type = GetItemTypeId(GetManipulatedItem());
                NoxItem check = s_index[type];
                if (check != null)
                    NoxUnit.Cast(GetManipulatingUnit()).RemoveModifier(check.mod);
            });
            // TriggerAddAction(itemUse, () =>
            // {
            //     int type = GetItemTypeId(GetManipulatedItem());
            //     NoxItem check = s_index[type];
            //     if (check != null)
            //         check.use.Invoke(GetManipulatingUnit());
            // });
        }
        private NoxItem(IModifier mod)
        {  
            this.mod = mod;
        }

        public static void RegisterItem(int itemCode, IModifier mod)
        {
            s_index.Add(itemCode, new NoxItem(mod));
        }
        public static void RegisterItem(string fourCC, IModifier mod)
        {
            RegisterItem(FourCC(fourCC), mod);
        }
    }
}
