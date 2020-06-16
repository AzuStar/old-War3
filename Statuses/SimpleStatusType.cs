using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven.Statuses
{
    public class SimpleStatusType
    {
        public delegate void StatusFunction(Status status);
        public static SimpleStatusType Stun = new SimpleStatusType(0, (status) =>
        {
            PauseUnit(status.Target, true);
        }, (status) =>
        {
            PauseUnit(status.Target, false);
        }, null, @"Abilities\Spells\Human\Thunderclap\ThunderclapTarget.mdl", @"overhead");
        public static SimpleStatusType Slow = new SimpleStatusType(1, (status) =>
        {
            status.Target.AddMoveSpeedPercent(-(float)status.Level / 100);
            SetUnitVertexColor(status.Target, 55 + R2I(RMaxBJ(100 - status.Level * 2, 0)), 55 + R2I(RMaxBJ(100 - status.Level * 2, 0)), 255, 255);
        }, (status) =>
        {
            status.Target.AddMoveSpeedPercent((float)status.Level / 100);
            SetUnitVertexColor(status.Target, 255, 255, 255, 255);
        }, null, null, null);
        public readonly int Id;
        public static int Count;
        public String Effectpath;
        public String Attachment;
        public StatusFunction Apply { get; private set; }
        public StatusFunction Reset { get; private set; }
        public StatusFunction OnRemove { get; private set; }
        public override int GetHashCode()
        {
            return Id;
        }
        /// <summary>
        /// Creates a StatusType. Delegates can be null.
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="reset"></param>
        /// <param name="specialEffectPath"></param>
        /// <param name="specialEffectAttachmentPoint"></param>
        public SimpleStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, String specialEffectPath, String specialEffectAttachmentPoint)
        {
            Id = 100 + ++Count;
            Apply = apply;
            Reset = reset;
            OnRemove = onRemove;
            Effectpath = specialEffectPath;
            Attachment = specialEffectAttachmentPoint;
        }
        /// <summary>
        /// Use this for reserved buffs, use ids 0-100
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apply"></param>
        /// <param name="reset"></param>
        /// <param name="specialEffectPath"></param>
        /// <param name="specialEffectAttachmentPoint"></param>
        public SimpleStatusType(int id, StatusFunction apply, StatusFunction onRemove, StatusFunction reset, String specialEffectPath, String specialEffectAttachmentPoint)
        {
            Id = id;
            Apply = apply;
            Reset = reset;
            OnRemove = onRemove;
            Effectpath = specialEffectPath;
            Attachment = specialEffectAttachmentPoint;
        }
        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        public virtual Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            //if (Utils.IsUnitDead(target)) return null;
            int resultId = Id;
            if (Id > 100)
                resultId = (Id - 101) * 100 + 101 + GetPlayerId(GetOwningPlayer(source.UnitRef));
            if (!target.ContainsStatus(resultId))
                // create new status and add it to unit
                return target.AddStatus(resultId, new Status(resultId, this, source, target, level, 0, 0, duration, 0, false, false));
            return target.GetStatus(resultId).Reapply(duration, level, 0);
        }

        public Status GetStatus(NoxUnit source, NoxUnit target)
        {
            int resultId = Id;
            if (Id > 100)
                resultId = (Id - 101) * 100 + 101 + GetPlayerId(GetOwningPlayer(source.UnitRef));
            return target.GetStatus(resultId);
        }

    }
}
