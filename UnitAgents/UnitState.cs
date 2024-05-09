using System;
using System.Collections.Generic;
using System.Diagnostics;
using NoxRaven.Data;

using static NoxRaven.UnitAgents.EUnitState;

namespace NoxRaven.UnitAgents
{
    public sealed class UnitState : NData
    {
        #region Data
        // default dependencies for all units
        public static List<NDataDependency> dependencies = new List<NDataDependency>()
        {
            // MAX_HP = BASE_HP * (1 + BASE_HP_PERCENT_BASE) + BONUS_HP
            new NDataDependency(MAX_HP, BASE_HP, EArithmetic.SET),
            new NDataDependency(MAX_HP, BASE_HP_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(MAX_HP, BONUS_HP_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(MAX_HP, BONUS_HP, EArithmetic.ADD),
            new NDataDependency(MAX_HP, BONUS_HP_PERCENT_TOTAL, EArithmetic.PERCENT_MULTIPLY),

            // MAX_MP = BASE_MP * (1 + BASE_MP_PERCENT_BASE) + BONUS_MP
            new NDataDependency(MAX_MP, BASE_MP, EArithmetic.SET),
            new NDataDependency(MAX_MP, BASE_MP_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(MAX_MP, BONUS_MP_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(MAX_MP, BONUS_MP, EArithmetic.ADD),
            new NDataDependency(MAX_MP, BONUS_MP_PERCENT_TOTAL, EArithmetic.PERCENT_MULTIPLY),

            // GREY_ATK = BASE_ATK * (1 + BASE_ATK_PERCENT_BASE)
            new NDataDependency(GREY_ATK, BASE_ATK, EArithmetic.SET),
            new NDataDependency(GREY_ATK, BASE_ATK_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            // GREEN_ATK = (GREY_ATK * (1 + BONUS_ATK_PERCENT_BASE) + BONUS_ATK) * (1 + BONUS_ATK_PERCENT_TOTAL) - GREY_ATK
            new NDataDependency(GREEN_ATK, GREY_ATK, EArithmetic.SET),
            new NDataDependency(GREEN_ATK, BONUS_ATK_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_ATK, BONUS_ATK, EArithmetic.ADD),
            new NDataDependency(GREEN_ATK, BONUS_ATK_PERCENT_TOTAL, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_ATK, GREY_ATK, EArithmetic.SUBTRACT),
            new NDataDependency(ATK, GREY_ATK, EArithmetic.SET),
            new NDataDependency(ATK, GREEN_ATK, EArithmetic.ADD),

            // GREY_AP = BASE_AP * (1 + BASE_AP_PERCENT_BASE)
            new NDataDependency(GREY_AP, BASE_AP, EArithmetic.SET),
            new NDataDependency(GREY_AP, BASE_AP_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            // GREEN_AP = (GREY_AP * (1 + BONUS_AP_PERCENT_BASE) + BONUS_AP) * (1 + BONUS_AP_PERCENT_TOTAL) - GREY_AP
            new NDataDependency(GREEN_AP, GREY_AP, EArithmetic.SET),
            new NDataDependency(GREEN_AP, BONUS_AP_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_AP, BONUS_AP, EArithmetic.ADD),
            new NDataDependency(GREEN_AP, BONUS_AP_PERCENT_TOTAL, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_AP, GREY_AP, EArithmetic.SUBTRACT),
            new NDataDependency(AP, GREY_AP, EArithmetic.SET),
            new NDataDependency(AP, GREEN_AP, EArithmetic.ADD),


            // RLD = (RELOAD_TIME/(1+ATTACK_SPEED))
            new NDataDependency(RELOAD_TIME, BASE_RELOAD_TIME, EArithmetic.SET),
            new NDataDependency(RELOAD_TIME, ATTACK_SPEED, EArithmetic.PERCENT_DIVIDE),

            // ARMOR
            new NDataDependency(GREY_ARMOR, BASE_ARMOR, EArithmetic.SET),
            new NDataDependency(GREY_ARMOR, BASE_ARMOR_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),

            new NDataDependency(GREEN_ARMOR, GREY_ARMOR, EArithmetic.SET),
            new NDataDependency(GREEN_ARMOR, BONUS_ARMOR_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_ARMOR, BONUS_ARMOR, EArithmetic.ADD),
            new NDataDependency(GREEN_ARMOR, BONUS_ARMOR_PERCENT_TOTAL, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_ARMOR, GREY_ARMOR, EArithmetic.SUBTRACT),
            new NDataDependency(ARMOR, GREY_ARMOR, EArithmetic.SET),
            new NDataDependency(ARMOR, GREEN_ARMOR, EArithmetic.ADD),

            // RESISTANCE
            new NDataDependency(GREY_MAGIC_RESIST, BASE_MAGIC_RESIST, EArithmetic.SET),
            new NDataDependency(GREY_MAGIC_RESIST, BASE_MAGIC_RESIST_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),

            new NDataDependency(GREEN_MAGIC_RESIST, GREY_MAGIC_RESIST, EArithmetic.SET),
            new NDataDependency(GREEN_MAGIC_RESIST, BONUS_MAGIC_RESIST_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_MAGIC_RESIST, BONUS_MAGIC_RESIST, EArithmetic.ADD),
            new NDataDependency(GREEN_MAGIC_RESIST, BONUS_MAGIC_RESIST_PERCENT_TOTAL, EArithmetic.PERCENT_MULTIPLY),
            new NDataDependency(GREEN_MAGIC_RESIST, GREY_MAGIC_RESIST, EArithmetic.SUBTRACT),
            new NDataDependency(MAGIC_RESIST, GREY_MAGIC_RESIST, EArithmetic.SET),
            new NDataDependency(MAGIC_RESIST, GREEN_MAGIC_RESIST, EArithmetic.ADD),

            new NDataDependency(ATTACK_RANGE, BASE_ATTACK_RANGE, EArithmetic.SET),

            new NDataDependency(HP_REG, BASE_HP_REGEN, EArithmetic.SET),
            new NDataDependency(HP_REG, BASE_HP_REGEN_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),

            new NDataDependency(MP_REG, BASE_MP_REGEN, EArithmetic.SET),
            new NDataDependency(MP_REG, BASE_MP_REGEN_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),

            new NDataDependency(MOVEMENT_SPEED, BASE_MOVE_SPEED, EArithmetic.SET),
            new NDataDependency(MOVEMENT_SPEED, BASE_MOVE_SPEED_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),

            new NDataDependency(ABSORBTION, BASE_ABSORB, EArithmetic.SET),
            new NDataDependency(ABSORBTION, BASE_ABSORB_PERCENT_BASE, EArithmetic.PERCENT_MULTIPLY),
        };
        public override List<NDataDependency> defaultDependencies => dependencies;
        #endregion

        #region Constructors
        #endregion

        #region Methods
        public void SetMax(EUnitState statId, EUnitState maxId)
        {
            Set(statId, this[maxId]);
        }
        #endregion

    }
}