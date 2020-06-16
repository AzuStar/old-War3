using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Blizzard;
using static War3Api.Common;

namespace NoxRaven.Frames
{
    public class UIFrame
    {
        public readonly framehandle _Frame;
        public UIFrame(string template, string name, framehandle parent, string inherits, int createContext)
        {
            _Frame = BlzCreateFrameByType(template, name, parent, inherits, createContext);
        }
    }
}
