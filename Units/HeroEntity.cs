using System;
using System.Collections.Generic;
using System.Text;
using War3Api;
using static War3Api.Common;

namespace War3.NoxRaven.Units
{
    public class HeroEntity : UnitEntity
    {
        // +xx stat
        public int AddedStr;
        public int AddedAgi;
        public int AddedInt;

        private float PercentageStrBonus;
        private float PercentageAgiBonus;
        private float PercentageIntBonus;
        private float ExpBonus;
        private float ExpCache;
        public HeroEntity(Common.unit u) : base(u)
        {
        }

        public new static HeroEntity Cast(unit u)
        {
            return (HeroEntity)Indexer[u];
        }
    }
}
