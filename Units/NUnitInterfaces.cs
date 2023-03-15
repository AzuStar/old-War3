using System;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.UnitAgents;
using NoxRaven.Events;
using NoxRaven.Data;

using static NoxRaven.UnitAgents.EUnitState;
using System.Diagnostics;

namespace NoxRaven.Units
{
    // protected and virtual fields/methods
    public partial class NUnit
    {
        protected NUnit(unit u, NDataModifier initialStats = null)
        {
            wc3agent = u;
            if (initialStats == null)
            {
                state[BASE_MOVE_SPEED] = BlzGetUnitRealField(u, UNIT_RF_SPEED);
                state[BASE_HP] = BlzGetUnitRealField(u, UNIT_RF_HP);
                state[BASE_MP] = BlzGetUnitRealField(u, UNIT_RF_MANA);
                state[BASE_ATK] = BlzGetUnitWeaponIntegerField(u, UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE, 0);
                state[RELOAD_TIME] = BlzGetUnitWeaponRealField(u, UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN, 0);
                state[BASE_ARMOR] = BlzGetUnitRealField(u, UNIT_RF_DEFENSE);
            }
            else
                state.ResetFromModifier(initialStats);
            state.AddListener(MAX_HP, (prev, cur) =>
            {
                BlzSetUnitMaxHP(this, R2I(state[MAX_HP] + Utils.ROUND_DOWN_CONST_OVERHEAD));
                return cur;
            });
            // max_mp
            state.AddListener(MAX_MP, (prev, cur) =>
            {
                BlzSetUnitMaxMana(this, R2I(state[MAX_MP] + Utils.ROUND_DOWN_CONST_OVERHEAD));
                return cur;
            });
            state.AddListener(UNIT_MS, (prev, cur) =>
            {
                SetUnitMoveSpeed(wc3agent, state[UNIT_MS]);
                return cur;
            });
            state.AddListener(RLD, (prev, cur) =>
            {
                BlzSetUnitAttackCooldown(wc3agent, state[RLD], 0);
                return cur;
            });
            // when unit indexed (aka created) it must have full vitals
            SetUnitState(u, UNIT_STATE_LIFE, 999999999);
            SetUnitState(u, UNIT_STATE_MANA, 999999999);

            dmgHookTrig = CreateTrigger();
            TriggerRegisterUnitEvent(dmgHookTrig, u, EVENT_UNIT_DAMAGED);
            TriggerAddAction(dmgHookTrig, DamageHandler);

            attackHookTrig = CreateTrigger();
            TriggerRegisterUnitEvent(attackHookTrig, u, EVENT_UNIT_ATTACKED);
            TriggerAddAction(attackHookTrig, () =>
            {
                NUnit attacker = GetAttacker();
                OnAttack attackEvt = new OnAttack()
                {
                    caller = attacker,
                    target = this
                };
                attacker.TriggerEvent(attackEvt);
            });
        }

        // protected virtual void RecalculateStats(Stats myStats)
        // {
        //     BlzSetUnitAttackCooldown(wc3agent, myStats.baseAttackCooldown / (1 + myStats.attackSpeed), 0);
        //     SetUnitMoveSpeed(wc3agent, myStats.baseMS * (1 + myStats.baseMSPercent));
        //     BlzSetUnitMaxHP(this, R2I(((myStats.baseHP * (1 + myStats.baseHPPercent)) * (1 + myStats.baseHPPercentBonus) + myStats.bonusHP) * (1 + myStats.totalHPPercent) + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
        //     BlzSetUnitMaxMana(this, R2I(((myStats.baseMP * (1 + myStats.baseMPPercent)) * (1 + myStats.baseMPPercentBonus) + myStats.bonusMP) * (1 + myStats.totalMPPercent) + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
        // }

        protected virtual void Regenerate(float delta)
        {
            SetUnitState(wc3agent, UNIT_STATE_LIFE, GetUnitState(this, UNIT_STATE_LIFE) + state[EUnitState.HP_REG] * delta * (1 + state[EUnitState.INCOMING_HEALING]));
            SetUnitState(wc3agent, UNIT_STATE_MANA, GetUnitState(this, UNIT_STATE_MANA) + state[EUnitState.MP_REG] * delta * (1 + state[EUnitState.INCOMING_MANA]));
        }
    }
}