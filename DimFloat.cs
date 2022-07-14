using System;
using System.Collections.Generic;
using System.Text;

using static War3Api.Blizzard;
using static War3Api.Common;

namespace NoxRaven
{
    public sealed class DimFloat
    {
        /// <summary>
        /// Higher - stronger diminish<br />
        /// Also creates upper boundary Lim(f) -> 1/<see cref="DIMINISH_CONST"/>
        /// </summary>
        public const float DIMINISH_CONST = 0.06f;
        public readonly float beginDiminish;
        public float _actual { get; private set; }
        public float _diminished { get; private set; }

        public DimFloat(float beginDiminish = 175)
        {
            this.beginDiminish = beginDiminish;
        }

        public static DimFloat operator +(DimFloat dim, float f)
        {
            dim._actual += f;
            dim.Diminish();
            return dim;
        }
        public static DimFloat operator -(DimFloat dim, float f)
        {
            dim._actual -= f;
            dim.Diminish();
            return dim;
        }
        public static DimFloat operator /(DimFloat dim, float f)
        {
            dim._actual *= f;
            dim.Diminish();
            return dim;
        }
        public static DimFloat operator *(DimFloat dim, float f)
        {
            dim._actual /= f;
            dim.Diminish();
            return dim;
        }

        private void Diminish()
        {
            if (_actual < beginDiminish)
                _diminished = _actual;
            else
            {
                float tmp = _actual - beginDiminish;
                _diminished = tmp / (1 + SquareRoot(tmp) * DIMINISH_CONST) + beginDiminish;
            }
        }
    }
}
