using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NoxRaven;

using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Statuses;
using NoxRaven.Events;
using NoxRaven.Events.EventTypes;
using NoxRaven.Events.Metas;

/// <summary>
/// To use in own code simply create class which has inner reference to this class.
/// Call function <see cref="Master.RunAfterExtensionsReady"/>. It will initialize indexing logic for all units. Put it somewhere after all players and custom classes were initialized.<br><br></br></br>
/// <b>Never RemoveUnit() indexed unit or it will permaleak.</b>
/// </summary>
public partial class NoxUnit
{
    /// <summary>
    /// Util info.
    /// </summary>
    public bool Corpse = false;
    /// <summary>
    /// Unit itself.
    /// </summary>
    public readonly unit _Self;
    private trigger DamageTrig;

    private Dictionary<int, Status> Statuses = new Dictionary<int, Status>();

    public Dictionary<int, OnHit> OnHits = new Dictionary<int, OnHit>();
    //public Dictionary<int, OnHit> AmHits = new Dictionary<int, OnHit>();

    // ******************
    // * Damage related *
    // ******************
    /// <summary>
    /// Later would like to create projectiles when they attack.
    /// </summary>
    public bool Ranged;
    /// <summary>
    /// This is +xxx damage on unit.
    /// </summary>
    private int GreenDamage = 0;
    /// <summary>
    /// This is flat bonus of green damage.
    /// </summary>
    private int BonusDamage = 0;
    /// <summary>
    /// This is also +xxx damage but according to current base damage.
    /// </summary>
    private float BonusDamagePercent = 0;
    /// <summary>
    /// Additive(+)
    /// Range: 0.00-1.00
    /// </summary>
    public float ArmorPenetration = 0;
    /// <summary>
    /// Additive(+)
    /// Range: 0.00-1.00
    /// </summary>
    public float CritChance = 0;
    /// <summary>
    /// Additive(+)
    /// </summary>
    public float CritDamage = 1.5f;
    /// <summary>
    /// Base attack speed / AttackSpeed = New Attack Speed <br />
    /// 1/2 = 0.5 (2 attacks a second)<br />
    /// 2/4 = 0.5 (2 attacks a second)
    /// </summary>
    private float AttackSpeed = 1;
    private float BaseAttackCooldown;

    // ******************
    // * Health related *
    // ******************
    /// <summary>
    /// Set initially at runtime.
    /// </summary>
    private float BaseHP;
    /// <summary>
    /// This value is percentage of total health.
    /// Additive(+)
    /// </summary>
    private float TotalHPPercent = 1;
    /// <summary>
    /// Set initially at runtime.
    /// </summary>
    private float BaseMana;
    /// <summary>
    /// This value is percentage of total mana.
    /// Additive(+)
    /// </summary>
    private float TotalManaPercent = 1;
    /// <summary>
    /// Life from weapon attacks.
    /// Additive(+)
    /// </summary>
    public float Lifesteal = 0;
    /// <summary>
    /// Life from spells.
    /// Additive(+)
    /// </summary>
    public float SpellVamp = 0;
    /// <summary>
    /// Healing from all sources will be multiplied by this.
    /// Multiplicative (*)
    /// </summary>
    public float HealingFromAllSources = 1;
    /// <summary>
    /// Additive(+)
    /// </summary>
    public float RegenFlat = 0f;

    /// <summary>
    /// When mana is added this is multiplier.
    /// </summary>
    public float ManaFromAllSources = 1;

    public float RegenManaFlat = 0f;
    // *********************
    // * Toughness related *
    // *********************
    /// <summary>
    /// By how much damage will be reduced. This applies to all damage after all modifications (armor, armorpen etc).<para />
    /// Range: 0.00-1.00, but can be negative
    /// </summary>
    public float DamageReduction = 0;
    /// <summary>
    /// Negative and positive green armor.
    /// </summary>
    private float GreenArmor = 0;
    /// <summary>
    /// Negative and positive grey armor.
    /// </summary>
    private float GreyArmor = 0;
    //*********
    // * Util *
    //*********
    /// <summary>
    /// This is a Chance diceroll event happens. 
    /// For example, passive ability with 25% activation chance to deal extra damage will have 50% chance if this value is 2.
    /// </summary>
    public float TriggerChance = 1;
    /// <summary>
    /// Multiplier, starts at 1
    /// </summary>
    private float MovementSpeedPercent = 1;
    private float BaseMovementSpeed = 220;

}