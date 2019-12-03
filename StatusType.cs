using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven
{
    public class StatusType
    {
        public delegate void StatusFunction(UnitEntity source, UnitEntity target, Status status);
        public readonly int Id;
        public String Effectpath;
        public String Attachment;
        public StatusFunction Apply { get; private set; }
        public StatusFunction Reset { get; private set; }
        public override int GetHashCode()
        {
            return Id;
        }
        /// <summary>
        /// Creates a StatusType from id (which should be > 100)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apply"></param>
        /// <param name="reset"></param>
        /// <param name="specialEffectPath"></param>
        /// <param name="specialEffectAttachmentPoint"></param>
        public StatusType(int id, StatusFunction apply, StatusFunction reset, String specialEffectPath, String specialEffectAttachmentPoint)
        {
            if (id < 100) id += 100;
            Id = id;
            Apply = apply;
            Reset = reset;
            Effectpath = specialEffectPath;
            Attachment = specialEffectAttachmentPoint;
        }
        public void ApplyOneTimeStatus()
        {

        }

    }
}
