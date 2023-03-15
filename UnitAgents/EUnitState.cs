namespace NoxRaven.UnitAgents
{
    // Reserved states available to all NUnits
    public enum EUnitState : int
    {
        // these are states that are used by the engine
        MAX_MP = 0,
        MAX_HP,
        SLD,
        MP_REG,
        HP_REG,

        GREY_ATK,
        GREEN_ATK,
        GREY_AP,
        GREEN_AP,
        RLD,

        GREY_ARM,
        GREEN_ARM,
        GREY_RES,
        GREEN_RES,

        ABS,
        DMG_REDUCE,

        UNIT_MS,
        RNG,

        EXP_RATE,

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
        RELOAD_TIME,

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

        PENETRATION_ARMOR,
        PENETRATION_MAGIC_RESIST,

        CRIT_CHANCE,
        CRIT_DAMAGE,
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

        
        //*********
        // * Util *
        //*********
        /// <summary>
        /// This is a Chance diceroll event happens. 
        /// For example, passive ability with 25% activation chance to deal extra damage will have 50% chance if this value is 2.
        /// </summary>
        DAMAGE_RESIST,
        TRIGGER_CHANCE,

        BASE_ABSORB,
        BASE_ABSORB_PERCENT_BASE,

        BASE_MOVE_SPEED,
        BASE_MOVE_SPEED_PERCENT_BASE,

    }
}