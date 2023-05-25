using System.Collections.Generic;
using System.Text;

namespace NoxRaven.IO
{
    public interface ISerialized
    {
        void Serialize(StringBuilder sb);
        void Deserialize(Dictionary<string, string> dict);
    }
}