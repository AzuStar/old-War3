using System;
using System.Collections.Generic;
using System.Text;
using War3Api;
using static War3Api.Common;

namespace War3.NoxRaven.Units
{
    public class BuildingEntity : UnitEntity
    {
        public BuildingEntity(unit u) : base(u)
        {
        }

        public new static BuildingEntity Cast(unit u)
        {
            return (BuildingEntity)Indexer[u];
        }

        public override float WeapondDamage()
        {
            Utils.DisplayMessageToEveryone("Overriden damage", 20);
            return base.WeapondDamage();
        }
        protected override void DeattachClass()
        {
            base.DeattachClass();
            // extra cleanup
        }
    }
}
