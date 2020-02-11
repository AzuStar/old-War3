using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Units;
using static War3Api.Common;

namespace NoxRaven
{
    public sealed class OnHit
    {
        public dynamic Data;
        private OnHitType Type;
        private float Chance;

        public OnHit(OnHitType type, float chance)
        {
            Type = type;
            Chance = chance;
        }
        /// <summary>
        /// Reverse the source and target to make it AmHit.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ApplyOnHit(NoxUnit source, NoxUnit target)
        {
            if (GetRandomReal(0, 1) < source.TriggerChance * Chance)
                for (int i = 0; i < source.OnHitApplied_Times; i++)
                    Type.Callback.Invoke(source, target, this);
        }
    }
}
