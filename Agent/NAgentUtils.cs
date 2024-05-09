namespace NoxRaven
{
    public class UnitUtils
    {
        public static float GetDamageReductionFromArmor(float armormr)
        {
            if (armormr < 0)
                return 2 - (100 / (100 - armormr));
            else
                return 100 / (100 + armormr);
        }
    }
}