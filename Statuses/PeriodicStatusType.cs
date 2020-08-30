using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven.Statuses
{
    public class PeriodicStatusType : TimedStatusType
    {
        public float PeriodicTimeoutTime;

        public PeriodicStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, string specialEffectPath, string specialEffectAttachmentPoint, float periodicTimeoutTime = 1) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
            PeriodicTimeoutTime = periodicTimeoutTime;
        }

        public override Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, 0, 0, duration, PeriodicTimeoutTime, false, false, false));
            return target.GetStatus(Id).Reapply(duration, level, 0);
        }

    }
}
