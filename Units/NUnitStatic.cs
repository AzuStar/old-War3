using System;
using System.Collections.Generic;
using System.Diagnostics;
using NoxRaven.Events;
using NoxRaven.Statuses;
using static War3Api.Common;

namespace NoxRaven.Units
{
    public partial class NUnit
    {
        public static Dictionary<int, NUnit> s_indexer = new Dictionary<int, NUnit>();
        private static Dictionary<int, Type> s_customTypes = new Dictionary<int, Type>();
        private static Dictionary<Type, int> s_inversedCustomType = new Dictionary<Type, int>();
        private static int _resetAAAbility = FourCC("A00E");
        private static bool _damageEngineIgnore = false;

        private static Dictionary<string, BehaviourList<Events.EventArgs>> _globalEvents = new Dictionary<string, BehaviourList<Events.EventArgs>>();

        /// <summary>
        /// Put a custom type that will be attached to a unit when indexing.
        /// Custom type has to extend this class and invoke base(u) in the constructor.<br/><br/>
        /// WHEN ASSIGNING HERO TYPE, MAKE SURE CONSTRUCTOR HAS SAME ARGUEMNTS AS <see cref="NHero"/>:<br/>
        /// <code>
        /// public MyHero(unit u, HeroStats statsPerLevel = null, HeroStats initialStats = null) : base(u, statsPerLevel, initialStats)
        /// </code>
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t">hit type</param>
        public static void AddCustomType<T>(int unitId) where T : NUnit
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
        public static void AddCustomType<T>(string unitId) where T : NUnit
        {
            AddCustomType<T>(FourCC(unitId));
        }

        public static T CreateCustomUnit<T>(NPlayer owner, float x, float y, float facing = 0) where T : NUnit
        {
            return (T)Cast(CreateUnit(owner.wc3agent, s_inversedCustomType[typeof(T)], x, y, facing));
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
            TriggerRegisterLeaveRegion(CreateTrigger(), reg, Filter(() => { ((NUnit)GetLeavingUnit()).Remove(); return false; })); // catch unit removal, destroy everything attached
                                                                                                                                   // Utility functions


            Master.s_globalTick.Add((delta) =>
            {
                foreach (NUnit ue in s_indexer.Values)
                {
                    if (ue.corpse) continue;
                    ue.UpdateDisposables();
                    ue.Regenerate(delta);
                }
            });

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
            try
            {
                if (s_customTypes.ContainsKey(GetUnitTypeId(u)))
                    s_indexer[War3Api.Common.GetHandleId(u)] = (NUnit)Activator.CreateInstance(s_customTypes[GetUnitTypeId(u)], u);
                else
                if (IsUnitType(u, UNIT_TYPE_HERO))
                    s_indexer[War3Api.Common.GetHandleId(u)] = new NHero(u);
                else
                    s_indexer[War3Api.Common.GetHandleId(u)] = new NUnit(u);

                // is pickup triggered on units when map starts?
                // if(UnitInventorySize(u) > 0)
                // {
                //     for(int i = 0; i < UnitInventorySize(u); i++)
                //     {
                //         item it = UnitItemInSlot(u, i);
                //         if (it != null)
                //             NItem.itemPickUp
                //     }
                // }
            }
            catch (Exception e)
            {
                Utils.Debug("0x0F04: " + e.Message);
            }
            u = null;
            return false;
        }
        /// <summary>
        /// Do not call RemoveUnit on indexed unit or permaleak.
        /// </summary>
        /// <param name="u"></param>
        private static void AwaitRemoval(NUnit u, NUnit killer)
        {
            if (!s_indexer.ContainsKey(u.GetId())) return; // this is a very weird thing to happen, but will happen for Neutrals so yeah
            if (u.corpse) return;

            foreach (SortedList<Status> st in u._statuses.Values)
                foreach (Status s in st)
                    if (s.removeOndeath)
                        s.Remove();

            OnDeath odmeta = new OnDeath()
            {
                killer = killer,
                caller = u,
                keepCorpse = true,
                keepCorpseFor = 10,
            };
            u.TriggerEvent<OnDeath>(odmeta);

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return; // wat
            if (IsUnitType(u, UNIT_TYPE_HERO)) return; // always leak heroes

            if (odmeta.keepCorpse)
            {
                u.corpse = true;

                Utils.DelayedInvoke(odmeta.keepCorpseFor, () => { u.Remove(); });
            }
            else u.Remove();
        }

        public static void SubscribeToGlobalEvent(IBehaviour pb)
        {
            string fullName = pb.GetType().GetGenericArguments()[0].FullName;
            if (!_globalEvents.ContainsKey(fullName))
            {
                _globalEvents.Add(fullName, new BehaviourList<Events.EventArgs>());
            }
            _globalEvents[pb.GetType().GetGenericArguments()[0].FullName].Add(pb);
        }
        /// <summary>
        /// Unscubscribes from a certain event type
        /// </summary>
        /// <param name="behaviour"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnsubscribeFromGlobalEvent(IBehaviour pb)
        {
            _globalEvents[pb.GetType().GetGenericArguments()[0].FullName].Remove(pb);
        }


        // may be implicit operator, Ive been down this road
        public static NUnit Cast(unit u)
        {
            return s_indexer[War3Api.Common.GetHandleId(u)];
        }
        public static implicit operator NUnit(unit u)
        {
            return Cast(u);
        }
        public static implicit operator unit(NUnit ue)
        {
            return ue.wc3agent;
        }
    }
}