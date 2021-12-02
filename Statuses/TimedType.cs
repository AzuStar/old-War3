using System;
using System.Collections.Generic;

using static War3Api.Common;

namespace NoxRaven.Statuses
{
    public class TimedType
    {
        public static TimedType Pause = new TimedType((status) =>
        {
            PauseUnit(status.Target, true);
        }, (status) =>
        {
            if (!status.Target.ContainsStatus(Stun.Id))
                PauseUnit(status.Target, false);
        }, null, null, null);
        public static TimedType Stun = new TimedType((status) =>
        {
            PauseUnit(status.Target, true);
        }, (status) =>
        {
            if (!status.Target.ContainsStatus(Pause.Id))
                PauseUnit(status.Target, false);
        }, null, @"Abilities\Spells\Human\Thunderclap\ThunderclapTarget.mdl", @"overhead");

        public static TimedType Slow = new TimedType((status) =>
        {
            // -1% ms per level
            status.Target.AddPercentMovementSpeed(-(float)status.Level / 100);
            //SetUnitVertexColor(status.Target, 55 + R2I(RMaxBJ(100 - status.Level * 2, 0)), 55 + R2I(RMaxBJ(100 - status.Level * 2, 0)), 255, 255);
        }, (status) =>
        {
            // +1% ms per level
            status.Target.AddPercentMovementSpeed((float)status.Level / 100);
            //SetUnitVertexColor(status.Target, 255, 255, 255, 255);
        }, null, null, null);

        public delegate void StatusFunction(Status status);
        public readonly int Id;
        public static int Count;
        public String Effectpath;
        public String Attachment;
        internal StatusFunction Apply { get; private set; }
        internal StatusFunction Reset { get; private set; }
        internal StatusFunction OnRemove { get; private set; }
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
        public TimedType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, String specialEffectPath, String specialEffectAttachmentPoint)
        {
            Id = ++Count;
            Apply = apply;
            Reset = reset;
            OnRemove = onRemove;
            Effectpath = specialEffectPath;
            Attachment = specialEffectAttachmentPoint;
        }

        public Status GetStatus(NoxUnit unit) => unit.GetStatus(Id);

        public virtual Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration, int bonusLevel = 0, float bonusDuration = 0)
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, duration));
            return target.GetStatus(Id).Reapply(bonusDuration, bonusLevel, 0);
        }

        public virtual void DispelStatus(NoxUnit unit)
        {
            if (unit.ContainsStatus(Id))
                unit.GetStatus(Id).Remove();
        }
    }
}
