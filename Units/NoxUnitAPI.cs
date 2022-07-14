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
        public float lookupbaseDMG => _stats.baseDMG;
        public float lookupbaseDMGPercent => _stats.baseDMGPercent;
        public float lookupbonusDMG => _stats.bonusDMG;
        public float lookupbaseDMGPercentBonus => _stats.baseDMGPercentBonus;
        public float lookuparmorPenetration => _stats.armorPenetration;
        public float lookupattackSpeed => _stats.attackSpeed;
        public float lookupbaseAttackCooldown => _stats.baseAttackCooldown;
        public float lookupbaseAP => _stats.baseAP;
        public float lookupbaseAPPercent => _stats.baseAPPercent;
        public float lookupbonusAP => _stats.bonusAP;
        public float lookupbaseAPPercentBonus => _stats.baseAPPercentBonus;
        public float lookupbaseHP => _stats.baseHP;
        public float lookupbaseHPPercent => _stats.baseHPPercent;
        public float lookupbonusHP => _stats.bonusHP;
        public float lookupbaseHPPercentBonus => _stats.baseHPPercentBonus;
        public float lookupregenHP => _stats.regenHP;
        public float lookupregenHPPercent => _stats.regenHPPercent;
        public float lookupbaseMP => _stats.baseMP;
        public float lookupbaseMPPercent => _stats.baseMPPercent;
        public float lookupbonusMP => _stats.bonusMP;
        public float lookupbaseMPPercentBonus => _stats.baseMPPercentBonus;
        public float lookupregenMP => _stats.regenMP;
        public float lookupregenMPPercent => _stats.regenMPPercent;
        public float lookupbaseARM => _stats.baseARM;
        public float lookupbaseARMPercent => _stats.baseARMPercent;
        public float lookupbonusARM => _stats.bonusARM;
        public float lookupbaseARMPercentBonus => _stats.baseARMPercentBonus;
        public float lookupbaseMR => _stats.baseMR;
        public float lookupbaseMRPercent => _stats.baseMRPercent;
        public float lookupbonusMR => _stats.bonusMR;
        public float lookupbaseMRPercentBonus => _stats.baseMRPercentBonus;
        public float lookupcritChace => _stats.critChace;
        public float lookupcritDamage => _stats.critDamage;
        public float lookuplifeSteal => _stats.lifeSteal;
        public float lookupspellVamp => _stats.spellVamp;
        public float lookupincomingHealing => _stats.incomingHealing;
        public float lookupincomingMana => _stats.incomingMana;
        public float lookupdamageReduction => _stats.damageReduction;
        public float lookuptriggerChance => _stats.triggerChance;
        public float lookupbaseMSPercent => _stats.baseMSPercent;
        public float lookupbaseMS => _stats.baseMS;

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
            DamageEngineIgnore = true;
            UnitDamageTarget(this, target, damage, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null);
            DamageEngineIgnore = false;
            if (GetWidgetLife(target) <= 0.305) AwaitRemoval(target, this);// weird bug BECAUSE WC3REFUNDED SHIT HEHE
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

        public void AddModifier(Modifier mod)
        {
            if (mod != null)
            {
                _stats = mod.ApplyModifier(_stats);
                _RecalculateStats();
            }

        }
        public void RemoveModifier(Modifier mod)
        {
            if (mod != null)
            {
                _stats = mod.UnapplyModifier(_stats);
                _RecalculateStats();
            }
        }

        public void RemoveStatus(int id)
        {
            Statuses.Remove(id);
        }
        public Status GetStatus(int id)
        {
            return Statuses[id];
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