using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven.Statuses
{
    /// <summary>
    /// Simple permanent status
    /// </summary>
    public class SimpleStatusType
    {
        public delegate void StatusFunction(Status status);
        public static SimpleStatusType Stun = new SimpleStatusType((status) =>
        {
            PauseUnit(status.Target, true);
        }, (status) =>
        {
            PauseUnit(status.Target, false);
        }, null, @"Abilities\Spells\Human\Thunderclap\ThunderclapTarget.mdl", @"overhead");
        // public static SimpleStatusType Slow = new SimpleStatusType((status) =>
        // {
        //     status.Target.AddMoveSpeedPercent(-(float)status.Level / 100);
        //     SetUnitVertexColor(status.Target, 55 + R2I(RMaxBJ(100 - status.Level * 2, 0)), 55 + R2I(RMaxBJ(100 - status.Level * 2, 0)), 255, 255);
        // }, (status) =>
        // {
        //     status.Target.AddMoveSpeedPercent((float)status.Level / 100);
        //     SetUnitVertexColor(status.Target, 255, 255, 255, 255);
        // }, null, null, null);
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
            Id = ++Count;
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
        public virtual Status ApplyStatus(NoxUnit source, NoxUnit target, int level)
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, 0, 0, 0, 0, false, false, true));
            return target.GetStatus(Id).Reapply(0, level, 0);
        }

        public Status GetStatus(NoxUnit target)
        {
            return target.GetStatus(Id);
        }

    }
}
