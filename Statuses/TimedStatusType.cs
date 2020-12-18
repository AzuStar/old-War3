using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven.Statuses
{
    public class TimedStatusType : PermanentStackingStatusType
    {
        public static TimedStatusType Stun = new TimedStatusType((status) =>
        {
            PauseUnit(status.Target, true);
        }, (status) =>
        {
            PauseUnit(status.Target, false);
        }, null, @"Abilities\Spells\Human\Thunderclap\ThunderclapTarget.mdl", @"overhead");
        public static TimedStatusType Slow = new TimedStatusType((status) =>
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


        public TimedStatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, string specialEffectPath, string specialEffectAttachmentPoint) : base(apply, reset, onRemove, specialEffectPath, specialEffectAttachmentPoint)
        {
        }

        private new Status ApplyStatus(NoxUnit source, NoxUnit target, int level){
            return null;
        }

        public virtual Status ApplyStatus(NoxUnit source, NoxUnit target, int level, float duration)
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, 0, 0, duration, 0, false, false, false));
            return target.GetStatus(Id).Reapply(0, 0, 0);
        }

    }
}