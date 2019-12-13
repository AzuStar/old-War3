using System;
using System.Collections.Generic;
using System.Text;
using War3Net.CodeAnalysis.Common;

namespace War3Map.TimeAbyss.Source
{
    [NativeLuaMemberContainer]
    public class LuaMethods
    {
        [NativeLuaMember("tostring")]
        public static extern string LuaToString(dynamic obj);
        [NativeLuaMember("type")]
        public static extern string LuaType(dynamic obj);
    }
}
