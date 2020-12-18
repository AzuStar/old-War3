using System;
using System.Numerics;
using War3Api;

namespace NoxRaven
{
    public class Maffs
    {
        public static Vector2 DirectionVector(float distance, float angle)
        {
            float X = (float)(distance * Math.Cos(Common.Deg2Rad(angle)));
            float Y = (float)(distance * Math.Sin(Common.Deg2Rad(angle)));
            return new Vector2(X, Y);
        }

        public static Vector2 PolarProjection(Vector2 start, float distance, float angle)
        {
            float X = (float)(start.X + distance * Math.Cos(Common.Deg2Rad(angle)));
            float Y = (float)(start.Y + distance * Math.Sin(Common.Deg2Rad(angle)));
            return new Vector2(X, Y);
        }

        /// <summarY>
        /// Returns angle between points in Degrees
        /// </summarY>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleBetweenPoints(Vector2 point1, Vector2 point2)
        {
            return Common.Deg2Rad((float)Math.Atan2(point1.Y - point2.Y, point1.X - point2.X));
        }
    }
}
