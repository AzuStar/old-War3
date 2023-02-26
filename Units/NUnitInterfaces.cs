using System;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.UnitAgents;
using NoxRaven.Events;

namespace NoxRaven.Units
{
    // protected and virtual fields/methods
    public partial class NUnit
    {
        protected NUnit(unit u, Stats initialStats = null)
        {
            wc3agent = u;
            if (initialStats == null)
            {
                _stats.baseMS = BlzGetUnitRealField(u, UNIT_RF_SPEED);
                _stats.baseHP = BlzGetUnitRealField(u, UNIT_RF_HP);
                _stats.baseMP = BlzGetUnitRealField(u, UNIT_RF_MANA);
                _stats.baseDMG = BlzGetUnitWeaponIntegerField(u, UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE, 0);
                _stats.baseAttackCooldown = BlzGetUnitWeaponRealField(u, UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN, 0);
                _stats.baseARM = BlzGetUnitRealField(u, UNIT_RF_DEFENSE);
            }
            else
                _stats = initialStats;
            RecalculateStats(_stats);
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

        protected virtual void RecalculateStats(Stats myStats)
        {
            BlzSetUnitAttackCooldown(wc3agent, myStats.baseAttackCooldown / (1 + myStats.attackSpeed), 0);
            SetUnitMoveSpeed(wc3agent, myStats.baseMS * (1 + myStats.baseMSPercent));
            BlzSetUnitMaxHP(this, R2I(((myStats.baseHP * (1 + myStats.baseHPPercent)) * (1 + myStats.baseHPPercentBonus) + myStats.bonusHP) * (1 + myStats.totalHPPercent) + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
            BlzSetUnitMaxMana(this, R2I(((myStats.baseMP * (1 + myStats.baseMPPercent)) * (1 + myStats.baseMPPercentBonus) + myStats.bonusMP) * (1 + myStats.totalMPPercent) + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues

            state.DMG = (myStats.baseDMG * (1 + myStats.baseDMGPercent) * (1 + myStats.baseDMGPercentBonus) + myStats.bonusDMG) * (1 + myStats.totalDMGPercent);
            state.AP = (myStats.baseAP * (1 + myStats.baseAPPercent) * (1 + myStats.baseAPPercentBonus) + myStats.bonusAP) * (1 + myStats.totalAPPercent);
            state.ARM = (myStats.baseARM * (1 + myStats.baseARMPercent) * (1 + myStats.baseARMPercentBonus) + myStats.bonusARM) * (1 + myStats.totalARMPercent);
            state.MR = (myStats.baseMR * (1 + myStats.baseMRPercent) * (1 + myStats.baseMRPercentBonus) + myStats.bonusMR) * (1 + myStats.totalMRPercent);
            state.HPRegen = myStats.regenHP * (1 + myStats.regenHPPercent);
            state.MPRegen = myStats.regenMP * (1 + myStats.regenMPPercent);
        }

        protected virtual void Regenerate(float delta)
        {
            OnRegeneration regenEvt = new OnRegeneration()
            {
                caller = this,
                healthRegen = state.HPRegen,
                manaRegen = state.MPRegen,
            };
            TriggerEvent(regenEvt);
            SetUnitState(wc3agent, UNIT_STATE_LIFE, GetUnitState(this, UNIT_STATE_LIFE) + regenEvt.healthRegen * delta * (1 + _stats.incomingHealing));
            SetUnitState(wc3agent, UNIT_STATE_MANA, GetUnitState(this, UNIT_STATE_MANA) + regenEvt.manaRegen * delta * (1 + _stats.incomingMana));
        }
    }
}