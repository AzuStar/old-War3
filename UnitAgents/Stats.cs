namespace NoxRaven.UnitAgents
{
    public class Stats
    {
        // ****************
        // * Physical Dmg *
        // ****************
        public float baseDMG = 0;
        public float baseDMGPercent = 0;
        public float bonusDMG = 0;
        public float baseDMGPercentBonus = 0;

        public float armorPenetration = 0;

        /// <summary>
        /// Base attack speed / attackSpeed = New Attack Speed <br />
        /// 1/2 = 0.5 (2 attacks a second)<br />
        /// 2/1 = 2 (0.5 attacks a second)
        /// </summary>
        public float attackSpeed = 0;
        public float baseAttackCooldown;

        // *****************
        // * Ability Power *
        // *****************
        public float baseAP = 0;
        public float baseAPPercent = 0;
        public float bonusAP = 0;
        public float baseAPPercentBonus = 0;


        // **********
        // * Vitals *
        // **********
        public float baseHP;
        public float baseHPPercent;
        public float bonusHP;
        public float baseHPPercentBonus;
        public float regenHP;
        public float regenHPPercent;

        public float baseMP;
        public float baseMPPercent;
        public float bonusMP;
        public float baseMPPercentBonus;
        public float regenMP;
        public float regenMPPercent;

        public float baseARM;
        public float baseARMPercent;
        public float bonusARM;
        public float baseARMPercentBonus;

        public float baseMR;
        public float baseMRPercent;
        public float bonusMR;
        public float baseMRPercentBonus;


        public float critChace = 0;
        public float critDamage = 0;
        /// <summary>
        /// Life restored from amount of ALL PHYSICAL damage DEALT <br/>
        /// Damage flags affected by life steal: isBaseAttack, isCrit, isOnhit, isPhysical
        /// </summary>
        public float lifeSteal = 0;
        /// <summary>
        /// Life restored from amount of ALL spell damage DEALT <br/>
        /// Damage flags affected by spell vamp: isPhysical, isCrit, isMagical
        /// </summary>
        public float spellVamp = 0;
        /// <summary>
        /// Healing from all sources will be multiplied by this.
        /// </summary>
        public float incomingHealing;
        /// <summary>
        /// When mana is added this is multiplier.
        /// </summary>
        public float incomingMana;

        // *********************
        // * Toughness related *
        // *********************
        /// <summary>
        /// By how much damage will be reduced. This applies to damage after all modifications (armor, armorpen etc).<para />
        /// Range: 0.00-1.00, but can be negative
        /// Applied to types: magical, physical
        /// </summary>
        public float damageReduction = 0;

        //*********
        // * Util *
        //*********
        /// <summary>
        /// This is a Chance diceroll event happens. 
        /// For example, passive ability with 25% activation chance to deal extra damage will have 50% chance if this value is 2.
        /// </summary>
        public float triggerChance = 0;
        public float baseMSPercent = 0;
        public float baseMS = 0;

        public static Stats operator +(Stats left, Stats right)
        {
            left.baseDMG += right.baseDMG;
            left.baseDMGPercent += right.baseDMGPercent;
            left.bonusDMG += right.bonusDMG;
            left.baseDMGPercentBonus += right.baseDMGPercentBonus;
            left.armorPenetration += right.armorPenetration;
            left.attackSpeed += right.attackSpeed;
            left.baseAttackCooldown += right.baseAttackCooldown;
            left.baseAP += right.baseAP;
            left.baseAPPercent += right.baseAPPercent;
            left.bonusAP += right.bonusAP;
            left.baseAPPercentBonus += right.baseAPPercentBonus;
            left.baseHP += right.baseHP;
            left.baseHPPercent += right.baseHPPercent;
            left.bonusHP += right.bonusHP;
            left.baseHPPercentBonus += right.baseHPPercentBonus;
            left.regenHP += right.regenHP;
            left.regenHPPercent += right.regenHPPercent;
            left.baseMP += right.baseMP;
            left.baseMPPercent += right.baseMPPercent;
            left.bonusMP += right.bonusMP;
            left.baseMPPercentBonus += right.baseMPPercentBonus;
            left.regenMP += right.regenMP;
            left.regenMPPercent += right.regenMPPercent;
            left.baseARM += right.baseARM;
            left.baseARMPercent += right.baseARMPercent;
            left.bonusARM += right.bonusARM;
            left.baseARMPercentBonus += right.baseARMPercentBonus;
            left.baseMR += right.baseMR;
            left.baseMRPercent += right.baseMRPercent;
            left.bonusMR += right.bonusMR;
            left.baseMRPercentBonus += right.baseMRPercentBonus;
            left.critChace += right.critChace;
            left.critDamage += right.critDamage;
            left.lifeSteal += right.lifeSteal;
            left.spellVamp += right.spellVamp;
            left.incomingHealing += right.incomingHealing;
            left.incomingMana += right.incomingMana;
            left.damageReduction += right.damageReduction;
            left.triggerChance += right.triggerChance;
            left.baseMSPercent += right.baseMSPercent;
            left.baseMS += right.baseMS;
            return left;
        }

        public static Stats operator -(Stats left, Stats right)
        {
            left.baseDMG -= right.baseDMG;
            left.baseDMGPercent -= right.baseDMGPercent;
            left.bonusDMG -= right.bonusDMG;
            left.baseDMGPercentBonus -= right.baseDMGPercentBonus;
            left.armorPenetration -= right.armorPenetration;
            left.attackSpeed -= right.attackSpeed;
            left.baseAttackCooldown -= right.baseAttackCooldown;
            left.baseAP -= right.baseAP;
            left.baseAPPercent -= right.baseAPPercent;
            left.bonusAP -= right.bonusAP;
            left.baseAPPercentBonus -= right.baseAPPercentBonus;
            left.baseHP -= right.baseHP;
            left.baseHPPercent -= right.baseHPPercent;
            left.bonusHP -= right.bonusHP;
            left.baseHPPercentBonus -= right.baseHPPercentBonus;
            left.regenHP -= right.regenHP;
            left.regenHPPercent -= right.regenHPPercent;
            left.baseMP -= right.baseMP;
            left.baseMPPercent -= right.baseMPPercent;
            left.bonusMP -= right.bonusMP;
            left.baseMPPercentBonus -= right.baseMPPercentBonus;
            left.regenMP -= right.regenMP;
            left.regenMPPercent -= right.regenMPPercent;
            left.baseARM -= right.baseARM;
            left.baseARMPercent -= right.baseARMPercent;
            left.bonusARM -= right.bonusARM;
            left.baseARMPercentBonus -= right.baseARMPercentBonus;
            left.baseMR -= right.baseMR;
            left.baseMRPercent -= right.baseMRPercent;
            left.bonusMR -= right.bonusMR;
            left.baseMRPercentBonus -= right.baseMRPercentBonus;
            left.critChace -= right.critChace;
            left.critDamage -= right.critDamage;
            left.lifeSteal -= right.lifeSteal;
            left.spellVamp -= right.spellVamp;
            left.incomingHealing -= right.incomingHealing;
            left.incomingMana -= right.incomingMana;
            left.damageReduction -= right.damageReduction;
            left.triggerChance -= right.triggerChance;
            left.baseMSPercent -= right.baseMSPercent;
            left.baseMS -= right.baseMS;
            return left;
        }
    }
}