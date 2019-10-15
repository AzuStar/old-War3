using System;
using System.Collections.Generic;
using System.Text;
using War3Net.CodeAnalysis.Common;

namespace War3.NoxRaven
{
    [NativeLuaMemberContainer]
    public class LuaMethods
    {
        [NativeLuaMember("collectgarbage")]
        public static extern void RunGC();
    }
}
