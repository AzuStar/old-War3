using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven.Statuses
{
    /// <summary>
    /// Simple permanent status
    /// </summary>
    public class PermanentStackingStatusType
    {
        // Example
        //public static SimpleStatusType DamageAbsorption = new SimpleStatusType(
        //    (status) =>
        //    {

        //    },
        //    (status) =>
        //    {

        //    },
        //    (status) =>
        //    {

        //    },
        //    null, null);

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        public Status ApplyStatus(NoxUnit source, NoxUnit target, int level)
        {
            if (!target.ContainsStatus(Id))
                // create new status and add it to unit
                return target.AddStatus(Id, new Status(Id, this, source, target, level, 0, 0, 0, 0, false, false, true));
            return target.GetStatus(Id).Reapply(0, 0, 0);
        }

        public Status GetStatus(NoxUnit target)
        {
            return target.GetStatus(Id);
        }
        public bool ContainStatus(NoxUnit target)
        {
            return target.ContainsStatus(Id);
        }

    }
}
