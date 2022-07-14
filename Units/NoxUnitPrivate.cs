using System;
using System.Collections.Generic;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Statuses;
using NoxRaven;
using NoxRaven.Events;
using NoxRaven.UnitAgents;

namespace NoxRaven.Units
{
    public partial class NoxUnit
    {
        // ****************
        // * Unit Methods *
        // ****************
        private void DamageHandler()
        {
            if (DamageEngineIgnore) return;
            if (GetEventDamage() < 1) return;
            NoxUnit source = Cast(GetEventDamageSource());
            BlzSetEventDamage(0);
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.DMG;
            source._DealDamage(this, dmg, true, true, DamageSource.BASIC_ATTACK, DamageType.PHYSICAL, false);
        }

        protected NoxUnit(unit u)
        {
            _self_ = u;
            _stats.baseMS = GetUnitMoveSpeed(u);
            _stats.baseHP = BlzGetUnitMaxHP(u);
            _stats.baseMP = BlzGetUnitMaxMana(u);
            _stats.baseDMG = BlzGetUnitBaseDamage(u, 0);
            _stats.baseAttackCooldown = BlzGetUnitAttackCooldown(_self_, 0);
            _stats.baseARM = BlzGetUnitArmor(u);
            _RecalculateStats();
            // Damage Utilization
            DamageTrig = CreateTrigger();
            TriggerRegisterUnitEvent(DamageTrig, u, EVENT_UNIT_DAMAGED);
            TriggerAddAction(DamageTrig, DamageHandler);
        }

        private void Remove()
        {
            TriggerEvent(new RemovalEvent() { Target = this });
            OnHits.Clear();
            //AmHits.Clear();
            DestroyTrigger(DamageTrig);
            Indexer.Remove(War3Api.Common.GetHandleId(_self_));
            OnHits = null;
            Statuses = null;
            RemoveUnit(this);
            DamageTrig = null;
        }

        // /// <summary>
        // /// Set unit's -xxx.x or +xxx.x armor. Does support single precision decimal point.
        // /// </summary>
        // /// <param name="val"></param>
        // protected void SetGreenArmor(float val)
        // {
        //     int leftover = R2I(val);
        //     int decimals = R2I((val - R2I(val)) * 10);
        //     if (val < 0) decimals = 1 - decimals;
        //     SetUnitAbilityLevel(s_ref, FourCC("ARDP"), decimals + 1);
        //     GreenArmor = val;
        //     foreach (int abil in Abilities_BonusArmor)
        //         UnitRemoveAbility(this, abil);
        //     foreach (int abil in Abilities_Corruption)
        //         UnitRemoveAbility(this, abil);

        //     if (leftover < 0)
        //     {
        //         leftover = -leftover;
        //         for (int i = Abilities_Corruption.Length - 1; i >= 0; i--)
        //         {
        //             int comparator = R2I(Pow(2, i));
        //             if (comparator <= leftover)
        //             {
        //                 UnitAddAbility(this, Abilities_Corruption[i]);
        //                 leftover -= comparator;
        //             }
        //         }
        //     }
        //     else
        //         for (int i = Abilities_BonusArmor.Length - 1; i >= 0; i--)
        //         {
        //             int comparator = R2I(Pow(2, i));
        //             if (comparator <= leftover)
        //             {
        //                 UnitAddAbility(this, Abilities_BonusArmor[i]);
        //                 leftover -= comparator;
        //             }
        //         }
        // }

        // // This guy makes +xxx damage magic
        // protected void SetGreenDamage(int val)
        // {
        //     GreenDamage = val;
        //     for (int i = Abilities_BonusDamage.Length - 1; i >= 0; i--)
        //     {
        //         UnitRemoveAbility(this, Abilities_BonusDamage[i]);
        //         int comparator = R2I(Pow(2, i));
        //         if (comparator <= val)
        //         {
        //             UnitAddAbility(this, Abilities_BonusDamage[i]);
        //             val -= comparator;
        //         }
        //     }
        // }

        // protected virtual void Regenerate()
        // {
        //     RegenerationTickEvent parsEvent = new RegenerationTickEvent()
        //     {
        //         EventInfo = new NoxRaven.Events.RegenerationMeta()
        //         {
        //             Target = s_ref
        //         },
        //         HealthValue = RegenFlat,
        //         ManaValue = RegenManaFlat
        //     };
        //     HealHP(parsEvent.HealthValue * RegenerationTimeout);
        //     HealMP(parsEvent.ManaValue * RegenerationTimeout);
        // }

        private void _RecalculateStats()
        {
            BlzSetUnitAttackCooldown(_self_, _stats.baseAttackCooldown / (1 + _stats.attackSpeed), 0);
            SetUnitMoveSpeed(_self_, _stats.baseMS * _stats.baseMSPercent);
            BlzSetUnitMaxHP(this, R2I((_stats.baseHP * (1 + _stats.baseHPPercent)) * (1 + _stats.baseHPPercentBonus) + _stats.bonusHP + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues

        }

        #region internal Status/onhit api
        internal bool ContainsOnHit(int id)
        {
            return OnHits.ContainsKey(id);
        }
        internal OnHit AddOnHit(int id, OnHit toAdd)
        {
            OnHits.Add(id, toAdd);
            return toAdd;
        }
        internal OnHit GetOnHit(int id)
        {
            return OnHits[id];
        }
        internal void RemoveOnHit(int id)
        {
            OnHits.Remove(id);
        }
        internal bool ContainsStatus(int id)
        {
            //if(st.GetHashCode()<=100)
            return Statuses.ContainsKey(id);
        }
        internal Status AddStatus(int id, Status toAdd)
        {
            Statuses.Add(id, toAdd);
            return toAdd;
        }
        #endregion

        private void _DealDamage(NoxUnit target, float damage, bool triggerOnHit, bool triggerCrit, DamageSource dmgsource, DamageType dmgtype, bool stopRecursion = false)
        {
            if (damage < 0) return;
            location loc = Location(GetUnitX(target) + GetRandomReal(0, 5), GetUnitY(target) + GetRandomReal(0, 5));
            float pars = damage;


            float critC = _stats.critChace;
            float critD = _stats.critDamage;
            // maths
            if (dmgtype == DamageType.PHYSICAL || dmgtype == DamageType.MAGICAL)
                pars *= (1 - Math.Min(target._stats.damageReduction, 1));
            float armor = BlzGetUnitArmor(target);
            if (armor < 0)
                pars *= (1.71f - Pow(1f - ARMOR_MR_REDUCTION, -armor)); // war3 real armor reduction is 1.71-pow(xxx) - why? - no idea
            else pars *= 1 / (1 + armor * ARMOR_MR_REDUCTION * (1 - _stats.armorPenetration)); // Inverse armor reduction function, got by solving: Armor * CONST / (1 + ARMOR * CONST)

            //Event Pars

            OnDamageDealt evt = new OnDamageDealt()
            {
                caller = this,
                target = target,
                triggerOnHit = triggerOnHit,
                triggerCrit = triggerCrit,
                dmgsource = dmgsource,
                dmgtype = dmgtype,
                noRecursion = stopRecursion,
                rawDamage = damage,

                processedDamage = pars,
                critChance = critC,
                critDamage = critD,
            };
            TriggerEvent(evt);
            pars = evt.processedDamage;
            critC = evt.critChance;
            critD = evt.critDamage;
            // The logic

            if (triggerCrit && GetRandomReal(0, 1) < critC)
            {
                pars *= critD;

                if (Master.s_numbersOn)
                {
                    Utils.TextDirectionRandom(Utils.NotateNumber(R2I(pars)) + "!", loc, 8.5f, 255, 0, 0, 0, 1.3f, GetOwningPlayer(this));
                    Utils.TextDirectionRandom(Utils.NotateNumber(R2I(pars)) + "!", loc, 8.5f, 255, 0, 0, 0, 1.3f, GetOwningPlayer(target));
                }
            }
            else
            {
                if (Master.s_numbersOn)
                {
                    Utils.TextDirectionRandom(Utils.NotateNumber(R2I(pars)), loc, 6.9f, 255, 0, 0, 0, 0.8f, GetOwningPlayer(this));
                    Utils.TextDirectionRandom(Utils.NotateNumber(R2I(pars)), loc, 6.9f, 255, 0, 0, 0, 0.8f, GetOwningPlayer(target));
                }
            }

            RawDamage(target, pars);

            if (dmgsource == DamageSource.SPELL)
                HealHP(pars * _stats.spellVamp);
            else if (dmgsource == DamageSource.ONHIT || dmgsource == DamageSource.BASIC_ATTACK)
                HealHP(pars * _stats.lifeSteal);

            // Now that's done
            // Onhits
            if (triggerOnHit)
            {
                List<OnHit> onhits = new List<OnHit>(OnHits.Values);
                foreach (OnHit onhit in onhits)
                    onhit.ApplyOnHit(this, target, damage, pars);
                //ApplyAmHits(source);
            }

            // cleanup
            RemoveLocation(loc);
            loc = null;
        }
    }
}