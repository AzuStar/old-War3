using System;
using System.Collections.Generic;
using NoxRaven;
using NoxRaven.Data;
using NoxRaven.Events;
using NoxRaven.UnitAgents;
using static NoxRaven.UnitAgents.EUnitState;
using static War3Api.Blizzard;
using static War3Api.Common;

namespace NoxRaven
{
    public partial class NAgent
    {
        // ****************
        // * Unit Methods *
        // ****************
        private void DamageHandler()
        {
            if (_damageEngineIgnore)
                return;
            if (GetEventDamage() < 1)
                return;
            NAgent source = Cast(GetEventDamageSource());
            BlzSetEventDamage(0);
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.state[GREY_ATK] + source.state[GREEN_ATK];

            UnitRemoveAbility(source.wc3agent, _resetAAAbility);

            source._DealDamage(
                this,
                dmg,
                DamageOnHit.DEFAULT_BASIC_AA,
                DamageCrit.DEFAULT_BASIC_AA,
                DamageSource.BASIC_ATTACK,
                DamageType.PHYSICAL,
                false
            );
        }

        private void Remove()
        {
            // not unit disposable
            // foreach (SortedList<Status> st in _statuses.Values)
            // foreach (Status s in st)
            //     s.Remove();

            // not unit disposable
            foreach (SortedList<NAbility> ab in _abilities.Values)
            foreach (NAbility a in ab)
                RemoveAbility(a);

            foreach (UnitDisposable ud in _disposables)
            {
                ud.Dispose();
            }
            TriggerEvent(new OnRecycle() { Target = this });
            onHits.Clear();
            //AmHits.Clear();
            DestroyTrigger(_dmgHookTrig);
            DestroyTrigger(_attackHookTrig);
            DestroyTrigger(_spellEffectTrig);
            s_indexer.Remove(War3Api.Common.GetHandleId(wc3agent));
            onHits = null;
            RemoveUnit(this);
            _dmgHookTrig = null;
        }

        internal void _ApplyAbility(NAbility inst)
        {
            inst.CollectCurrentData();
            inst.OnApplied();
            if (inst.localBehaviours != null)
                foreach (IBehaviour behaviour in inst.localBehaviours)
                {
                    SubscribeToEvent(behaviour);
                }
            if (inst.dataDependencies != null)
                foreach (NDataDependency data in inst.dataDependencies)
                {
                    state.AddDependency(data);
                }
            if (inst.modifier != null)
                state.ApplyModifier(inst.modifier);
        }

        internal void _UnapplyAbility(NAbility inst)
        {
            inst.OnUnapplied();
            if (inst.localBehaviours != null)
                foreach (IBehaviour behaviour in inst.localBehaviours)
                {
                    UnsubscribeFromEvent(behaviour);
                }
            if (inst.dataDependencies != null)
                foreach (NDataDependency data in inst.dataDependencies)
                {
                    state.RemoveDependency(data);
                }
            if (inst.modifier != null)
                state.UnapplyModifier(inst.modifier);
            if (inst.statListeners != null)
                foreach (StatListener listener in inst.statListeners)
                {
                    listener.Dispose();
                }
            inst.statListeners = null;
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
        #endregion

        private void UpdateDisposables()
        {
            foreach (UnitDisposable disp in _disposables)
            {
                disp.Update();
            }
        }

        private void _DealDamage(
            NAgent target,
            float damage,
            DamageOnHit dmgOnHit,
            DamageCrit dmgCrit,
            DamageSource dmgsource,
            DamageType dmgtype,
            bool stopRecursion = false
        )
        {
            if (damage < 0)
                return;
            location loc = Location(
                GetUnitX(target) + GetRandomReal(0, 5),
                GetUnitY(target) + GetRandomReal(0, 5)
            );
            // here for now ?
            if (
                dmgsource == DamageSource.BASIC_ATTACK
                && GetRandomReal(0, 1) < target.state[DODGE_CHANCE]
            )
            {
                Utils.TextDirectionRandom(
                    "dodge!",
                    loc,
                    3.5f,
                    255,
                    255,
                    255,
                    0,
                    0.8f,
                    GetOwningPlayer(this)
                );
                Utils.TextDirectionRandom(
                    "dodge!",
                    loc,
                    3.5f,
                    255,
                    255,
                    255,
                    0,
                    0.8f,
                    GetOwningPlayer(target)
                );
                return;
            }
            float pars = damage;

            if (dmgtype == DamageType.PHYSICAL)
            {
                pars *= UnitUtils.GetDamageReductionFromArmor(
                    target.state[GREY_ARMOR] + target.state[GREEN_ARMOR]
                );
            }
            else if (dmgtype == DamageType.MAGICAL)
            {
                pars *= UnitUtils.GetDamageReductionFromArmor(
                    target.state[GREY_MAGIC_RESIST] + target.state[GREEN_MAGIC_RESIST]
                );
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
            float critD = state[CRIT_DAMAGE] * (1 + evt.dmgCrit.critDamageMultiplier);
            // The logic
            float txtSize = 5.9f;
            float txtDur = 0.8f;
            string txtSuffix = "";
            if (evt.dmgCrit.applyCrit)
            {
                if (evt.dmgCrit.guaranteedCrit || GetRandomReal(0, 1) < critC)
                {
                    pars *= (1 + critD);

                    txtSize *= 1.3f;
                    txtDur *= 1.3f;
                    txtSuffix = "!";
                }
            }
            if (Master.s_numbersOn)
            {
                Utils.TextDirectionRandom(
                    Utils.NotateNumber(R2I(pars)) + txtSuffix,
                    loc,
                    txtSize,
                    235,
                    109,
                    30,
                    0,
                    0.8f,
                    GetOwningPlayer(this)
                );
                Utils.TextDirectionRandom(
                    Utils.NotateNumber(R2I(pars)) + txtSuffix,
                    loc,
                    txtSize,
                    235,
                    109,
                    30,
                    0,
                    0.8f,
                    GetOwningPlayer(target)
                );
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

        private void _Regenerate(float delta)
        {
            // note to self: if you are even thinking of optimizing this
            // reread the entire cache assembly section of the lua_cpp you f@ck
            SetUnitState(
                wc3agent,
                UNIT_STATE_LIFE,
                GetUnitState(this, UNIT_STATE_LIFE)
                    + state[EUnitState.HP_REG] * delta * (1 + state[EUnitState.INCOMING_HEALING])
            );
            SetUnitState(
                wc3agent,
                UNIT_STATE_MANA,
                GetUnitState(this, UNIT_STATE_MANA)
                    + state[EUnitState.MP_REG] * delta * (1 + state[EUnitState.INCOMING_MANA])
            );
        }

        private void _SetStarterStats()
        {
            state[BASE_MOVE_SPEED] = BlzGetUnitRealField(this, UNIT_RF_SPEED);
            state[BASE_HP] = BlzGetUnitRealField(this, UNIT_RF_HP);
            state[BASE_MP] = BlzGetUnitRealField(this, UNIT_RF_MANA);
            state[BASE_ATK] = BlzGetUnitWeaponIntegerField(
                this,
                UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE,
                0
            );
            state[BASE_RELOAD_TIME] = BlzGetUnitWeaponRealField(
                this,
                UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN,
                0
            );
            state[BASE_ARMOR] = BlzGetUnitRealField(this, UNIT_RF_DEFENSE);
        }
    }
}
