using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven
{
    /// <summary>
    /// Some really special utilities that does not really belong (or may be not yet) in UnitEntity
    /// </summary>
    public static class UnitExperimentals
    {
        public static float MAX_RANGE = 10f;
        public static int DUMMY_ITEM_ID = FourCC("wolq");
    }
}
