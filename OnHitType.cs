using NoxRaven.Units;
using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;

namespace NoxRaven
{
    public class OnHitType
    {
        //public static Dictionary<int, OnHitType> Indexer = new Dictionary<int, OnHitType>();
        // index it yourself, sorry :(
        public static int s_count = 0;

        public int id;
        public delegate void OnHitCallback(NoxUnit source, NoxUnit target, float damage, float processedDamage, OnHit data);
        public readonly bool epic; // only single instance of onhit per unit allowed
        public readonly OnHitCallback callback;
        public readonly float chance = 0;

        public OnHitType(bool epic, float chance, OnHitCallback callback)
        {
            id = s_count++;
            this.chance = chance;
            this.callback = callback;
            this.epic = epic;
        }

        public void RegisterOnHit(NoxUnit whatUnit)
        {
            if (whatUnit.ContainsOnHit(id))
                whatUnit.GetOnHit(id).count++;
            else
                whatUnit.AddOnHit(id, new OnHit(this));
        }

        public bool ContainsOnHit(NoxUnit whatUnit)
        {
            return whatUnit.ContainsOnHit(id);
        }

        public OnHit GetOnHit(NoxUnit whatUnit)
        {
            return whatUnit.GetOnHit(id);
        }

        public void UnregisterOnHit(NoxUnit whatUnit)
        {
            if (whatUnit.ContainsOnHit(id))
            {
                OnHit hit = whatUnit.GetOnHit(id);
                hit.count--;
                if (hit.count == 0) whatUnit.RemoveOnHit(id);
            }
        }
    }
}
