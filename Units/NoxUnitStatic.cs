using System;
using System.Collections.Generic;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Statuses;
using NoxRaven;
using NoxRaven.Events.EventTypes;

public partial class NoxUnit
{
    protected internal static Dictionary<int, NoxUnit> Indexer = new Dictionary<int, NoxUnit>();
    private static Dictionary<int, Type> CustomTypes = new Dictionary<int, Type>();
    private static float KeepCorpsesFor = 25;
    private static bool DamageEngineIgnore = false;
    public const float RegenerationTimeout = 0.04f;
    internal static int[] Abilities_BonusDamage = new int[19];
    internal static int[] Abilities_Corruption = new int[13];
    internal static int[] Abilities_BonusArmor = new int[14];
    /// <summary>
    /// Default game reduction constant.
    /// </summary>
    public const float ARMOR_CONST = 0.06f;

    /// <summary>
    /// Put a custom type that will be attached to a unit when indexing.
    /// Custom type has to extend this class and invoke base(u) in the constructor.
    /// </summary>
    /// <param name="unitId"></param>
    /// <param name="t">hit type</param>
    public static void AddCustomType(int unitId, Type t)
    {
        if (typeof(NoxUnit).IsAssignableFrom(t))
        {
            Utils.Error("You have tried to parse type that does not inherit this class!", typeof(NoxUnit));
            return;
        }
        CustomTypes[unitId] = t;
    }
    /// <summary>
    /// Put a custom type that will be attached to a unit when indexing.
    /// Custom type has to extend this class and invoke base(u) in the constructor.
    /// </summary>
    /// <param name="unitId"></param>
    /// <param name="t"></param>
    public static void AddCustomType(string unitId, Type t)
    {
        AddCustomType(FourCC(unitId), t);
    }

    /// <summary>
    /// Put this initializer somewhere after all players have been initialized. Do this only after you have put all customtypes in the dictionary.
    /// </summary>
    internal static void InitUnitLogic()
    {
        region reg = CreateRegion();
        group g = CreateGroup();
        rect rec = GetWorldBounds();
        RegionAddRect(reg, rec);
        TriggerRegisterEnterRegion(CreateTrigger(), reg, Filter(AttachClass));

        // Add existing units
        GroupEnumUnitsInRect(g, rec, Filter(AttachClass));

        // // Deattach when unit leaves the map
        // TriggerRegisterLeaveRegion(CreateTrigger(), reg, Filter(() => { ((NoxUnit)GetLeavingUnit()).OnRemoval(); return false; })); // catch unit removal, destroy everything attached
        // // Utility functions
        // TimerStart(CreateTimer(), RegenerationTimeout, true, () => { foreach (NoxUnit ue in Indexer.Values) ue.Regenerate(); });

        // Recycle stuff
        DestroyGroup(g);
        g = null;
        rec = null;
        reg = null;
    }

    private static bool AttachClass()
    {
        unit u = GetFilterUnit();

        if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return false;
        if (Indexer.ContainsKey(GetHandleId(u))) return false;

        if (CustomTypes.ContainsKey(GetUnitTypeId(u))) Indexer[GetHandleId(u)] = (NoxUnit)Activator.CreateInstance(CustomTypes[GetUnitTypeId(u)], u);
        else if (IsUnitType(u, UNIT_TYPE_HERO)) Indexer[GetHandleId(u)] = new NoxHero(u);
        else Indexer[GetHandleId(u)] = new NoxUnit(u);
        u = null;
        return false;
    }
    /// <summary>
    /// Do not call RemoveUnit on indexed unit or permaleak.
    /// </summary>
    /// <param name="u"></param>
    private static void AwaitRemoval(NoxUnit u, NoxUnit killer)
    {
        if (!Indexer.ContainsKey(GetHandleId(u))) return; // this is a very weird thing to happen, but will happen for Neutrals so yeah

        if (u.Corpse) return;
        KillEvent @event = new KillEvent()
        {
            EventInfo = new NoxRaven.Events.Metas.KillMeta()
            {
                Killer = killer,
                Dying = u
            }
        };
        killer.OnKill(@event);
        u.OnDeath(@event);

        foreach (Status st in u.Statuses.Values)
            st.Remove();
        u.Statuses.Clear();// just in case

        if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return; // wat
        if (IsUnitType(u, UNIT_TYPE_HERO)) return; // always leak heroes

        u.Corpse = true;

        Utils.DelayedInvoke(KeepCorpsesFor, () => { u.Remove(); }); // ah shiet, change to let resurrections
        //ue = null;
    }

    // ****************
    // * Unit Methods *
    // ****************
    private void DamageHandler()
    {
        if (DamageEngineIgnore) return;
        if (GetEventDamage() < 1) return;
        NoxUnit source = Cast(GetEventDamageSource());
        BlzSetEventDamage(0);
        // ~this is the target
        //if (!Ranged) // later
        float dmg = source.WeaponDamage();

        source.DealPhysicalDamage(this, dmg, true, true, false, Ranged);
    }

    protected NoxUnit(unit u)
    {

        _Self = u;
        {
            // Regenerate = () =>
            // {
            //     Heal(RegenFlat * RegenerationTimeout);
            //     ReplenishMana(RegenManaFlat * RegenerationTimeout);
            // },
            // AbilityDamage = () =>
            // {
            //     return (BlzGetUnitBaseDamage(this, 0) + 1 + GreenDamage) * DamageMultiplier;
            // },
            // WeaponDamage = () =>
            // {
            //     return (BlzGetUnitBaseDamage(this, 0) + 1 + GreenDamage) * DamageMultiplier;
            // },
            // OnCalculateTotalHP = () =>
            // {
            //     
            // },
            // OnAddBaseDamage = (int val) =>
            // {
            //     BlzSetUnitBaseDamage(_Self, GetBaseDamage() + val, 0);
            //     SetGreenDamage(BonusDamage + R2I(BlzGetUnitBaseDamage(_Self, 0) * BonusDamagePercent));
            // },
            // OnAddBonusDamage = (int val) =>
            // {
            //     BonusDamage += val;
            //     SetGreenDamage(BonusDamage + R2I(BlzGetUnitBaseDamage(_Self, 0) * BonusDamagePercent + Utils.ROUND_DOWN_CONST_OVERHEAD));

            // },
            // OnRemoval = () =>
            // {
            //     OnHits.Clear();
            //     //AmHits.Clear();
            //     DestroyTrigger(DamageTrig);
            //     Indexer.Remove(GetHandleId(_Self));
            //     OnHits = null;
            //     Statuses = null;
            //     RemoveUnit(this);
            //     DamageTrig = null;
            // },
            // OnAddGreyArmor = (float val) =>
            // {
            //     SetGreenArmor(GreyArmor += val);
            // },
            // OnAddMoveSpeedpercent = (float val) =>
            // {
            //     MovementSpeedPercent += val;
            //     SetUnitMoveSpeed(this, BaseMovementSpeed * MovementSpeedPercent);
            // },
            // OnAddBaseMoveSpeed = (float val) =>
            // {
            //     BaseMovementSpeed += val;
            //     SetUnitMoveSpeed(this, BaseMovementSpeed * MovementSpeedPercent);
            // },
            // CalculateTotalMana
        };

        // WeaponDamage = () => { return 10; };
        // SetBaseHP(BlzGetUnitMaxHP(u));
        // BaseAttackCooldown = BlzGetUnitAttackCooldown(_Self, 0);
        // AddAttackSpeed(0);
        // GreyArmor = BlzGetUnitArmor(u);
        // BaseMovementSpeed = GetUnitMoveSpeed(u);
        Ranged = IsUnitType(u, UNIT_TYPE_RANGED_ATTACKER);
        // Damage Utilization
        DamageTrig = CreateTrigger();
        TriggerRegisterUnitEvent(DamageTrig, u, EVENT_UNIT_DAMAGED);
        TriggerAddAction(DamageTrig, DamageHandler);
        UnitAddAbility(u, FourCC("ARDP")); //arm decimal
        BlzUnitHideAbility(u, FourCC("ARDP"), true);
    }

    private void Remove()
    {
        OnRemoval(new RemovalEvent() { Target = this });
        OnHits.Clear();
        //AmHits.Clear();
        DestroyTrigger(DamageTrig);
        Indexer.Remove(GetHandleId(_Self));
        OnHits = null;
        Statuses = null;
        RemoveUnit(this);
        DamageTrig = null;
    }

    /// <summary>
    /// Set unit's -xxx.x or +xxx.x armor. Does support single precision decimal point.
    /// </summary>
    /// <param name="val"></param>
    private void SetGreenArmor(float val)
    {
        int leftover = R2I(val);
        int decimals = R2I((val - R2I(val)) * 10);
        if(val < 0) decimals = 1 - decimals;
        SetUnitAbilityLevel(_Self, FourCC("ARDP"), decimals + 1);
        GreenArmor = val;
        foreach (int abil in Abilities_BonusArmor)
            UnitRemoveAbility(this, abil);
        foreach (int abil in Abilities_Corruption)
            UnitRemoveAbility(this, abil);

        if (leftover < 0)
        {
            leftover = -leftover;
            for (int i = Abilities_Corruption.Length - 1; i >= 0; i--)
            {
                int comparator = R2I(Pow(2, i));
                if (comparator <= leftover)
                {
                    UnitAddAbility(this, Abilities_Corruption[i]);
                    leftover -= comparator;
                }
            }
        }
        else
            for (int i = Abilities_BonusArmor.Length - 1; i >= 0; i--)
            {
                int comparator = R2I(Pow(2, i));
                if (comparator <= leftover)
                {
                    UnitAddAbility(this, Abilities_BonusArmor[i]);
                    leftover -= comparator;
                }
            }
    }

    // This guy makes +xxx damage magic
    private void SetGreenDamage(int val)
    {
        GreenDamage = val;
        for (int i = Abilities_BonusDamage.Length - 1; i >= 0; i--)
        {
            UnitRemoveAbility(this, Abilities_BonusDamage[i]);
            int comparator = R2I(Pow(2, i));
            if (comparator <= val)
            {
                UnitAddAbility(this, Abilities_BonusDamage[i]);
                val -= comparator;
            }
        }
    }

    private void CalculateTotalHP()
    {
        CalculateTotalEvent ev = new CalculateTotalEvent()
        {
            ExpectedTotal = BaseHP + TotalHPPercent,
        };
        OnCalculateTotalHP(ev);
        BlzSetUnitMaxHP(this, R2I(ev.ExpectedTotal + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
    }

    private void CalculateTotalMana()
    {
        CalculateTotalEvent ev = new CalculateTotalEvent()
        {
            ExpectedTotal = BaseHP + TotalHPPercent,
        };
        OnCalculateTotalMana(ev);
        BlzSetUnitMaxMana(this, R2I(ev.ExpectedTotal + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
    }

    private float WeaponDamage()
    {
        return (BlzGetUnitBaseDamage(this, 0) + 1 + GreenDamage);
    }
    #region internal Status api
    internal bool ContainsOnHit(int id)
    {
        return OnHits.ContainsKey(id);
    }
    internal OnHit AddOnHit(int id, OnHit toAdd)
    {
        OnHits.Add(id, toAdd);
        return toAdd;
    }
    internal OnHit GetOnHit(int id)
    {
        return OnHits[id];
    }
    internal void RemoveOnHit(int id)
    {
        OnHits.Remove(id);
    }
    internal bool ContainsStatus(int id)
    {
        //if(st.GetHashCode()<=100)
        return Statuses.ContainsKey(id);
    }
    internal Status AddStatus(int id, Status toAdd)
    {
        Statuses.Add(id, toAdd);
        return toAdd;
    }
    #endregion

    // may be implicit operator, Ive been down this road
    public static NoxUnit Cast(unit u)
    {
        return Indexer[GetHandleId(u)];
    }
    public static implicit operator NoxUnit(unit u)
    {
        return Cast(u);
    }
    public static implicit operator unit(NoxUnit ue)
    {
        return ue._Self;
    }
}