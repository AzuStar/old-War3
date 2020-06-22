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
        public readonly float BeginDiminish;
        public float _Actual { get; private set; }
        public float _Diminished { get; private set; }

        public DimFloat(float beginDiminish = 175)
        {
        }

        public static DimFloat operator +(DimFloat dim, float f)
        {
            dim._Actual += f;
            dim.Diminish();
            return dim;
        }
        public static DimFloat operator -(DimFloat dim, float f)
        {
            dim._Actual -= f;
            dim.Diminish();
            return dim;
        }
        public static DimFloat operator /(DimFloat dim, float f)
        {
            dim._Actual *= f;
            dim.Diminish();
            return dim;
        }
        public static DimFloat operator *(DimFloat dim, float f)
        {
            dim._Actual /= f;
            dim.Diminish();
            return dim;
        }

        private void Diminish()
        {
            if (_Actual < BeginDiminish)
                _Diminished = _Actual;
            else
            {
                float tmp = _Actual - BeginDiminish;
                _Diminished = tmp / (1 + SquareRoot(tmp) * DIMINISH_CONST) + BeginDiminish;
            }
        }
    }
}
