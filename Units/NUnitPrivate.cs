using System;
using System.Collections.Generic;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Statuses;
using NoxRaven;
using NoxRaven.Events;
using NoxRaven.UnitAgents;

using static NoxRaven.UnitAgents.EUnitState;

namespace NoxRaven.Units
{
    public partial class NUnit
    {
        // ****************
        // * Unit Methods *
        // ****************
        private void DamageHandler()
        {
            if (_damageEngineIgnore) return;
            if (GetEventDamage() < 1) return;
            NUnit source = Cast(GetEventDamageSource());
            BlzSetEventDamage(0);
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.state[GREY_ATK] + source.state[GREEN_ATK];

            UnitRemoveAbility(source.wc3agent, _resetAAAbility);

            source._DealDamage(this, dmg, DamageOnHit.DEFAULT_BASIC_AA, DamageCrit.DEFAULT_BASIC_AA, DamageSource.BASIC_ATTACK, DamageType.PHYSICAL, false);
        }

        private void Remove()
        {
            // not unit disposable
            foreach (SortedList<Status> st in _statuses.Values)
                foreach (Status s in st)
                    s.Remove();

            // not unit disposable
            foreach (SortedList<NAbility> ab in abilities.Values)
                foreach (NAbility a in ab)
                    a.DetachAbility();

            foreach (UnitDisposable ud in _disposables)
            {
                ud.Dispose();
            }
            TriggerEvent(new OnRecycle() { Target = this });
            onHits.Clear();
            //AmHits.Clear();
            DestroyTrigger(dmgHookTrig);
            s_indexer.Remove(War3Api.Common.GetHandleId(wc3agent));
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

        internal void _ApplyAbility(NAbility inst)
        {
            inst.AttachAbility(this);
            if (inst.modifier != null)
            {
                AddModifier(inst.modifier);
            }
            foreach (IBehaviour behaviour in inst.localBehaviours)
            {
                SubscribeToEvent(behaviour);
            }
            foreach (IBehaviour behaviour in inst.globalBehaviours)
            {
                SubscribeToGlobalEvent(behaviour);
            }
        }
        internal void _UnapplyAbility(NAbility inst)
        {
            inst.DetachAbility();
            if (inst.modifier != null)
            {
                RemoveModifier(inst.modifier);
            }
            foreach (IBehaviour behaviour in inst.localBehaviours)
            {
                UnsubscribeFromEvent(behaviour);
            }
            foreach (IBehaviour behaviour in inst.globalBehaviours)
            {
                UnsubscribeFromGlobalEvent(behaviour);
            }
        }

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
        internal bool ContainsStatus<StatusType>() where StatusType : Status
        {
            //if(st.GetHashCode()<=100)
            return _statuses.ContainsKey(typeof(StatusType));
        }
        #endregion

        private void UpdateDisposables()
        {
            foreach (UnitDisposable disp in _disposables)
            {
                disp.Update();
            }
        }

        private void _DealDamage(NUnit target, float damage, DamageOnHit dmgOnHit, DamageCrit dmgCrit, DamageSource dmgsource, DamageType dmgtype, bool stopRecursion = false)
        {
            if (damage < 0) return;
            location loc = Location(GetUnitX(target) + GetRandomReal(0, 5), GetUnitY(target) + GetRandomReal(0, 5));
            // here for now ?
            if (dmgsource == DamageSource.BASIC_ATTACK && GetRandomReal(0, 1) < target.state[DODGE_CHANCE])
            {
                Utils.TextDirectionRandom("dodge!", loc, 3.5f, 255, 255, 255, 0, 0.8f, GetOwningPlayer(this));
                Utils.TextDirectionRandom("dodge!", loc, 3.5f, 255, 255, 255, 0, 0.8f, GetOwningPlayer(target));
                return;
            }
            float pars = damage;


            if (dmgtype == DamageType.PHYSICAL)
            {
                pars *= UnitUtils.GetDamageReductionFromArmor(target.state[GREY_ARM] + target.state[GREEN_ARM]);
            }
            else if (dmgtype == DamageType.MAGICAL)
            {
                pars *= UnitUtils.GetDamageReductionFromArmor(target.state[GREY_RES] + target.state[GREEN_RES]);
            }
            //Event Pars

            OnDamageDealt evt = new OnDamageDealt()
            {
                caller = this,
                target = target,
                dmgOnHit = dmgOnHit,
                dmgCrit = dmgCrit,
                dmgsource = dmgsource,
                dmgtype = dmgtype,
                noRecursion = stopRecursion,
                rawDamage = damage,

                processedDamage = pars,
            };
            TriggerEvent(evt);

            pars = evt.processedDamage;
            float critC = state[CRIT_CHANCE] + evt.dmgCrit.critChanceBonus;
            float critD = state[CRIT_DAMAGE] * (1 + evt.dmgCrit.critDamageBonus);
            // The logic
            float txtSize = 5.9f;
            float txtDur = 0.8f;
            string txtSuffix = "";
            if (evt.dmgCrit.applyCrit)
            {
                if (evt.dmgCrit.guaranteedCrit || GetRandomReal(0, 1) < critC)
                {
                    pars *= (1 + critD);

                    txtSize *= 1.5f;
                    txtDur *= 1.5f;
                    txtSuffix = "!";
                }
            }
            if (Master.s_numbersOn)
            {
                Utils.TextDirectionRandom(Utils.NotateNumber(R2I(pars)) + txtSuffix, loc, 5.9f, 255, 0, 0, 0, 0.8f, GetOwningPlayer(this));
                Utils.TextDirectionRandom(Utils.NotateNumber(R2I(pars)) + txtSuffix, loc, 5.9f, 255, 0, 0, 0, 0.8f, GetOwningPlayer(target));
            }


            RawDamage(target, pars);

            if (dmgsource == DamageSource.SPELL)
                HealHP(pars * state[SPELLVAMP]);
            else if (dmgsource == DamageSource.ONHIT || dmgsource == DamageSource.BASIC_ATTACK)
                HealHP(pars * state[LIFESTEAL]);

            // Now that's done
            // Onhits
            if (dmgOnHit.applyOnHit)
            {
                foreach (OnHit onhit in onHits.Values)
                    onhit.ApplyOnHit(this, target, damage, pars);
                //ApplyAmHits(source);
            }

            // cleanup
            RemoveLocation(loc);
            loc = null;
        }
    }
}