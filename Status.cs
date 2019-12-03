using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven
{
    public class Status
    {
        StatusType Type;
        UnitEntity Source;
        UnitEntity Target;
        public int Stacks;
        internal Status()
        {

        }

        public void Reset()
        {
            Type.Reset.Invoke(Source, Target, this);
        }

    }
}
