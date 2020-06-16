using NoxRaven.Units;
using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven.Statuses
{
    public class StackingStatusType : SimpleStatusType
    {
        public StackingStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, string specialEffectPath, string specialEffectAttachmentPoint) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
        }

        public StackingStatusType(int id, StatusFunction apply, StatusFunction onRemove, StatusFunction reset, string specialEffectPath, string specialEffectAttachmentPoint) : base(id, apply, onRemove, reset, specialEffectPath, specialEffectAttachmentPoint)
        {
        }

        public Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration, int initialStacks, int limitStacks, int applyStacks)
        {
            int resultId = Id;
            if (Id > 100)
                resultId = (Id - 101) * 100 + 101 + GetPlayerId(GetOwningPlayer(source.UnitRef));
            if (!target.ContainsStatus(resultId))
                // create new status and add it to unit
                return target.AddStatus(resultId, new Status(resultId, this, source, target, level, initialStacks, limitStacks, duration, 0, false, false));
            return target.GetStatus(resultId).Reapply(duration, level, applyStacks);
        }

        private new Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            return null;
        }

    }
}
