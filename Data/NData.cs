using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NoxRaven.IO;
using War3Api;

namespace NoxRaven.Data
{
    [System.Serializable]
    public abstract class NData : ICloneable
    {
        #region Static
        public delegate void ListenerHandler(StatChange statChange);

        public sealed class StatChange
        {
            public float value;
            public float stackedValue;

            public StatChange(float val, float stack)
            {
                this.value = val;
                this.stackedValue = stack;
            }
        }
        #endregion
        #region Data
        public string classCode => GetType().Name;
        private Dictionary<int, _Data> _data = new Dictionary<int, _Data>();

        // CSharp.Lua bug (lags with abstract properties)
        public virtual List<NDataDependency> defaultDependencies => null;
        #endregion

        public NData()
        {
            if (defaultDependencies != null)
            {
                foreach (NDataDependency dependency in defaultDependencies)
                {
                    AddDependency(dependency);
                }
            }
        }

        public void AddDependency(NDataDependency dependency)
        {
            if (!_data.ContainsKey(dependency.targetStatId))
            {
                _data[dependency.targetStatId] = new _Data();
            }
            if (!_data.ContainsKey(dependency.sourceStatId))
            {
                _data[dependency.sourceStatId] = new _Data();
            }

            _data[dependency.targetStatId].dependencies.Add(dependency);
            _data[dependency.sourceStatId].dependants.Add(dependency.targetStatId);
            RecomputeStat(dependency.targetStatId, new HashSet<int>());
        }

        public void RemoveDependency(NDataDependency dependency)
        {
            _data[dependency.targetStatId].dependencies.Remove(dependency);
            _data[dependency.sourceStatId].dependants.Remove(dependency.targetStatId);
            RecomputeStat(dependency.targetStatId, new HashSet<int>());
        }

        public StatListener AddListener(Enum targetStatId, ListenerHandler callback)
        {
            return AddListener(Convert.ToInt32(targetStatId), callback);
        }

        // create a listener and call it once
        public StatListener AddListener(int targetStatId, ListenerHandler callback)
        {
            StatListener listener = new StatListener(this, targetStatId, callback);
            listener.OnStatChanged(new StatChange(Get(targetStatId), Get(targetStatId)));
            return listener;
        }

        internal void RegisterListener(int targetStatId, ListenerHandler listener)
        {
            if (!_data.ContainsKey(targetStatId))
            {
                _data[targetStatId] = new _Data();
            }

            _data[targetStatId].listeners.Add(listener);
        }

        internal void UnregisterListener(int targetStatId, ListenerHandler listener)
        {
            if (!_data.ContainsKey(targetStatId))
            {
                return;
            }

            _data[targetStatId].listeners.Remove(listener);
        }

        public void RecomputeStat(int targetId, HashSet<int> visitedIds)
        {
            if (!_data.ContainsKey(targetId) || visitedIds.Contains(targetId))
            {
                return;
            }
            visitedIds.Add(targetId);
            _Data targetValue = _data[targetId];

            // if (!targetValue.dependencies.Any())
            // {
            targetValue.stackedValue = targetValue.value;
            //     return;
            // }
            foreach (NDataDependency dependency in targetValue.dependencies)
            {
                _Data sourceValue = _data[dependency.sourceStatId];
                targetValue.ArithmeticOperation(dependency.arithmetic, sourceValue);
            }
            // call listeners
            foreach (ListenerHandler listener in targetValue.listeners)
            {
                StatChange statChange = new StatChange(targetValue.value, targetValue.stackedValue);
                listener(statChange);
                targetValue.value = statChange.value;
                targetValue.stackedValue = statChange.stackedValue;
            }
            // recompute states that depend on this stat
            foreach (int dependantId in targetValue.dependants)
            {
                RecomputeStat(dependantId, visitedIds);
            }
        }

        #region Methods
        public NDataModifier ToModifier()
        {
            NDataModifier modifier = new NDataModifier();
            foreach (KeyValuePair<int, _Data> kv in _data)
            {
                modifier[kv.Key] = kv.Value.value;
            }
            return modifier;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Serializer.SerializeObject(this._data, sb);
            return sb.ToString();
        }

        public object Clone()
        {
            NData clone = (NData)MemberwiseClone();
            clone._data = new Dictionary<int, _Data>(_data);
            return clone;
        }

        public void ApplyModifier(NDataModifier modifier)
        {
            IEnumerable<KeyValuePair<int, float>> enumerator = modifier.GetEnumerable();
            foreach (KeyValuePair<int, float> kv in enumerator)
            {
                this[kv.Key] += kv.Value;
            }
        }

        public void UnapplyModifier(NDataModifier modifier)
        {
            IEnumerable<KeyValuePair<int, float>> enumerator = modifier.GetEnumerable();
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

        public void ResetFromModifier(NDataModifier modifier)
        {
            IEnumerable<KeyValuePair<int, float>> enumerator = modifier.GetEnumerable();
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
            Set(Common.FourCC(id), val);
        }

        public virtual void Set(int id, float value)
        {
            if (!_data.ContainsKey(id))
            {
                _data[id] = new _Data();
            }

            _data[id].value = value;

            HashSet<int> visitedIds = new HashSet<int>();
            RecomputeStat(id, visitedIds);
        }

        public float Get(Enum id)
        {
            return Get(id.GetHashCode());
        }

        public float Get(string id)
        {
            return Get(Common.FourCC(id));
        }

        public virtual float Get(int id)
        {
            if (!_data.ContainsKey(id))
            {
                return 0;
            }

            return _data[id].stackedValue;
        }
        #endregion

        public float this[Enum id]
        {
            get => Get(id);
            set => Set(id, value);
        }
        public float this[string id]
        {
            get => Get(id);
            set => Set(id, value);
        }
        public float this[int id]
        {
            get => Get(id);
            set => Set(id, value);
        }

        internal class _Data
        {
            public int id;
            public float value;
            public float stackedValue;

            public List<ListenerHandler> listeners = new List<ListenerHandler>();
            public List<NDataDependency> dependencies = new List<NDataDependency>();
            public List<int> dependants = new List<int>();

            // Slightly different from FormulaBuilder due to custom stacking logic
            public void ArithmeticOperation(EArithmetic arithmetic, _Data sourceData)
            {
                switch (arithmetic)
                {
                    case EArithmetic.SET:
                        value = sourceData.stackedValue;
                        stackedValue = sourceData.stackedValue;
                        break;

                    // not normally needed, but just in case
                    case EArithmetic.ADD:
                        stackedValue = stackedValue + sourceData.stackedValue;
                        break;
                    // not normally needed, but just in case
                    case EArithmetic.SUBTRACT:
                        stackedValue = stackedValue - sourceData.stackedValue;
                        break;

                    case EArithmetic.MULTIPLY:
                        stackedValue = stackedValue * sourceData.stackedValue;
                        break;

                    case EArithmetic.DIVIDE:
                        stackedValue = stackedValue / sourceData.stackedValue;
                        break;

                    case EArithmetic.PERCENT_MULTIPLY:
                        stackedValue = stackedValue * (1.0f + sourceData.stackedValue);
                        break;

                    case EArithmetic.PERCENT_DIVIDE:
                        stackedValue = stackedValue / (1.0f + sourceData.stackedValue);
                        break;

                    case EArithmetic.TO_PERCENT_POWER:
                        stackedValue = Common.Pow(stackedValue + 1f, sourceData.stackedValue) - 1f;
                        break;

                    case EArithmetic.TO_POWER:
                        stackedValue = Common.Pow(stackedValue, sourceData.stackedValue);
                        break;

                    case EArithmetic.LIMIT_TO:
                        stackedValue =
                            sourceData.stackedValue
                            + (1 - sourceData.stackedValue) / (1 + stackedValue);
                        break;

                    case EArithmetic.MIN:
                        stackedValue = Math.Min(stackedValue, sourceData.stackedValue);
                        break;

                    case EArithmetic.MAX:
                        stackedValue = Math.Max(stackedValue, sourceData.stackedValue);
                        break;

                    case EArithmetic.LIMIT_BY:
                        stackedValue = (1 - sourceData.stackedValue) / (1 + (1 / stackedValue));
                        break;
                }
            }
        }
    }
}
