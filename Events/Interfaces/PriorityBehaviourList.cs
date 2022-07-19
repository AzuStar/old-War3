using System;
using System.Collections.Generic;
using System.Linq;

namespace NoxRaven.Events
{
    public sealed class BehaviourList<T> where T : EventArgs
    {
        private Dictionary<int, List<IPriorityBehaviour>> _list = new Dictionary<int, List<IPriorityBehaviour>>();

        //?
        public BehaviourList() { }

        public void Add(IPriorityBehaviour obj)
        {
            List<IPriorityBehaviour> list;
            if (!_list.TryGetValue((int)obj.priority, out list))
            {
                list = new List<IPriorityBehaviour>();
                _list.Add((int)obj.priority, list);
            }
            list.Add(obj);
        }

        public void Remove(IPriorityBehaviour obj)
        {
            List<IPriorityBehaviour> list;
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
                foreach (IPriorityBehaviour behaviour in _list[key])
                {
                    behaviour.GenericsInvoke(args);
                }
            }
        }

        public void Clear()
        {
            foreach (List<IPriorityBehaviour> list in _list.Values)
                list.Clear();
        }
    }
}