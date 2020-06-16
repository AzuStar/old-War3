using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Units;
using static War3Api.Common;

namespace NoxRaven
{
    public class OnHitType
    {
        //public static Dictionary<int, OnHitType> Indexer = new Dictionary<int, OnHitType>();
        // index it yourself, sorry :(
        public static int Count = 0;

        public int Id;
        public delegate void OnHitCallback(NoxUnit source, NoxUnit target, float damage, float processedDamage, OnHit data);
        public readonly bool Epic; // only single instance of onhit per unit allowed
        public readonly OnHitCallback Callback;
        public readonly float Chance = 0;

        public OnHitType(bool epic, float chance, OnHitCallback callback)
        {
            Id = Count++;
            Chance = chance;
            Callback = callback;
            Epic = epic;
        }

        public void RegisterOnHit(NoxUnit whatUnit)
        {
            if (whatUnit.ContainsOnHit(Id))
                whatUnit.GetOnHit(Id).Count++;
            else
                whatUnit.AddOnHit(Id, new OnHit(this));
        }

        public bool ContainsOnHit(NoxUnit whatUnit)
        {
            return whatUnit.ContainsOnHit(Id);
        }

        public OnHit GetOnHit(NoxUnit whatUnit)
        {
            return whatUnit.GetOnHit(Id);
        }

        public void UnregisterOnHit(NoxUnit whatUnit)
        {
            if (whatUnit.ContainsOnHit(Id))
            {
                OnHit hit = whatUnit.GetOnHit(Id);
                hit.Count--;
                if (hit.Count == 0) whatUnit.RemoveOnHit(Id);
            }
        }
    }
}
