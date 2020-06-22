
using System;

namespace NoxRaven
{
    public class LuaMethods
    {
        /// <summary>
        /// @CSharpLua.Template = "tostring({0})"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static extern string ToString(dynamic obj);
        /// <summary>
        /// @CSharpLua.Template = "type({0})"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static extern string Type(dynamic obj);
        /// <summary>
        /// @CSharpLua.Template = "load({0})"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static extern Action Load(string s);
        /// <summary>
        /// @CSharpLua.Template = "print({0})"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static extern void Print(string s);
        /// <summary>
        /// @CSharpLua.Template = "benchmark({0}, {1})"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns
        public static extern void Benchmark(string callName, Action testFunc);

    }
}
