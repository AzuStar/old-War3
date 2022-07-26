using System;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.UnitAgents;

namespace NoxRaven.Units
{
    // protected and virtual fields/methods
    public partial class NoxUnit
    {
        protected NoxUnit(unit u, Stats initialStats = null)
        {
            _self_ = u;
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
        }

        protected virtual void RecalculateStats(Stats myStats)
        {
            BlzSetUnitAttackCooldown(_self_, myStats.baseAttackCooldown / (1 + myStats.attackSpeed), 0);
            SetUnitMoveSpeed(_self_, myStats.baseMS * (1 + myStats.baseMSPercent));
            BlzSetUnitMaxHP(this, R2I(((myStats.baseHP * (1 + myStats.baseHPPercent)) * (1 + myStats.baseHPPercentBonus) + myStats.bonusHP) * (1 + myStats.totalHPPercent) + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
            BlzSetUnitMaxMana(this, R2I(((myStats.baseMP * (1 + myStats.baseMPPercent)) * (1 + myStats.baseMPPercentBonus) + myStats.bonusMP) * (1 + myStats.totalMPPercent) + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues

            DMG = (myStats.baseDMG * (1 + myStats.baseDMGPercent) * (1 + myStats.baseDMGPercentBonus) + myStats.bonusDMG) * (1 + myStats.totalDMGPercent);
            AP = (myStats.baseAP * (1 + myStats.baseAPPercent) * (1 + myStats.baseAPPercentBonus) + myStats.bonusAP) * (1 + myStats.totalAPPercent);
            ARM = (myStats.baseARM * (1 + myStats.baseARMPercent) * (1 + myStats.baseARMPercentBonus) + myStats.bonusARM) * (1 + myStats.totalARMPercent);
            MR = (myStats.baseMR * (1 + myStats.baseMRPercent) * (1 + myStats.baseMRPercentBonus) + myStats.bonusMR) * (1 + myStats.totalMRPercent);
        }

        protected virtual void Regenerate(float delta)
        {
            SetUnitState(_self_, UNIT_STATE_LIFE, GetUnitState(this, UNIT_STATE_LIFE) + (_stats.regenHP * (1 + _stats.regenHPPercent) * delta));
            SetUnitState(_self_, UNIT_STATE_MANA, GetUnitState(this, UNIT_STATE_MANA) + (_stats.regenMP * (1 + _stats.regenMPPercent) * delta));
        }
    }
}