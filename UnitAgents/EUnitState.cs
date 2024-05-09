namespace NoxRaven.UnitAgents
{
    // Reserved states available to all NUnits
    public enum EUnitState : int
    {
        // these are states that are used by the engine
        MAX_MP = 0,
        MAX_HP,
        MP_REG,
        HP_REG,

        ATK,
        GREY_ATK,
        GREEN_ATK,

        AP,
        GREY_AP,
        GREEN_AP,

        RELOAD_TIME,

        ARMOR,
        GREY_ARMOR,
        GREEN_ARMOR,

        MAGIC_RESIST,
        GREY_MAGIC_RESIST,
        GREEN_MAGIC_RESIST,

        ABSORBTION,

        MOVEMENT_SPEED,
        ATTACK_RANGE,

        // Stats from which states above are computed
        BASE_ATK = 100,
        BASE_ATK_PERCENT_BASE,
        BONUS_ATK,
        BONUS_ATK_PERCENT_BASE,
        BONUS_ATK_PERCENT_TOTAL,

        BASE_AP,
        BASE_AP_PERCENT_BASE,
        BONUS_AP,
        BONUS_AP_PERCENT_BASE,
        BONUS_AP_PERCENT_TOTAL,

        /// <summary>
        /// Base reload / attackSpeed = attacks a second <br />
        /// 1/2 = 0.5 (2 attacks a second)<br />
        /// 2/1 = 2 (0.5 )
        /// </summary>
        ATTACK_SPEED,
        BASE_RELOAD_TIME,

        BASE_ATTACK_RANGE,


        BASE_HP,
        BASE_HP_PERCENT_BASE,
        BONUS_HP,
        BONUS_HP_PERCENT_BASE,
        BONUS_HP_PERCENT_TOTAL,
        BASE_HP_REGEN,
        BASE_HP_REGEN_PERCENT_BASE,

        BASE_MP,
        BASE_MP_PERCENT_BASE,
        BONUS_MP,
        BONUS_MP_PERCENT_BASE,
        BONUS_MP_PERCENT_TOTAL,
        BASE_MP_REGEN,
        BASE_MP_REGEN_PERCENT_BASE,

        BASE_ARMOR,
        BASE_ARMOR_PERCENT_BASE,
        BONUS_ARMOR,
        BONUS_ARMOR_PERCENT_BASE,
        BONUS_ARMOR_PERCENT_TOTAL,

        BASE_MAGIC_RESIST,
        BASE_MAGIC_RESIST_PERCENT_BASE,
        BONUS_MAGIC_RESIST,
        BONUS_MAGIC_RESIST_PERCENT_BASE,
        BONUS_MAGIC_RESIST_PERCENT_TOTAL,

        BASE_ABSORB,
        BASE_ABSORB_PERCENT_BASE,

        BASE_MOVE_SPEED,
        BASE_MOVE_SPEED_PERCENT_BASE,

        // *****************
        // * direct access *
        // *****************

        /// <summary>
        /// This is a Chance diceroll event happens. 
        /// For example, passive ability with 25% activation chance to deal extra damage will have 50% chance if this value is 2.
        /// </summary>
        TRIGGER_CHANCE = 1000,
        DAMAGE_RESIST,

        /// <summary>
        /// Life restored from amount of ALL PHYSICAL damage DEALT <br/>
        /// Damage flags affected by life steal: isBaseAttack, isCrit, isOnhit, isPhysical
        /// </summary>
        LIFESTEAL,
        /// <summary>
        /// Life restored from amount of ALL spell damage DEALT <br/>
        /// Damage flags affected by spell vamp: isPhysical, isCrit, isMagical
        /// </summary>
        SPELLVAMP,
        /// <summary>
        /// Healing from all sources will be multiplied by this.
        /// </summary>
        INCOMING_HEALING,
        /// <summary>
        /// When mana is added this is multiplier.
        /// </summary>
        INCOMING_MANA,

        DODGE_CHANCE,

        EXP_RATE,


        // old artifact
        WINDUP_MODIFIER,

        PENETRATION_ARMOR,
        PENETRATION_MAGIC_RESIST,

        CRIT_CHANCE,
        CRIT_DAMAGE,

        COOLDOWN_REDUCTION,
    }
}