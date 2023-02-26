using System;
using System.Collections.Generic;
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
            SetStateChain(ATK, BASE_ATK);
            AddChainStack(BASE_ATK, BASE_ATK_PERCENT_BASE, (val, chain) => val * chain);
            AddChainStack(BASE_ATK

            AddValueStack(DEF, BASE_DEFENCE);
            AddValueStack(MAX_HP, BASE_HP);
            AddValueStack(HP_REG, BASE_HP_REGEN);
            AddValueStack(ABS, BASE_ABSOLUTE_DEFENCE);
            AddValueStack(MS, BASE_MOVE_SPEED);
            AddValueStack(RNG, BASE_ATTACK_RANGE);
            AddValueStack(RLD, BASE_RELOAD_TIME);
            // recompute those states
            RecomputeStack((int)ATK);
            RecomputeStack((int)DEF);
            RecomputeStack((int)MAX_HP);
            RecomputeStack((int)HP_REG);
            RecomputeStack((int)ABS);
            RecomputeStack((int)MS);
            RecomputeStack((int)RNG);
            RecomputeStack((int)RLD);
        }

        private void AddChainStack(EUnitState target, EUnitState chain, Func<float, float, float> value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Methods
        public void AddValueStack(EUnitState targetId, EUnitState stackingStateId)
        {
            AddValueStack((int)targetId, (int)stackingStateId);
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