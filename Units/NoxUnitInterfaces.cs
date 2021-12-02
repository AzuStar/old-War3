using System;
using NoxRaven.Events.EventTypes;
using static War3Api.Common;
using static War3Api.Blizzard;
namespace NoxRaven.Units
{
    public partial class NoxUnit
    {
        /// <summary>
        /// Is hit on kill events
        /// </summary>
        public Action<KillEvent> OnKill = (e) => { };
        /// <summary>
        /// Called when unit dies and becomes corpe.<br />
        /// </summary>
        public Action<KillEvent> OnDeath = (e) => { };
        /// <summary>
        /// Deconstructor in short. Can't use classical ~Deconstructor() because lua?
        /// </summary>
        public Action<RemovalEvent> OnRemoval = (deconstructingUnit) => { };
        /// <summary>
        /// Extra function in case damage needs to be altered, counted, recordered, redirected, well...anyfuckingthing :)<br />
        /// Can even customly process it if you want. Eg take processedDamage (after armour, damage reduction..etc) and return -5 to that value
        /// </summary>
        /// <returns></returns>
        public Action<DamageEvent> OnRecievePhysicalDamage = (e) => { };//
        public Action<DamageEvent> OnDealPhysicalDamage = (e) => { };

        public Action<RegenerationEvent> OnHeal = (e) => { };//
        public Action<RegenerationEvent> OnReplenish = (e) => { };//
        /// <summary>
        /// Called every regeneration timeout, parsed value is 'per second'. <br />
        /// </summary>
        public Action<RegenerationTickEvent> OnRegenerationTick = (e) => { };
        public Action<CalculateTotalEvent> OnCalculateTotalMana = (e) => { };//
        /// <summary>
        /// This sets new total HP.
        /// Don't forget float poor precision offset.
        /// Add + 0.19f for completness
        /// </summary>
        public Action<CalculateTotalEvent> OnCalculateTotalHP = (e) => { };//
        //public Action<FloatEvent> OnSetBaseDamage = (e) => { };//
        public Action<FloatEvent> OnAddGreyArmor = (e) => { };
        public Action<FloatEvent> OnAddGreenArmor = (e) => { };
        public Action<FloatEvent> OnMoveSpeedChange = (e) => { };
    }
}