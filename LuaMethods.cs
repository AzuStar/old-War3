using War3Net.CodeAnalysis.Common;

namespace NoxRaven
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
