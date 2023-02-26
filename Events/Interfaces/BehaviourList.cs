using System;
using System.Collections.Generic;
using System.Linq;

namespace NoxRaven.Events
{
    public sealed class BehaviourList<T> where T : EventArgs
    {
        private Dictionary<int, List<IBehaviour>> _list = new Dictionary<int, List<IBehaviour>>();

        //?
        public BehaviourList() { }

        public void Add(IBehaviour obj)
        {
            List<IBehaviour> list;
            if (!_list.TryGetValue((int)obj.priority, out list))
            {
                list = new List<IBehaviour>();
                _list.Add((int)obj.priority, list);
            }
            list.Add(obj);
        }

        public void Remove(IBehaviour obj)
        {
            List<IBehaviour> list;
            if (!_list.TryGetValue((int)obj.priority, out list)) return;

            list.Remove(obj);
            if (list.Count == 0)
                _list.Remove((int)obj.priority);

        }

        public void InvokeBehaviours(T args)
        {
            IEnumerable<int> sortedKeys = _list.Keys.OrderBy(key => key);
            foreach (int key in sortedKeys)
            {
                foreach (IBehaviour behaviour in _list[key])
                {
                    behaviour.GenericsInvoke(args);
                }
            }
        }

        public void Clear()
        {
            foreach (List<IBehaviour> list in _list.Values)
                list.Clear();
        }
    }
}