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
    /// Improve logic to address more cases. (targeted abils can be created through trigger spell effect tho)
    /// Base class for all items. <br />
    /// Constructor parsed with nothing. <br />
    /// </summary>
    public class NItem
    {
        public delegate void ItemAction(NUnit manipulator, item it);
        public static Dictionary<int, NItem> s_index = new Dictionary<int, NItem>();
        public static Dictionary<int, Type> s_customTypes = new Dictionary<int, Type>();
        public static Dictionary<Type, int> s_inverseCustomTypes = new Dictionary<Type, int>();

        public static trigger itemPickUp = CreateTrigger();
        public static trigger itemDrop = CreateTrigger();
        public static trigger itemUse = CreateTrigger();

        /// <summary>
        /// All charged items will be stacking.
        /// </summary>
        public static bool s_chargedStack = true;

        public int typeId;
        protected ItemAction pickUp = null;
        protected ItemAction drop = null;
        protected ItemAction use = null;

        protected IModifier mod = null;
        private List<NAbility> _abilities = new List<NAbility>();
        protected List<Type> abilities = new List<Type>();

        public static void AddCustomType<T>(int typeId) where T : NItem
        {
            s_customTypes.Add(typeId, typeof(T));
            s_inverseCustomTypes.Add(typeof(T), typeId);
        }
        public static void AddCustomType<T>(string type) where T : NItem
        {
            AddCustomType<T>(FourCC(type));
        }
        internal static void _InitItemLogic()
        {

            TriggerRegisterAnyUnitEventBJ(itemPickUp, EVENT_PLAYER_UNIT_PICKUP_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemDrop, EVENT_PLAYER_UNIT_PAWN_ITEM);
            TriggerRegisterAnyUnitEventBJ(itemDrop, EVENT_PLAYER_UNIT_DROP_ITEM);
            // TriggerRegisterAnyUnitEventBJ(itemUse, EVENT_PLAYER_UNIT_USE_ITEM);

            TriggerAddAction(itemPickUp, _ItemPickedUp);
            TriggerAddAction(itemDrop, _ItemDropped);
            // TriggerAddAction(itemUse, () =>
            // {
            //     int type = GetItemTypeId(GetManipulatedItem());
            //     NoxItem check = s_index[type];
            //     if (check != null)
            //         check.use.Invoke(GetManipulatingUnit());
            // });
        }
        internal static void _ItemPickedUp()
        {
            item it = GetManipulatedItem();
            NUnit nu = GetManipulatingUnit();
            int id = GetHandleId(it);
            if (!s_index.ContainsKey(id))
            {
                // Custom item
                Type type = s_customTypes[GetItemTypeId(it)];
                if (type == null) return;
                NItem item = (NItem)Activator.CreateInstance(type, true);
                s_index.Add(id, item);
                item.typeId = GetItemTypeId(it);
            }
            NItem check = s_index[id];
            nu.AddModifier(check.mod);
            foreach (Type t in check.abilities)
            {
                check._abilities.Add(nu.AddAbility(t));
            }
            check.pickUp?.Invoke(nu, it);
        }
        internal static void _ItemDropped()
        {
            item it = GetManipulatedItem();
            NUnit nu = GetManipulatingUnit();
            int id = GetHandleId(it);
            if (s_index.ContainsKey(id))
            {
                NItem check = s_index[id];
                nu.RemoveModifier(check.mod);
                foreach (NAbility ab in check._abilities)
                {
                    nu.RemoveAbility(ab);
                }
                check.drop?.Invoke(nu, GetManipulatedItem());
            }
        }
        protected NItem()
        {
            // s_customTypes.Add(typeId, this.GetType());
            // s_inverseCustomTypes.Add(this.GetType(), typeId);
        }
    }
}
