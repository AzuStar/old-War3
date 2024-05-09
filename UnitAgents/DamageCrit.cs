namespace NoxRaven.UnitAgents
{
    public class DamageCrit
    {
        public static DamageCrit DEFAULT_BASIC_AA = new DamageCrit()
        {
            applyCrit = true,
            guaranteedCrit = false,
            critDamageMultiplier = 0,
            critChanceBonus = 0,
        };

        public static DamageCrit DEFAULT_ABILITY = new DamageCrit()
        {
            applyCrit = false,
            guaranteedCrit = false,
        };

        public bool applyCrit;
        public bool guaranteedCrit = false;
        public float critDamageMultiplier = 0;
        public float critChanceBonus = 0;

    }
}