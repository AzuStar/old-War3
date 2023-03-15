using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NoxRaven.Data
{
    public abstract class NData : ICloneable
    {
        #region Static
        public delegate float StateChangeHandler(float prev, float cur);
        public delegate float ChainFunction(float value, float chain);
        /// <summary>
        /// Convert a 4-character string to a 32-bit integer, use exactly 4 character. Always. Only.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FourCC(string s)
        {
            return s[0] | (s[1] << 8) | (s[2] << 16) | (s[3] << 24);
        }
        #endregion
        #region Data
        public string classCode => GetType().Name;

        protected Dictionary<int, NValue> _data = new Dictionary<int, NValue>();
        #endregion

        public NData() { }

        #region Methods
        public object Clone()
        {
            NData clone = (NData)MemberwiseClone();
            clone._data = new Dictionary<int, NValue>(_data);
            return clone;
        }
        public void ApplyModifier(NDataModifier modifier)
        {
            Dictionary<int, float> enumerator = modifier.GetDictionary();
            foreach (KeyValuePair<int, float> kv in enumerator)
            {
                this[kv.Key] += kv.Value;
            }
        }
        public void UnapplyModifier(NDataModifier modifier)
        {
            Dictionary<int, float> enumerator = modifier.GetDictionary();
            foreach (KeyValuePair<int, float> kv in enumerator)
            {
                this[kv.Key] -= kv.Value;
            }
        }
        public virtual void ReapplyModifier(NDataModifier modifier)
        {
            UnapplyModifier(modifier);
            ApplyModifier(modifier);
        }
        public void AddListener(int id, StateChangeHandler handler)
        {
            if (!_data.ContainsKey(id))
            {
                _data.Add(id, new NValue());
            }
            _data[id].AddListener(handler);
        }
        public void RemoveListener(int id, StateChangeHandler handler)
        {
            if (_data.TryGetValue(id, out NValue val))
            {
                val.RemoveListener(handler);
            }
        }
        public void AddChainStack(int targetId, int stackingValueId, ChainFunction arithmetic)
        {
            // contains
            if (!_data.ContainsKey(targetId))
            {
                _data.Add(targetId, new NValue());
            }
            if (!_data.ContainsKey(stackingValueId))
            {
                _data.Add(stackingValueId, new NValue());
            }
            _data[targetId].AddValueChain(_data[stackingValueId], arithmetic);
        }
        public void RecomputeStack(int targetId)
        {
            if (_data.ContainsKey(targetId))
            {
                _data[targetId].RecomputeStack();
            }
        }
        public void ResetFromModifier(NDataModifier modifier)
        {
            Dictionary<int, float> enumerator = modifier.GetDictionary();
            
            foreach (KeyValuePair<int, float> kv in enumerator)
            {
                this[kv.Key] = kv.Value;
            }
        }
        #endregion
        #region Getter/Setter
        public void Set(Enum id, float val)
        {
            Set(id.GetHashCode(), val);
        }
        public void Set(string id, float val)
        {
            Set(NData.FourCC(id), val);
        }
        public virtual void Set(int id, float val)
        {
            if (!_data.ContainsKey(id))
            {
                _data.Add(id, new NValue());
            }
            _data[id].value = val;
        }

        public float Get(Enum id)
        {
            return Get(id.GetHashCode());
        }
        public float Get(string id)
        {
            return Get(NData.FourCC(id));
        }
        public virtual float Get(int id)
        {
            if (_data.TryGetValue(id, out NValue val))
                return val.value;

            _data.Add(id, new NValue());
            return 0;
        }
        #endregion

        public float this[Enum id] { get => Get(id); set => Set(id, value); }
        public float this[string id] { get => Get(id); set => Set(id, value); }
        public float this[int id] { get => Get(id); set => Set(id, value); }

    }

}