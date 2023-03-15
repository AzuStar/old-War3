using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven;
using NoxRaven.Events;
using System.Collections.Generic;
using System;
using NoxRaven.Statuses;
using NoxRaven.UnitAgents;
using System.Linq;
using NoxRaven.Data;

namespace NoxRaven.Units
{
    public partial class NUnit
    {
        /// <summary>
        /// Call this to remove unit instantly.
        /// Not calling will leak.
        /// </summary>
        public void GracefulRemove()
        {
            Remove();
        }

        public void Execute(NUnit whoToKill)
        {
            RawDamage(whoToKill, GetWidgetLife(whoToKill));
        }

        public int GetId() => War3Api.Common.GetHandleId(wc3agent);

        /// <summary>
        /// Ensure all damage goes through this.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void RawDamage(NUnit target, float damage)
        {
            _damageEngineIgnore = true;
            UnitDamageTarget(this, target, damage, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null);
            _damageEngineIgnore = false;
            if (GetWidgetLife(target) <= 0.305) AwaitRemoval(target, this);// weird bug BECAUSE WC3REFUNDED SHIT HEHE
        }

        public void ResetBasicAttackTimer()
        {
            UnitAddAbility(wc3agent, _resetAAAbility);
        }

        /// <summary>
        /// Damage parsers that takes care of all calculations. Damage parser calculates outgoing damage from the unit.
        /// </summary>
        public void DealDamage(NUnit target, float damage, DamageOnHit dmgOnHit, DamageCrit dmgCrit, DamageSource dmgSource, DamageType dmgType, bool stopRecursion = false)
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
        public void AddAbility(NAbility abil)
        {
            if (abil.unique)
            {
                if (!abilitiesUniques.ContainsKey(abil.GetType()))
                    abilitiesUniques.Add(abil.GetType(), new SortedList<NAbility>());
                if (abilitiesUniques[abil.GetType()].Count > 0)
                {
                    NAbility firstInst = abilitiesUniques[abil.GetType()].First();
                    if (firstInst.level < abil.level)
                    {
                        _UnapplyAbility(firstInst);
                        _ApplyAbility(abil);
                    }
                }
            }
            else
            {
                _ApplyAbility(abil);
            }
            abilitiesUniques[abil.GetType()].Add(abil);
        }
        public void RemoveAbility(NAbility abil)
        {
            if (abil.unique)
            {
                if (!abilitiesUniques.ContainsKey(abil.GetType()))
                    abilitiesUniques.Add(abil.GetType(), new SortedList<NAbility>());
                    if (abilitiesUniques[abil.GetType()].Count > 1)
                    {
                        NAbility first = abilitiesUniques[abil.GetType()].First();
                        if(first == abil){
                         abilitiesUniques[abil.GetType()].Remove(abil);   
                        _UnapplyAbility(abil);
                        _ApplyAbility(abilitiesUniques[abil.GetType()].First());
                        }
                        
                    }
            }
            else
            {
                _UnapplyAbility(abil);
                
            }
            abilitiesUniques[abil.GetType()].Remove(abil);
        }
        public Status AddStatus<T>() where T : Status
        {
            Status st = (Status)Activator.CreateInstance(typeof(T), this);
            if (!_statuses.ContainsKey(typeof(T)))
            {
                _statuses.Add(typeof(T), new SortedList<Status>());
            }
            _statuses[typeof(T)].Add(st);

            return st;
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
        public virtual void HealHP(float howMuch, bool show = false)
        {
            float pars = howMuch * state[EUnitState.INCOMING_HEALING];
            // TODO onHealHP event
            SetWidgetLife(this, GetWidgetLife(this) + pars);
            if (show)
            {
                location loc = Location(GetUnitX(this) + GetRandomReal(0, 10), GetUnitY(this) + GetRandomReal(0, 5));
                Utils.TextDirectionRandom("+" + Utils.NotateNumber(R2I(pars)), loc, 5.7f, 128, 255, 128, 0, 0.7f, GetOwningPlayer(this));
                RemoveLocation(loc);
                loc = null;
            }
        }
        public virtual void HealMP(float howMuch, bool show = false)
        {
            // TODO onHealMana event
            SetUnitManaBJ(this, GetUnitState(this, UNIT_STATE_MANA) + howMuch * state[EUnitState.INCOMING_MANA]);
            if (show)
            {
                location loc = Location(GetUnitX(this) + GetRandomReal(0, 10), GetUnitY(this) + GetRandomReal(0, 5));
                Utils.TextDirectionRandom("+" + Utils.NotateNumber(R2I(howMuch)), loc, 5.7f, 128, 128, 255, 0, 0.7f, GetOwningPlayer(this));
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
            _events[pb.GetType().GetGenericArguments()[0].FullName].Add(pb);
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
        public void TriggerEvent<T>(T eventMeta) where T : Events.EventArgs
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