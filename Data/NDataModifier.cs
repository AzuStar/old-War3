using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NoxRaven.IO;
using War3Api;

namespace NoxRaven.Data
{
    [System.Serializable]
    public class NDataModifier : ICloneable
    {
        #region Data
        public static NDataModifier Default => new NDataModifier();
        private Dictionary<int, float> _data = new Dictionary<int, float>();
        #endregion

        public NDataModifier() { }

        #region Methods
        public bool IsEmpty
        {
            get => _data.Count == 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Serializer.SerializeObject(this._data, sb);
            return sb.ToString();
        }

        // Get and Set
        public NDataModifier MultiplyExisting(NDataModifier right)
        {
            if (right == null)
                return this;
            NDataModifier newstats = (NDataModifier)this.Clone();
            foreach (int key in _data.Keys.Intersect(right._data.Keys).ToHashSet())
            {
                newstats.Set(key, Get(key) * right.Get(key));
            }
            return newstats;
        }

        public void Set(Enum id, float val)
        {
            Set(Convert.ToInt32(id), val);
        }

        public void Set(string id, float val)
        {
            Set(Common.FourCC(id), val);
        }

        public void Set(int id, float val)
        {
            _data[id] = val;
        }

        public float Get(Enum id)
        {
            return Get(Convert.ToInt32(id));
        }

        public float Get(string id)
        {
            return Get(Common.FourCC(id));
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

        public static NDataModifier From(Dictionary<Enum, float> data)
        {
            NDataModifier modifier = new NDataModifier();
            foreach (KeyValuePair<Enum, float> kv in data)
            {
                modifier.Set(Convert.ToInt32(kv.Key), kv.Value);
            }
            return modifier;
        }

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
        #endregion
    }
}
