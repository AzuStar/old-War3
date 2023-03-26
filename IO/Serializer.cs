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
        public static string SerializeToJSON<T>(T obj)
        {

            StringBuilder sb = new StringBuilder();
            Serialize(obj, sb);
            return sb.ToString();

        }

        private static void Serialize(object obj, StringBuilder sb)
        {
            try
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
                else if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
                {
                    SerializeArray(obj as IEnumerable, sb);
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    SerializeDictionary(obj as IDictionary, sb);
                }
                else
                {
                    SerializeObject(obj, sb);
                }
            }
            catch { }
        }

        private static void SerializeArray(IEnumerable array, StringBuilder sb)
        {
            sb.Append("[");
            bool first = true;

            foreach (object item in array)
            {
                if (!first)
                {
                    sb.Append(",");
                }

                Serialize(item, sb);
                first = false;
            }
            sb.Append("]");
        }

        private static void SerializeObject(object obj, StringBuilder sb)
        {
            sb.Append("{");
            bool first = true;

            Type _type = obj.GetType();
            string[] names = _type.GetField("__fieldNames").GetValue(obj) as string[];

            foreach (string name in names)
            {
                FieldInfo field = _type.GetField(name);
                if (!first)
                {
                    sb.Append(",");
                }

                sb.Append($"\"{field.Name}\":");
                object value = field.GetValue(obj);
                Serialize(value, sb);
                first = false;
            }

            sb.Append("}");
        }

        private static void SerializeDictionary(IDictionary dictionary, StringBuilder sb)
        {
            sb.Append("{");
            bool first = true;

            foreach (DictionaryEntry entry in dictionary)
            {
                if (!first)
                {
                    sb.Append(",");
                }

                sb.Append($"\"{entry.Key}\":");
                Serialize(entry.Value, sb);
                first = false;
            }

            sb.Append("}");
        }
    }

}