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
        // index it yourself, sorry(

        public delegate void OnHitCallback(UnitEntity source, UnitEntity target, OnHit data);
        public readonly bool Epic;
        public readonly OnHitCallback Callback;

        public OnHitType(bool epic, OnHitCallback callback)
        {
            Callback = callback;
            Epic = epic;
        }
    }
}
