using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoxRaven.Statuses
{
    public abstract class StatusType
    {
        public delegate void StatusFunction(Status status);
        public readonly int Id;
        public static int Count;
        public String Effectpath;
        public String Attachment;
        internal StatusFunction Apply { get; private set; }
        internal StatusFunction Reset { get; private set; }
        internal StatusFunction OnRemove { get; private set; }
        public override int GetHashCode()
        {
            return Id;
        }

        /// <summary>
        /// Creates a StatusType. Delegates can be null.
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="reset"></param>
        /// <param name="specialEffectPath"></param>
        /// <param name="specialEffectAttachmentPoint"></param>
        protected StatusType(StatusFunction apply, StatusFunction reset, StatusFunction onRemove, String specialEffectPath, String specialEffectAttachmentPoint)
        {
            Id = ++Count;
            Apply = apply;
            Reset = reset;
            OnRemove = onRemove;
            Effectpath = specialEffectPath;
            Attachment = specialEffectAttachmentPoint;
        }
    }
}
