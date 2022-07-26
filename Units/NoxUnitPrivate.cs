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
            if (_damageEngineIgnore) return;
            if (GetEventDamage() < 1) return;
            NoxUnit source = Cast(GetEventDamageSource());
            BlzSetEventDamage(0);
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.DMG;

            UnitRemoveAbility(source._self_, _resetAAAbility);

            source._DealDamage(this, dmg, true, true, DamageSource.BASIC_ATTACK, DamageType.PHYSICAL, false);
        }



        private void Remove()
        {
            TriggerEvent(new OnRecycle() { Target = this });
            onHits.Clear();
            //AmHits.Clear();
            DestroyTrigger(dmgHookTrig);
            s_indexer.Remove(War3Api.Common.GetHandleId(_self_));
            onHits = null;
            _statuses = null;
            RemoveUnit(this);
            dmgHookTrig = null;
        }

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

        #region internal Status/onhit api
        internal bool ContainsOnHit(int id)
        {
            return onHits.ContainsKey(id);
        }
        internal OnHit AddOnHit(int id, OnHit toAdd)
        {
            onHits.Add(id, toAdd);
            return toAdd;
        }
        internal OnHit GetOnHit(int id)
        {
            return onHits[id];
        }
        internal void RemoveOnHit(int id)
        {
            onHits.Remove(id);
        }
        internal bool ContainsStatus(int id)
        {
            //if(st.GetHashCode()<=100)
            return _statuses.ContainsKey(id);
        }
        internal Status AddStatus(int id, Status toAdd)
        {
            _statuses.Add(id, toAdd);
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

            if (dmgtype == DamageType.PHYSICAL)
            {
                pars *= UnitUtils.GetDamageReductionFromArmor(target.ARM);
            }
            else if (dmgtype == DamageType.MAGICAL)
            {
                pars *= UnitUtils.GetDamageReductionFromArmor(target.MR);
            }
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
                List<OnHit> onhits = new List<OnHit>(onHits.Values);
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