using System;
using System.Collections.Generic;

namespace NoxRaven.Events
{
    public sealed class BehaviourList<T> where T : EventArgs
    {
        private SortedList<int, List<IPriorityBehaviour>> _list = new SortedList<int, List<IPriorityBehaviour>>();

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
            foreach (KeyValuePair<int, List<IPriorityBehaviour>> list in _list)
            {
                foreach (IPriorityBehaviour behaviour in list.Value)
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