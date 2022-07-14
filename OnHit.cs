using NoxRaven.Units;
using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;

namespace NoxRaven
{
    public sealed class OnHit
    {
        public dynamic data;
        private OnHitType type;
        public int count = 0;

        internal OnHit(OnHitType type)
        {
            this.type = type;
        }
        /// <summary>
        /// Reverse the source and target to make it AmHit.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ApplyOnHit(NoxUnit source, NoxUnit target, float damage, float processedDamage)
        {
            if (GetRandomReal(0, 1) < source.lookuptriggerChance * type.chance)
                if (!type.epic)
                    for (int i = 0; i < count; i++)
                        type.callback.Invoke(source, target, damage, processedDamage, this);
                else type.callback.Invoke(source, target, damage, processedDamage, this);
        }
    }
}
