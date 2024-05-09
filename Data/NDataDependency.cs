using System;

namespace NoxRaven.Data
{
    public struct NDataDependency
    {
        public readonly int targetStatId;
        public readonly int sourceStatId;
        public readonly EArithmetic arithmetic;

        public NDataDependency(Enum targetStatId, Enum sourceStatId, EArithmetic arithmetic)
        {
            this.targetStatId = targetStatId.GetHashCode();
            this.sourceStatId = sourceStatId.GetHashCode();
            this.arithmetic = arithmetic;
        }

        public NDataDependency(int targetStatId, int sourceStatId, EArithmetic arithmetic)
        {
            this.targetStatId = targetStatId;
            this.sourceStatId = sourceStatId;
            this.arithmetic = arithmetic;
        }
    }
}
