using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven.Statuses
{
    public class TimedStatusType : SimpleStatusType
    {
        public TimedStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, string specialEffectPath, string specialEffectAttachmentPoint) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
        }

        public virtual Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, 0, 0, duration, 0, false, false, true));
            return target.GetStatus(Id).Reapply(0, level, 0);
        }

    }
}