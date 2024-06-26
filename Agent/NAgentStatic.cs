using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using NoxRaven.Events;
using static War3Api.Common;

namespace NoxRaven
{
    public sealed partial class NAgent
    {
        public static Dictionary<int, NAgent> s_indexer = new Dictionary<int, NAgent>();
        private static Dictionary<int, NUnitInitializer> s_customTypes =
            new Dictionary<int, NUnitInitializer>();
        private static int _resetAAAbility = FourCC("ASPD");
        private static bool _damageEngineIgnore = false;

        private static Dictionary<string, BehaviourList<Events.EventArgs>> _globalEvents =
            new Dictionary<string, BehaviourList<Events.EventArgs>>();

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
        public static void SetCustomType(int unitId, NUnitInitializer initialier)
        {
            s_customTypes[unitId] = initialier;
        }

        /// <summary>
        /// Put a custom type that will be attached to a unit when indexing.
        /// Custom type has to extend this class and invoke base(u) in the constructor.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t"></param>
        public static void SetCustomType(string unitId, NUnitInitializer initializer)
        {
            SetCustomType(FourCC(unitId), initializer);
        }

        public static NAgent CreateCustomUnit(
            NPlayer owner,
            string fourcc,
            Vector2 position,
            float facing = 0
        )
        {
            return Cast(CreateUnit(owner.wc3agent, FourCC(fourcc), position.X, position.Y, facing));
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
            TriggerRegisterLeaveRegion(
                CreateTrigger(),
                reg,
                Filter(() =>
                {
                    if (s_indexer.ContainsKey(War3Api.Common.GetHandleId(GetLeavingUnit())))
                        ((NAgent)GetLeavingUnit()).Remove();
                    return false;
                })
            ); // catch unit removal, destroy everything attached
            // Utility functions


            Master.s_globalTick.Add(
                (delta) =>
                {
                    foreach (NAgent ue in s_indexer.Values)
                    {
                        if (ue.corpse)
                            continue;
                        ue.UpdateDisposables();
                        ue._Regenerate(delta);
                    }
                }
            );

            // Recycle stuff
            DestroyGroup(g);
            RemoveRect(rec);
            g = null;
            rec = null;
            reg = null;
        }

        private static bool AttachClass()
        {
            unit u = GetFilterUnit();

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0)
                return false;
            if (s_indexer.ContainsKey(War3Api.Common.GetHandleId(u)))
                return false;

            if (s_customTypes.ContainsKey(GetUnitTypeId(u)))
                s_indexer[War3Api.Common.GetHandleId(u)] = new NAgent(
                    u,
                    s_customTypes[GetUnitTypeId(u)]
                );
            else
                s_indexer[War3Api.Common.GetHandleId(u)] = new NAgent(u);
            u = null;
            return false;
        }

        /// <summary>
        /// Do not call RemoveUnit on indexed unit or permaleak.
        /// </summary>
        /// <param name="u"></param>
        private static void AwaitRemoval(NAgent u, NAgent killer)
        {
            if (!s_indexer.ContainsKey(u.GetId()))
                return; // this is a very weird thing to happen, but will happen for Neutrals so yeah
            if (u.corpse)
                return;

            OnDeath odmeta = new OnDeath()
            {
                killer = killer,
                caller = u,
                keepCorpse = true,
                keepCorpseFor = 10,
            };
            u.TriggerEvent<OnDeath>(odmeta);

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0)
                return; // wat
            if (IsUnitType(u, UNIT_TYPE_HERO))
                return; // always leak heroes

            if (odmeta.keepCorpse)
            {
                u.corpse = true;

                Utils.DelayedInvoke(
                    odmeta.keepCorpseFor,
                    () =>
                    {
                        u.Remove();
                    }
                );
            }
            else
                u.Remove();
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
            if (!_globalEvents.ContainsKey(pb.GetType().GetGenericArguments()[0].FullName))
                return;
            _globalEvents[pb.GetType().GetGenericArguments()[0].FullName].Remove(pb);
        }

        // may be implicit operator, Ive been down this road
        public static NAgent Cast(unit u)
        {
            return s_indexer[War3Api.Common.GetHandleId(u)];
        }

        public static implicit operator NAgent(unit u)
        {
            return Cast(u);
        }

        public static implicit operator unit(NAgent ue)
        {
            return ue.wc3agent;
        }
    }
}
