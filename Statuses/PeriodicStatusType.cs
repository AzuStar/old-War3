using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven.Statuses
{
    class PeriodicStatusType : SimpleStatusType
    {
        public float PeriodicTimeoutTime;

        public PeriodicStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, float periodicTimeoutTime, string specialEffectPath, string specialEffectAttachmentPoint) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
            PeriodicTimeoutTime = periodicTimeoutTime;
        }

        public PeriodicStatusType(int id, StatusFunction apply, StatusFunction onRemove, StatusFunction reset, float periodicTimeoutTime, string specialEffectPath, string specialEffectAttachmentPoint) : base(id, apply, onRemove, reset, specialEffectPath, specialEffectAttachmentPoint)
        {
            PeriodicTimeoutTime = periodicTimeoutTime;
        }

        public override Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            int resultId = Id;
            if (Id > 100)
                resultId = (Id - 101) * 100 + 101 + GetPlayerId(GetOwningPlayer(source.UnitRef));
            if (!target.ContainsStatus(resultId))
                // create new status and add it to unit
                return target.AddStatus(resultId, new Status(resultId, this, source, target, level, 0, 0, duration, PeriodicTimeoutTime, false, false));
            return target.GetStatus(resultId).Reapply(duration, level, 0);
        }

    }
}
