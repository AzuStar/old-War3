using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NoxRaven.Data
{
    [System.Serializable]
    public sealed class NDataModifier : ICloneable
    {
        #region Data
        private Dictionary<int, float> _data = new Dictionary<int, float>();
        #endregion

        public NDataModifier() { }

        #region Methods
        // Get and Set
        public NDataModifier MultiplyExisting(NDataModifier right)
        {
            NDataModifier newstats = (NDataModifier)Activator.CreateInstance(this.GetType());
            newstats._data = new Dictionary<int, float>(_data); // this will be existing
            foreach (int key in _data.Keys.Intersect(right._data.Keys).ToHashSet())
            {
                newstats.Set(key, Get(key) * right.Get(key));
            }
            return newstats;
        }
        public void Set(Enum id, float val)
        {
            Set(id.GetHashCode(), val);
        }
        public void Set(string id, float val)
        {
            Set(NData.FourCC(id), val);
        }
        public void Set(int id, float val)
        {
            _data[id] = val;

        }

        public float Get(Enum id)
        {
            return Get(id.GetHashCode());
        }
        public float Get(string id)
        {
            return Get(NData.FourCC(id));
        }
        public float Get(int id)
        {
            if (_data.TryGetValue(id, out float val))
                return val;

            _data.Add(id, 0);
            return 0;
        }
        public IEnumerable<KeyValuePair<int, float>> GetEnumerable()
        {
            return _data;
        }

        public object Clone()
        {
            NDataModifier clone = new NDataModifier();
            clone._data = new Dictionary<int, float>(_data);
            return clone;
        }
        #endregion

        #region Operators
        public static NDataModifier operator +(NDataModifier left, NDataModifier right)
        {
            NDataModifier newstats = (NDataModifier)Activator.CreateInstance(left.GetType());
            foreach (int key in left._data.Keys.Union(right._data.Keys).ToHashSet())
            {
                newstats.Set(key, left.Get(key) + right.Get(key));
            }
            return newstats;
        }

        public static NDataModifier operator -(NDataModifier left, NDataModifier right)
        {
            NDataModifier newstats = (NDataModifier)Activator.CreateInstance(left.GetType());
            foreach (int key in left._data.Keys.Union(right._data.Keys).ToHashSet())
            {
                newstats.Set(key, left.Get(key) - right.Get(key));
            }
            return newstats;
        }

        public static NDataModifier operator *(NDataModifier left, NDataModifier right)
        {
            NDataModifier newstats = (NDataModifier)Activator.CreateInstance(left.GetType());
            foreach (int key in left._data.Keys.Union(right._data.Keys).ToHashSet())
            {
                newstats.Set(key, left.Get(key) * right.Get(key));
            }
            return newstats;
        }

        public static NDataModifier operator /(NDataModifier left, NDataModifier right)
        {
            NDataModifier newstats = (NDataModifier)Activator.CreateInstance(left.GetType());
            foreach (int key in left._data.Keys.Union(right._data.Keys).ToHashSet())
            {
                newstats.Set(key, left.Get(key) / right.Get(key));
            }
            return newstats;
        }
        public float this[Enum id] { get => Get(id); set => Set(id, value); }
        public float this[string id] { get => Get(id); set => Set(id, value); }
        public float this[int id] { get => Get(id); set => Set(id, value); }
        #endregion
    }

}