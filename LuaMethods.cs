
using System;

namespace NoxRaven
{
    public class LuaMethods
    {
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
        /// <summary>
        /// @CSharpLua.Template = "tostring({0})"
        /// </summary>
        /// <returns></returns>
        public static extern string ToString(dynamic obj);
        /// <summary>
        /// @CSharpLua.Template = "type({0})"
        /// </summary>
        /// <returns></returns>
        public static extern string Type(dynamic obj);
        /// <summary>
        /// @CSharpLua.Template = "load({0})"
        /// </summary>
        /// <returns></returns>
        public static extern Func<T> Load<T>(string s);
        /// <summary>
        /// @CSharpLua.Template = "print({0})"
        /// </summary>
        /// <returns></returns>
        public static extern void Print(string s);
        /// <summary>
        /// @CSharpLua.Template = "benchmark({0}, {1})"
        /// </summary>
        /// <returns></returns>
        public static extern void Benchmark(string callName, Action testFunc);
        /// <summary>
        /// @CSharpLua.Template = "for {1},{2} in pairs({0}) do"
        /// </summary>
        /// 
        public static extern void ForPairs(object table, object keyName, object valueName);
        /// <summary>
        /// @CSharpLua.Template = "end"
        /// </summary>
        public static extern void End();
#pragma warning restore CS0626 // Method, operator, or accessor is marked external and has no attributes on it
    }
}
