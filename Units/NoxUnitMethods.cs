using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoxRaven.Units
{
    public class NoxUnitMethods
    {
        /// <summary>
        /// Override to hit kill events
        /// </summary>
        public Action OnKill = () => { };
        /// <summary>
        /// Called when unit dies and becomes corpe.<br />
        /// Return true to remove it instantly and false to leave corpse.
        /// </summary>
        public Func<NoxUnit, bool> OnDeath = (NoxUnit killer) => { return false; };
        /// <summary>
        /// Deconstructor in short. Can't use classical ~Deconstructor() because lua?
        /// </summary>
        public Action DeattachClass = () => { };
        /// <summary>
        /// Extra function in case damage needs to be altered, counted, recordered, redirected, well...anyfuckingthing :)<br />
        /// Return output damage. Can even customly process it if you want. Eg take processedDamage (after armour, damage reduction..etc) and return -5 to that value
        /// </summary>
        /// <param name="damageSource"></param>
        /// <param name="damage"></param>
        /// <param name="onHit"></param>
        /// <param name="crit"></param>
        /// <param name="spell"></param>
        /// <returns></returns>
        public Func<NoxUnit, float, float, bool, bool, bool, bool, float> OnPhysicalDamage = (NoxUnit damageSource, float damage, float processedDamage, bool triggerOnHit, bool canCrit, bool isSpell, bool isRanged) => { return processedDamage; };
        /// <summary>
        /// Perfect damage, when dice is 1 and roll is 1
        /// </summary>
        /// <returns></returns>
        public Func<float> AbilityDamage = () => { return 0; };
        /// <summary>
        /// Perfect damage, when dice is 1 and roll is 1
        /// </summary>
        /// <returns></returns>
        public Func<float> WeaponDamage = () => { return 0; };
        /// <summary>
        /// Called every regeneration timeout. <br />
        /// Use <see cref="NoxUnit.RegenerationTimeout"/> * xxx to turn into per sec.
        /// </summary>
        public Action Regenerate = () => { };
        public Action CalculateTotalMana = () => { };
        /// <summary>
        /// Override to your needs.
        /// This sets new total HP.
        /// Don't forget float poor precision offset.
        /// Add + 0.19f for completness
        /// </summary>
        public Action CalculateTotalHP = () => { };
        public Action<int> AddBaseDamage = (int val) => { };
        public Action<int> AddBonusDamage = (int val) => { };
        public Action<float> AddGreyArmor = (float val) => { };
        public Action<float> AddMoveSpeedpercent = (float val) => { };
        public Action<float> AddBaseMoveSpeed = (float val) => { };

    }
}
