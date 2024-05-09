using System;
using System.Collections.Generic;
using System.Linq;
using NoxRaven;
using NoxRaven.Data;
using NoxRaven.Events;
using NoxRaven.UnitAgents;
using static War3Api.Blizzard;
using static War3Api.Common;

namespace NoxRaven
{
    public partial class NAgent
    {
        /// <summary>
        /// Call this to remove unit instantly.
        /// Not calling will leak.
        /// </summary>
        public void GracefulRemove()
        {
            Remove();
        }

        public void Execute(NAgent whoToKill)
        {
            RawDamage(whoToKill, GetWidgetLife(whoToKill) + 1);
        }

        public int GetId() => War3Api.Common.GetHandleId(wc3agent);

        /// <summary>
        /// Ensure all damage goes through this.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void RawDamage(NAgent target, float damage)
        {
            _damageEngineIgnore = true;
            UnitDamageTarget(
                this,
                target,
                damage,
                true,
                false,
                ATTACK_TYPE_CHAOS,
                DAMAGE_TYPE_UNIVERSAL,
                null
            );
            _damageEngineIgnore = false;
            if (GetWidgetLife(target) <= 0.305)
                AwaitRemoval(target, this); // weird bug BECAUSE WC3REFUNDED SHIT HEHE
        }

        public void ResetBasicAttackTimer()
        {
            UnitAddAbility(wc3agent, _resetAAAbility);
        }

        /// <summary>
        /// Damage parsers that takes care of all calculations. Damage parser calculates outgoing damage from the unit.
        /// </summary>
        public void DealDamage(
            NAgent target,
            float damage,
            DamageOnHit dmgOnHit,
            DamageCrit dmgCrit,
            DamageSource dmgSource,
            DamageType dmgType,
            bool stopRecursion = false
        )
        {
            _DealDamage(target, damage, dmgOnHit, dmgCrit, dmgSource, dmgType, stopRecursion);
        }

        public void AddModifier(NDataModifier mod)
        {
            if (mod != null)
            {
                state.ApplyModifier(mod);
            }
        }

        public void RemoveModifier(NDataModifier mod)
        {
            if (mod != null)
            {
                state.UnapplyModifier(mod);
            }
        }

        public void AddDisposable(UnitDisposable ud)
        {
            _disposables.Add(ud);
        }

        public void RemoveDisposable(UnitDisposable ud)
        {
            _disposables.Remove(ud);
        }

        public void AddAbility(NAbility abil)
        {
            if (!_abilities.ContainsKey(abil.GetType()))
                _abilities.Add(abil.GetType(), new SortedList<NAbility>());

            if (abil.unique && _abilities[abil.GetType()].Count > 0)
            {
                NAbility firstInst = _abilities[abil.GetType()].First();
                _abilities[abil.GetType()].Add(abil);
                NAbility updatedInst = _abilities[abil.GetType()].First();
                if (updatedInst == abil)
                {
                    _UnapplyAbility(firstInst);
                    _ApplyAbility(abil);
                }
            }
            else
            {
                _abilities[abil.GetType()].Add(abil);
                _ApplyAbility(abil);
            }
            abil.OnAdded();
        }

        public void RemoveAbility(NAbility abil)
        {
            if (!_abilities.ContainsKey(abil.GetType()))
                _abilities.Add(abil.GetType(), new SortedList<NAbility>());

            abil.OnRemoved();
            if (abil.unique && _abilities[abil.GetType()].Count > 1)
            {
                NAbility first = _abilities[abil.GetType()].First();
                if (first == abil)
                {
                    _abilities[abil.GetType()].Remove(abil);
                    _UnapplyAbility(abil);
                    _ApplyAbility(_abilities[abil.GetType()].First());
                }
                else
                {
                    _abilities[abil.GetType()].Remove(abil);
                }
            }
            else
            {
                _UnapplyAbility(abil);
                _abilities[abil.GetType()].Remove(abil);
            }
        }

        /// <summary>
        /// Returns all abilities of type T (or derived) in _abilities (sorted by priority)
        /// </summary>
        public List<T> GetAllAbilities<T>()
            where T : NAbility
        {
            List<T> list = new List<T>();
            // search for ability of type (fast) or any derived type in _abilities
            if (_abilities.TryGetValue(typeof(T), out SortedList<NAbility> abils))
                foreach (var abil in abils)
                {
                    list.Add((T)abil);
                }
            foreach (var kv in _abilities)
            {
                if (kv.Key.IsSubclassOf(typeof(T)))
                    foreach (var abil in kv.Value)
                    {
                        list.Add((T)abil);
                    }
            }
            return list;
        }

        // public void RemoveStatus(int id)
        // {
        //     _statuses.Remove(id);
        // }
        // public Status GetStatus(int id)
        // {
        //     return _statuses[id];
        // }
        /// <summary>
        /// Heal unit by % missing hp. Unit with 50% HP and 10% Missing Healing will receive effective 5% heal.
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealHPPercentMissing(float percentHealed, bool show = false)
        {
            HealHP(percentHealed * (BlzGetUnitMaxHP(this) - GetWidgetLife(this)), show);
        }

        /// <summary>
        /// Simple function that heals a unit by percentHealed (%) of Max HP.<para />
        /// Range of percentHealed: 0.00 - 1.00
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealHPPercentMax(float percentHealed, bool show = false)
        {
            HealHP(percentHealed * BlzGetUnitMaxHP(this), show);
        }

        /// <summary>
        /// Simple function that heals a unit by howMuch amount (flat).
        /// </summary>
        /// <param name="howMuch"></param>
        public void HealHP(float howMuch, bool show = false)
        {
            float pars = howMuch * state[EUnitState.INCOMING_HEALING];
            // TODO onHealHP event
            SetWidgetLife(this, GetWidgetLife(this) + pars);
            if (show)
            {
                location loc = Location(
                    GetUnitX(this) + GetRandomReal(0, 10),
                    GetUnitY(this) + GetRandomReal(0, 5)
                );
                Utils.TextDirectionRandom(
                    "+" + Utils.NotateNumber(R2I(pars)),
                    loc,
                    5.7f,
                    128,
                    255,
                    128,
                    0,
                    0.7f,
                    GetOwningPlayer(this)
                );
                RemoveLocation(loc);
                loc = null;
            }
        }

        public void HealMP(float howMuch, bool show = false)
        {
            // TODO onHealMana event
            SetUnitManaBJ(
                this,
                GetUnitState(this, UNIT_STATE_MANA) + howMuch * state[EUnitState.INCOMING_MANA]
            );
            if (show)
            {
                location loc = Location(
                    GetUnitX(this) + GetRandomReal(0, 10),
                    GetUnitY(this) + GetRandomReal(0, 5)
                );
                Utils.TextDirectionRandom(
                    "+" + Utils.NotateNumber(R2I(howMuch)),
                    loc,
                    5.7f,
                    128,
                    128,
                    255,
                    0,
                    0.7f,
                    GetOwningPlayer(this)
                );
                RemoveLocation(loc);
                loc = null;
            }
        }

        public void SubscribeToEvent(IBehaviour pb)
        {
            string fullName = pb.GetType().GetGenericArguments()[0].FullName;
            if (!_events.ContainsKey(fullName))
            {
                _events.Add(fullName, new BehaviourList<Events.EventArgs>());
            }
            _events[fullName].Add(pb);
        }

        public void UnsubscribeFromEvent(IBehaviour pb)
        {
            _events[pb.GetType().GetGenericArguments()[0].FullName].Remove(pb);
        }

        /// <summary>
        /// Trigger a certain event type
        /// </summary>
        /// <param name="eventMeta"></param>
        /// <typeparam name="T"></typeparam>
        public void TriggerEvent<T>(T eventMeta)
            where T : Events.EventArgs
        {
            if (_events.ContainsKey(typeof(T).FullName))
            {
                _events[typeof(T).FullName].InvokeBehaviours(eventMeta);
            }
            if (_globalEvents.ContainsKey(typeof(T).FullName))
            {
                _globalEvents[typeof(T).FullName].InvokeBehaviours(eventMeta);
            }
        }
    }
}
