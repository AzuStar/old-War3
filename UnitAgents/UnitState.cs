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
        #endregion

        #region Constructors
        public UnitState()
        {
            // SetStateChain(GREY_ATK, BASE_ATK);
            AddChainStack(GREY_ATK, BASE_ATK, (val, chain) => chain);
            AddChainStack(GREY_ATK, BASE_ATK_PERCENT_BASE, (val, chain) => val * (1 + chain));

            AddChainStack(GREEN_ATK, GREY_ATK, (val, chain) => chain);
            AddChainStack(GREEN_ATK, BONUS_ATK_PERCENT_BASE, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_ATK, BONUS_ATK, (val, chain) => val + chain);
            AddChainStack(GREEN_ATK, BONUS_ATK_PERCENT_TOTAL, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_ATK, GREY_ATK, (val, chain) => val - chain);

            // chainstack for AP
            AddChainStack(GREY_AP, BASE_AP, (val, chain) => chain);
            AddChainStack(GREY_AP, BASE_AP_PERCENT_BASE, (val, chain) => val * (1 + chain));
            
            AddChainStack(GREEN_AP, GREY_AP, (val, chain) => chain);
            AddChainStack(GREEN_AP, BONUS_AP_PERCENT_BASE, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_AP, BONUS_AP, (val, chain) => val + chain);
            AddChainStack(GREEN_AP, BONUS_AP_PERCENT_TOTAL, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_AP, GREY_AP, (val, chain) => val - chain);

            // chainstack for ARMOR
            AddChainStack(GREY_ARM, BASE_ARMOR, (val, chain) => chain);
            AddChainStack(GREY_ARM, BASE_ARMOR_PERCENT_BASE, (val, chain) => val * (1 + chain));

            AddChainStack(GREEN_ARM, GREY_ARM, (val, chain) => chain);
            AddChainStack(GREEN_ARM, BONUS_ARMOR_PERCENT_BASE, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_ARM, BONUS_ARMOR, (val, chain) => val + chain);
            AddChainStack(GREEN_ARM, BONUS_ARMOR_PERCENT_TOTAL, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_ARM, GREY_ARM, (val, chain) => val - chain);

            // chainstack for RESIST
            AddChainStack(GREY_RES, BASE_MAGIC_RESIST, (val, chain) => chain);
            AddChainStack(GREY_RES, BASE_MAGIC_RESIST_PERCENT_BASE, (val, chain) => val * (1 + chain));

            AddChainStack(GREEN_RES, GREY_RES, (val, chain) => chain);
            AddChainStack(GREEN_RES, BONUS_MAGIC_RESIST_PERCENT_BASE, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_RES, BONUS_MAGIC_RESIST, (val, chain) => val + chain);
            AddChainStack(GREEN_RES, BONUS_MAGIC_RESIST_PERCENT_TOTAL, (val, chain) => val * (1 + chain));
            AddChainStack(GREEN_RES, GREY_RES, (val, chain) => val - chain);

            // chainstack for HP
            AddChainStack(MAX_HP, BASE_HP, (val, chain) => chain);
            AddChainStack(MAX_HP, BASE_HP_PERCENT_BASE, (val, chain) => val * (1 + chain));
            AddChainStack(MAX_HP, BONUS_HP_PERCENT_BASE, (val, chain) => val + chain);
            AddChainStack(MAX_HP, BONUS_HP, (val, chain) => val + chain);
            AddChainStack(MAX_HP, BONUS_HP_PERCENT_TOTAL, (val, chain) => val * (1 + chain));

            AddChainStack(MAX_MP, BASE_MP, (val, chain) => chain);
            AddChainStack(MAX_MP, BASE_MP_PERCENT_BASE, (val, chain) => val * (1 + chain));
            AddChainStack(MAX_MP, BONUS_MP_PERCENT_BASE, (val, chain) => val + chain);
            AddChainStack(MAX_MP, BONUS_MP, (val, chain) => val + chain);
            AddChainStack(MAX_MP, BONUS_MP_PERCENT_TOTAL, (val, chain) => val * (1 + chain));

            AddChainStack(ABS, BASE_ABSORB, (val, chain) => chain);
            AddChainStack(ABS, BASE_ABSORB_PERCENT_BASE, (val, chain) => val * (1 + chain));

            AddChainStack(UNIT_MS, BASE_MOVE_SPEED, (val, chain) => chain);
            AddChainStack(UNIT_MS, BASE_MOVE_SPEED_PERCENT_BASE, (val, chain) => val * (1 + chain));

            AddChainStack(RLD, RELOAD_TIME, (val, chain) => chain);
            AddChainStack(RLD, ATTACK_SPEED, (val, chain) => val / (1 + chain));

            AddChainStack(RNG, BASE_ATTACK_RANGE, (val, chain) => chain);

            AddChainStack(HP_REG, BASE_HP_REGEN, (val, chain) => chain);
            AddChainStack(HP_REG, BASE_HP_REGEN_PERCENT_BASE, (val, chain) => val * (1 + chain));

            AddChainStack(MP_REG, BASE_MP_REGEN, (val, chain) => chain);
            AddChainStack(MP_REG, BASE_MP_REGEN_PERCENT_BASE, (val, chain) => val * (1 + chain));

            // // recompute those states
            RecomputeStack((int)GREY_ATK);
            RecomputeStack((int)GREY_AP);
            RecomputeStack((int)GREY_ARM);
            RecomputeStack((int)GREY_RES);
            RecomputeStack((int)MAX_HP);
            RecomputeStack((int)MAX_MP);
            RecomputeStack((int)ABS);
            RecomputeStack((int)UNIT_MS);
            RecomputeStack((int)RLD);
            RecomputeStack((int)RNG);
            RecomputeStack((int)HP_REG);
            RecomputeStack((int)MP_REG);
        }


        #endregion

        #region Methods
        public void AddChainStack(EUnitState target, EUnitState chain, NData.ChainFunction arithmetic)
        {
            AddChainStack((int)target, (int)chain, arithmetic);
        }
        public void AddListener(EUnitState id, StateChangeHandler callback)
        {
            AddListener((int)id, callback);
        }
        public void RemoveListener(EUnitState id, StateChangeHandler callback)
        {
            RemoveListener((int)id, callback);
        }
        public void SetMax(EUnitState statId, EUnitState maxId)
        {
            Set(statId, this[maxId]);
        }
        #endregion

    }
}