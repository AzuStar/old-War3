using System;
using System.Collections.Generic;
using NoxRaven.Events;
using NoxRaven.Statuses;
using static War3Api.Common;

namespace NoxRaven.Units
{
    public partial class NoxUnit
    {
        protected internal static Dictionary<int, NoxUnit> s_indexer = new Dictionary<int, NoxUnit>();
        private static Dictionary<int, Type> s_customTypes = new Dictionary<int, Type>();
        private static Dictionary<Type, int> s_inversedCustomType = new Dictionary<Type, int>();
        private static int _resetAAAbility = FourCC("A00E");
        private static float KeepCorpsesFor = 25;
        private static bool _damageEngineIgnore = false;

        private static Dictionary<string, BehaviourList<Events.EventArgs>> _globalEvents = new Dictionary<string, BehaviourList<Events.EventArgs>>();

        /// <summary>
        /// Put a custom type that will be attached to a unit when indexing.
        /// Custom type has to extend this class and invoke base(u) in the constructor.<br/><br/>
        /// WHEN ASSIGNING HERO TYPE, MAKE SURE CONSTRUCTOR HAS SAME ARGUEMNTS AS <see cref="NoxHero"/>:<br/>
        /// <code>
        /// public MyHero(unit u, HeroStats statsPerLevel = null, HeroStats initialStats = null) : base(u, statsPerLevel, initialStats)
        /// </code>
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t">hit type</param>
        public static void AddCustomType<T>(int unitId) where T : NoxUnit
        {
            s_customTypes[unitId] = typeof(T);
            s_inversedCustomType[typeof(T)] = unitId;
        }
        /// <summary>
        /// Put a custom type that will be attached to a unit when indexing.
        /// Custom type has to extend this class and invoke base(u) in the constructor.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t"></param>
        public static void AddCustomType<T>(string unitId) where T : NoxUnit
        {
            AddCustomType<T>(FourCC(unitId));
        }

        public static NoxUnit CreateCustomUnit<T>(NoxPlayer owner, float x, float y, float facing = 0)
        {
            return CreateUnit(owner._self_, s_inversedCustomType[typeof(T)], x, y, facing);
        }
        /// <summary>
        /// Put this initializer somewhere after all players have been initialized. Do this only after you have put all customtypes in the dictionary.
        /// </summary>
        internal static void InitUnitLogic()
        {
            region reg = CreateRegion();
            group g = CreateGroup();
            rect rec = GetWorldBounds();
            RegionAddRect(reg, rec);
            TriggerRegisterEnterRegion(CreateTrigger(), reg, Filter(AttachClass));

            // Add existing units
            GroupEnumUnitsInRect(g, rec, Filter(AttachClass));

            // Deattach when unit leaves the map
            TriggerRegisterLeaveRegion(CreateTrigger(), reg, Filter(() => { ((NoxUnit)GetLeavingUnit()).Remove(); return false; })); // catch unit removal, destroy everything attached
                                                                                                                                     // Utility functions


            Master.s_globalTick.Add((delta) => { foreach (NoxUnit ue in s_indexer.Values) ue.Regenerate(delta); });

            // Recycle stuff
            DestroyGroup(g);
            g = null;
            rec = null;
            reg = null;
        }

        private static bool AttachClass()
        {
            unit u = GetFilterUnit();

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return false;
            if (s_indexer.ContainsKey(War3Api.Common.GetHandleId(u))) return false;

            if (s_customTypes.ContainsKey(GetUnitTypeId(u))) s_indexer[War3Api.Common.GetHandleId(u)] = (NoxUnit)Activator.CreateInstance(s_customTypes[GetUnitTypeId(u)], u);
            else if (IsUnitType(u, UNIT_TYPE_HERO)) s_indexer[War3Api.Common.GetHandleId(u)] = new NoxHero(u, null, null);
            else s_indexer[War3Api.Common.GetHandleId(u)] = new NoxUnit(u);
            u = null;
            return false;
        }
        /// <summary>
        /// Do not call RemoveUnit on indexed unit or permaleak.
        /// </summary>
        /// <param name="u"></param>
        private static void AwaitRemoval(NoxUnit u, NoxUnit killer)
        {
            if (!s_indexer.ContainsKey(u.GetId())) return; // this is a very weird thing to happen, but will happen for Neutrals so yeah
            if (u.corpse) return;

            foreach (Status st in u._statuses.Values)
                st.Remove();
            u._statuses.Clear();// just in case

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return; // wat
            if (IsUnitType(u, UNIT_TYPE_HERO)) return; // always leak heroes

            u.corpse = true;

            Utils.DelayedInvoke(KeepCorpsesFor, () => { u.Remove(); }); // ah shiet, change to let resurrections
                                                                        //ue = null;
        }

        /// <summary>
        /// Subscribes to a certain event type
        /// </summary>
        /// <param name="behaviour"></param>
        /// <typeparam name="T"></typeparam>
        public static void SubscribeToGlobalEvent<T>(Behaviour<T> behaviour) where T : Events.EventArgs
        {
            _globalEvents[typeof(T).FullName].Add(behaviour);
        }
        /// <summary>
        /// Unscubscribes from a certain event type
        /// </summary>
        /// <param name="behaviour"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnsubscribeFromGlobalEvent<T>(Behaviour<T> behaviour) where T : Events.EventArgs
        {
            _globalEvents[typeof(T).FullName].Remove(behaviour);
        }


        // may be implicit operator, Ive been down this road
        public static NoxUnit Cast(unit u)
        {
            return s_indexer[War3Api.Common.GetHandleId(u)];
        }
        public static implicit operator NoxUnit(unit u)
        {
            return Cast(u);
        }
        public static implicit operator unit(NoxUnit ue)
        {
            return ue._self_;
        }
    }
}