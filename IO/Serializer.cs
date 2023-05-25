using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NoxRaven.IO
{
    public static class Serializer
    {
        private static int _iter;
        public static T Deserialize<T>(string str) where T : ISerialized
        {
            Dictionary<string, string> dict = Deserialize(str);
            Type _type = typeof(T);
            T obj = (T)Activator.CreateInstance(_type);
            _type.GetMethod("Deserialize").Invoke(obj, new object[] { dict });

            return obj;
        }

        public static Dictionary<string, string> Deserialize(string str)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (_iter = 0; _iter < str.Length; _iter++)
            {
                if (str[_iter] == '"')
                {
                    string key = "";
                    string value = "";
                    _iter++;
                    while (str[_iter] != '"')
                    {
                        key += str[_iter];
                        _iter++;
                    }
                    // now we need to find a substring that contains the entire object
                    // object could be "string", 123, true, {object}, [array]

                    while (str[_iter] != ':') _iter++;
                    _iter++;
                    while (str[_iter] == ' ') _iter++;
                    if (str[_iter] == '{')
                    {
                        int bracketCount = 0;
                        if (str[_iter] == '{') bracketCount++;
                        while (bracketCount != 0)
                        {
                            value += str[_iter];
                            _iter++;
                            if (str[_iter] == '{') bracketCount++;
                            else if (str[_iter] == '}') bracketCount--;
                        }
                        value += str[_iter];
                    }
                    else if (str[_iter] == '[')
                    {
                        int bracketCount = 0;
                        if (str[_iter] == '[') bracketCount++;
                        while (bracketCount != 0)
                        {
                            value += str[_iter];
                            _iter++;
                            if (str[_iter] == '[') bracketCount++;
                            else if (str[_iter] == ']') bracketCount--;
                        }
                        value += str[_iter];
                    }
                    else if (str[_iter] == '"')
                    {
                        value += str[_iter];
                        _iter++;
                        while (str[_iter] != '"')
                        {
                            value += str[_iter];
                            _iter++;
                        }
                        value += str[_iter];
                    }
                    else
                    {
                        while (str[_iter] != ',' && str[_iter] != '}' && str[_iter] != ']')
                        {
                            value += str[_iter];
                            _iter++;
                        }
                        value.Replace(" ", "");
                    }

                    dict.Add(key, value);
                }

            }
            return dict;
        }

        public static string Serialize<T>(T obj) where T : ISerialized
        {

            StringBuilder sb = new StringBuilder();
            SerializeGeneric(obj, sb);
            return sb.ToString();

        }

        public static void SerializeGeneric(object obj, StringBuilder sb)
        {
            if (obj == null)
            {
                sb.Append("null");
                return;
            }

            Type type = obj.GetType();

            if (type == typeof(int) || type == typeof(float) || type == typeof(bool))
            {
                sb.Append(obj.ToString());
            }
            else if (type == typeof(string))
            {
                sb.Append($"\"{obj}\"");
            }
            // else if (typeof(IDictionary).IsAssignableFrom(type))
            // {
            //     SerializeDictionary(obj as IDictionary, sb);
            // }
            else if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
            {
                SerializeArray(obj as IEnumerable, sb);
            }
            else if (typeof(ISerialized).IsAssignableFrom(type))
            {
                SerializeObject(obj, sb);
            }
        }

        public static void SerializeArray(IEnumerable array, StringBuilder sb)
        {
            sb.Append("[");
            bool first = true;

            foreach (object item in array)
            {
                if (!first)
                {
                    sb.Append(",");
                }

                SerializeGeneric(item, sb);
                first = false;
            }
            sb.Append("]");
        }

        public static void SerializeObject(object obj, StringBuilder sb)
        {
            sb.Append("{");

            Type _type = obj.GetType();
            _type.GetMethod("Serialize").Invoke(obj, new object[] { sb });

            sb.Append("}");
        }

        // private static void SerializeDictionary(IDictionary dictionary, StringBuilder sb)
        // {
        //     sb.Append("{");
        //     bool first = true;

        //     foreach (object key in dictionary.Keys)
        //     {
        //         if (!first)
        //         {
        //             sb.Append(",");
        //         }

        //         sb.Append($"\"{key.ToString()}\":");

        //         sb.Append("{");
        //         Serialize(dictionary[key], sb);
        //         sb.Append("}");
        //         first = false;
        //     }

        //     sb.Append("}");
        // }
    }

}