using System;
using System.Collections.Generic;
using System.Linq;

namespace NoxRaven.Data
{
    [Serializable]
    // technically private type
    public class NValue
    {
        #region Data
        public float value { get => _value; set => Set(value); }
        private float _value = 0;
        private HashSet<NData.StateChangeHandler> _callbacks = new HashSet<NData.StateChangeHandler>();

        private List<ChainTuple> _valueChains = new List<ChainTuple>();
        #endregion

        #region Constructors
        public NValue()
        {
        }
        #endregion

        #region Methods
        public float Get()
        {
            return _value;
        }
        // use internally, as it needs to call callbacks
        public void Set(float val)
        {
            float prev = _value;
            _value = val;
            foreach (NData.StateChangeHandler callback in _callbacks)
            {
                _value = callback(prev, _value);
            }
        }
        public void AddListener(NData.StateChangeHandler callback)
        {
            _callbacks.Add(callback);
            callback(_value, _value);
        }
        public void RemoveListener(NData.StateChangeHandler callback)
        {
            _callbacks.Remove(callback);
        }
        public void AddValueChain(NValue stackingValue, NData.ChainFunction arithmetic)
        {
            _valueChains.Add(new ChainTuple()
            {
                stackingValue = stackingValue,
                arithmetic = arithmetic
            });
            stackingValue.AddListener((prev, cur) =>
            {
                RecomputeStack();
                return cur;
            });
        }
        public void RecomputeStack()
        {
            if (_valueChains.Count == 0)
            {
                return;
            }
            float multiple = 0;
            foreach (ChainTuple chain in _valueChains)
            {
                multiple = chain.arithmetic(multiple, chain.stackingValue._value);
            }
            Set(multiple);
        }
        #endregion

        private struct ChainTuple
        {
            public NValue stackingValue;
            public NData.ChainFunction arithmetic;
        }
    }

}