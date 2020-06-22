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
        public readonly int StackLimit;
        public readonly int InitialStacks;
        public StackingStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, string specialEffectAttachmentPoint, string specialEffectPath, int stackLimit = 0, int initialStacks = 1) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
            StackLimit = stackLimit;
            InitialStacks = initialStacks;
        }

        public StackingStatusType(int id, StatusFunction apply, StatusFunction onRemove, StatusFunction reset, string specialEffectPath, string specialEffectAttachmentPoint, int stackLimit = 0, int initialStacks = 1) : base(id, apply, onRemove, reset, specialEffectPath, specialEffectAttachmentPoint)
        {
            StackLimit = stackLimit;
            InitialStacks = initialStacks;
        }

        public Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration, int applyStacks)
        {
            int resultId = Id;
            if (Id > 100)
                resultId = (Id - 101) * 100 + 101 + GetPlayerId(GetOwningPlayer(source.UnitRef));
            if (!target.ContainsStatus(resultId))
                // create new status and add it to unit
                return target.AddStatus(resultId, new Status(resultId, this, source, target, level, InitialStacks, StackLimit, duration, 0, false, false));
            return target.GetStatus(resultId).Reapply(duration, level, applyStacks);
        }

        private new Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            return null;
        }

    }
}
