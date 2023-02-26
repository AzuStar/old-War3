using System;
using System.Collections.Generic;

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
        private Dictionary<NValue, Func<float, float, float>> _valueStacks = new Dictionary<NValue, Func<float, float, float>>();
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
        public void AddValueStack(NValue stackingValue, Func<float, float, float> stackingFunc)
        {
            _valueStacks.Add(stackingValue, stackingFunc);
            stackingValue.AddListener((prev, next) =>
            {
                RecomputeStack();
                return next;
            });
        }
        public void RecomputeStack()
        {
            if(_valueStacks.Count == 0)
            {
                return;
            }
            float multiple = 0;//_valueStacks[0].value;
            for (int i = 1; i < _valueStacks.Count; i++)
            {
                NValue val = _valueStacks[i];
                multiple *= (1 + val.value);
            }
            Set(multiple);
        }
        #endregion
    }
}