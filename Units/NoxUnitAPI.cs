using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven;
using NoxRaven.Events;
using System.Collections.Generic;
using System;
using NoxRaven.Statuses;
using NoxRaven.UnitAgents;

namespace NoxRaven.Units
{
    public partial class NoxUnit
    {
        /// <summary>
        /// Call this to remove unit instantly.
        /// Not calling will leak.
        /// </summary>
        public void GracefulRemove()
        {
            Remove();
        }

        // public void Kill(NoxUnit whoToKill)
        // {
        //     Kill(whoToKill);
        //     AwaitRemoval(this, whoToKill);
        // }

        public int GetId() => War3Api.Common.GetHandleId(_self_);

        /// <summary>
        /// Ensure all damage goes through this.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void RawDamage(NoxUnit target, float damage)
        {
            _damageEngineIgnore = true;
            UnitDamageTarget(this, target, damage, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null);
            _damageEngineIgnore = false;
            if (GetWidgetLife(target) <= 0.305) AwaitRemoval(target, this);// weird bug BECAUSE WC3REFUNDED SHIT HEHE
        }

        public void ResetBasicAttackTimer()
        {
            UnitAddAbility(_self_, _resetAAAbility);
        }

        /// <summary>
        /// Damage parsers that takes care of all calculations. Damage parser calculates outgoing damage from the unit.
        /// </summary>
        /// <param name="target">Whos is the target</param>
        /// <param name="damage"></param>
        /// <param name="triggerOnHit">Does it apply on-hit effects?</param>
        /// <param name="triggerCrit">Can it crit?</param>
        public void DealDamage(NoxUnit target, float damage, bool triggerOnHit, bool triggerCrit, DamageSource dmgSource, DamageType dmgType, bool stopRecursion = false)
        {
            _DealDamage(target, damage, triggerOnHit, triggerCrit, dmgSource, dmgType, stopRecursion);
        }

        public void AddModifier(IModifier mod)
        {
            if (mod != null)
            {
                _stats = mod.ApplyModifier(_stats);
                RecalculateStats(_stats);
            }

        }
        public void RemoveModifier(IModifier mod)
        {
            if (mod != null)
            {
                _stats = mod.UnapplyModifier(_stats);
                RecalculateStats(_stats);
            }
        }

        public void RemoveStatus(int id)
        {
            _statuses.Remove(id);
        }
        public Status GetStatus(int id)
        {
            return _statuses[id];
        }
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
            float pars = howMuch * _stats.incomingHealing;
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
            SetUnitManaBJ(this, GetUnitState(this, UNIT_STATE_MANA) + howMuch * _stats.incomingMana);
            if (show)
            {
                location loc = Location(GetUnitX(this) + GetRandomReal(0, 10), GetUnitY(this) + GetRandomReal(0, 5));
                Utils.TextDirectionRandom("+" + Utils.NotateNumber(R2I(howMuch)), loc, 5.7f, 128, 128, 255, 0, 0.7f, GetOwningPlayer(this));
                RemoveLocation(loc);
                loc = null;
            }
        }

        /// <summary>
        /// Subscribes to a certain event type
        /// </summary>
        /// <param name="behaviour"></param>
        /// <typeparam name="T"></typeparam>
        public void SubscribeToEvent<T>(Behaviour<T> behaviour) where T : Events.EventArgs
        {
            if (_events.ContainsKey(typeof(T).FullName))
            {
                _events[typeof(T).FullName].Add(behaviour);
            }
        }
        /// <summary>
        /// Unscubscribes from a certain event type
        /// </summary>
        /// <param name="behaviour"></param>
        /// <typeparam name="T"></typeparam>
        public void UnsubscribeFromEvent<T>(Behaviour<T> behaviour) where T : Events.EventArgs
        {
            if (_events.ContainsKey(typeof(T).FullName))
            {
                _events[typeof(T).FullName].Remove(behaviour);
            }
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
                _globalEvents[typeof(T).FullName].InvokeBehaviours(eventMeta);
            }
        }
    }
}