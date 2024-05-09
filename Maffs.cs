using System;
using System.Numerics;
using War3Api;

namespace NoxRaven
{
    public class Maffs
    {
        public static Vector2 DirectionVector(float distance, float angle)
        {
            float X = (float)(distance * Math.Cos(angle * Blizzard.bj_DEGTORAD));
            float Y = (float)(distance * Math.Sin(angle * Blizzard.bj_DEGTORAD));
            return new Vector2(X, Y);
        }

        public static Vector2 PolarProjection(Vector2 start, float distance, float angle)
        {
            return start + DirectionVector(distance, angle);
        }
        public static float GetSquaredDistance(Vector2 v1, Vector2 v2)
        {
            float dx = v2.X - v1.X;
            float dy = v2.Y - v1.Y;
            return dx * dx + dy * dy;
        }
        public static float GetDistance(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt(GetSquaredDistance(v1, v2));
        }
        public static float GetFacingTowardsAngle(Vector2 position, Vector2 facingTowardsPosition)
        {
            return Common.Atan2(facingTowardsPosition.Y - position.Y, facingTowardsPosition.X - position.X) * Blizzard.bj_RADTODEG;
        }
    }
}
