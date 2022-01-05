using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Units;
using static War3Api.Common;

namespace NoxRaven
{
    public class Projectile
    {
        /// <summary>
        /// Get it from map
        /// </summary>
        public static readonly int DUMMY_ID = FourCC("proj");
        public static readonly int CROW_FORM = FourCC("Amrf");
        public static readonly player OWNER_ID = Player(PLAYER_NEUTRAL_PASSIVE);
        //public static Dictionary<>

        public NoxUnit Owner;
        //? nullables
        public NoxUnit TargetUnit;
        public System.Drawing.PointF TargetLocation;

        // public Projectile(NoxUnit owner, )
        // {

        // }
    }
}
