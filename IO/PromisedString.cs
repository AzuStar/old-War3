using System;
using System.Collections.Generic;
using System.Text;

namespace NoxRaven.IO
{
    /// <summary>
    /// Not yet existing string
    /// </summary>
    public class PromisedString
    {
        public int Size = -1;
        public bool HaveString = false;
        public Dictionary<int, string> Chunks = new Dictionary<int, string>();

        public void CheckComplete()
        {
            HaveString = Chunks.Count == Size;
        }

        public string BuildString()
        {
            string result = "";
            for (int i = 0; i < Size; i++)
                result += Chunks[i];
            return result;
        }
    }
}
