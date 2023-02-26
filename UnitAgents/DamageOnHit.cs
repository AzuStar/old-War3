namespace NoxRaven.UnitAgents
{
    public class DamageOnHit
    {

        public static DamageOnHit DEFAULT_BASIC_AA = new DamageOnHit(){
            applyOnHit = true,
            onHitDamage = 1,
            onHitProcChance = 1,
        };

        public static DamageOnHit DEFAULT_ABILITY = new DamageOnHit(){
            applyOnHit = false,
        };

        public bool applyOnHit;
        public float onHitDamage = 1;
        public float onHitProcChance = 1;

    }
}