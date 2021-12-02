using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven.Statuses
{
    public class PeriodicType : TimedType
    {
        public float PeriodicTimeoutTime;
        // public bool PeriodicEffect;

        public PeriodicType(StatusFunction apply, StatusFunction onRemove, string specialEffectPath, string specialEffectAttachmentPoint, float periodicTimeoutTime = 1) : base(apply, null, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
            // PeriodicEffect = periodicEffect;
            PeriodicTimeoutTime = periodicTimeoutTime;
        }

        //public Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration, int bonusLevel = 0, float bonusDuration = 0, )
        //{
        //    if (!target.ContainsStatus(Id))
        //        // create new status and add it to unit
        //        return target.AddStatus(Id, new Status(Id, this, source, target, level, duration));
        //    return target.GetStatus(Id).Reapply(bonusDuration, bonusLevel, 0);
        //}

//        [Obsolete]
//#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration, int bonusLevel = 0, float bonusDuration = 0)
//#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, duration));
            return target.GetStatus(Id).Reapply(bonusDuration, bonusLevel, 0);
        }

    }
}
