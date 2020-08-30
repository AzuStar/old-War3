using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NoxRaven;

using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Statuses;

namespace NoxRaven.Units
{
    /// <summary>
    /// To use in own code simply create class which has inner reference to this class.
    /// Call function <see cref="Master.RunAfterExtensionsReady"/>. It will initialize indexing logic for all units. Put it somewhere after all players and custom classes were initialized.<br><br></br></br>
    /// <b>Never RemoveUnit() indexed unit or it will permaleak.</b>
    /// </summary>
    public class NoxUnit
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
        /// Util info.
        /// </summary>
        public bool Corpse = false;
        /// <summary>
        /// Unit itself.
        /// </summary>
        public readonly unit _Self;
        private trigger DamageTrig;

        public NoxUnitMethods Interfaces = null;


        //protected List<NoxUnitMethods> Interfaces = new List<NoxUnitMethods>() { DefaultInteface };


        private Dictionary<int, Status> Statuses = new Dictionary<int, Status>();

        public Dictionary<int, OnHit> OnHits = new Dictionary<int, OnHit>();
        //public Dictionary<int, OnHit> AmHits = new Dictionary<int, OnHit>();
        #region Unit Stats
        // Unit Extra Parameters
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
        /// This is flat bonus to green damage.
        /// </summary>
        private int BonusDamage = 0;
        /// <summary>
        /// This is also +xxx damage but according to current base damage.
        /// </summary>
        private float BonusDamagePercent = 0;
        /// <summary>
        /// Total damage multiplication. Multiplicative(*)
        /// </summary>
        public float DamageMultiplier = 1;
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
        /// <summary>
        /// Chance to evade projectile (any tier 1).
        /// Additive(+)
        /// Range: 0.00-1.00
        /// </summary>
        public float Evasion = 0;
        /// <summary>
        /// Chance to reduce incoming damage by 95% applies to melee attack.
        /// Additive(+)
        /// Range: 0.00-1.00
        /// </summary>
        public float Block = 0;
        //*********
        // * Util *
        //*********
        /// <summary>
        /// This is a Chance diceroll event happens. 
        /// For example, passive ability with 25% activation chance to deal extra damage will have 50% chance if this value is 2.
        /// </summary>
        public float TriggerChance = 1;
        private float MovementSpeedPercent = 1;
        private float BaseMovementSpeed = 220;
        #endregion

        protected NoxUnit(unit u)
        {
            _Self = u;
            SetBaseHP(BlzGetUnitMaxHP(u));
            BaseAttackCooldown = BlzGetUnitAttackCooldown(_Self, 0);
            AddAttackSpeed(0);
            GreyArmor = BlzGetUnitArmor(u);
            BaseMovementSpeed = GetUnitMoveSpeed(u);
            Ranged = IsUnitType(u, UNIT_TYPE_RANGED_ATTACKER);
            // Damage Utilization
            DamageTrig = CreateTrigger();
            TriggerRegisterUnitEvent(DamageTrig, u, EVENT_UNIT_DAMAGED);
            TriggerAddAction(DamageTrig, DamageHandler);
            UnitAddAbility(u, FourCC("ARDP")); //arm decimal
            BlzUnitHideAbility(u, FourCC("ARDP"), true);
            u = null;
            Interfaces = new NoxUnitMethods()
            {
                Regenerate = () =>
                {
                    Heal(RegenFlat * RegenerationTimeout);
                    ReplenishMana(RegenManaFlat * RegenerationTimeout);
                },
                AbilityDamage = () =>
                {
                    return (BlzGetUnitBaseDamage(this, 0) + 1 + GreenDamage) * DamageMultiplier;
                },
                WeaponDamage = () =>
                {
                    return (BlzGetUnitBaseDamage(this, 0) + 1 + GreenDamage) * DamageMultiplier;
                },
                CalculateTotalHP = () =>
                {
                    BlzSetUnitMaxHP(this, R2I(BaseHP * TotalHPPercent + Utils.ROUND_DOWN_CONST_OVERHEAD)); // rounding issues
                },
                AddBaseDamage = (int val) =>
                {
                    BlzSetUnitBaseDamage(_Self, GetBaseDamage() + val, 0);
                    SetGreenDamage(BonusDamage + R2I(BlzGetUnitBaseDamage(_Self, 0) * BonusDamagePercent));
                },
                AddBonusDamage = (int val) =>
                {
                    BonusDamage += val;
                    SetGreenDamage(BonusDamage + R2I(BlzGetUnitBaseDamage(_Self, 0) * BonusDamagePercent + Utils.ROUND_DOWN_CONST_OVERHEAD));

                },
                DeattachClass = () =>
                {
                    OnHits.Clear();
                    //AmHits.Clear();
                    DestroyTrigger(DamageTrig);
                    Indexer.Remove(GetHandleId(_Self));
                    OnHits = null;
                    Statuses = null;
                    RemoveUnit(this);
                    DamageTrig = null;
                },
                AddGreyArmor = (float val) =>
                {
                    SetGreenArmor(GreyArmor += val);
                },
                AddMoveSpeedpercent = (float val) =>
                {
                    MovementSpeedPercent += val;
                    SetUnitMoveSpeed(this, BaseMovementSpeed * MovementSpeedPercent);
                },
                AddBaseMoveSpeed = (float val) =>
                {
                    BaseMovementSpeed += val;
                    SetUnitMoveSpeed(this, BaseMovementSpeed * MovementSpeedPercent);
                },
                CalculateTotalMana
            };
        }

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

            // Deattach when unit leaves the map
            TriggerRegisterLeaveRegion(CreateTrigger(), reg, Filter(() => { ((NoxUnit)GetLeavingUnit()).Interfaces.DeattachClass(); return false; })); // catch unit removal, destroy everything attached
            // Utility functions
            TimerStart(CreateTimer(), RegenerationTimeout, true, () => { foreach (NoxUnit ue in Indexer.Values) ue.Interfaces.Regenerate(); });

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
            //if (!Indexer.ContainsKey(GetHandleId(u))) return; // this is a very weird thing to happen, but will happen for Neutrals so yeah


            if (u.Corpse) return;
            killer.Interfaces.OnKill();
            if (u.Interfaces.OnDeath(killer))
            {
                u.Interfaces.DeattachClass();
                return;
            }
            foreach (Status st in u.Statuses.Values)
                st.Remove();
            u.Statuses.Clear();// just in case

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return; // wat
            if (IsUnitType(u, UNIT_TYPE_HERO)) return; // always leak heroes

            u.Corpse = true;

            Utils.DelayedInvoke(KeepCorpsesFor, () => { u.Interfaces.DeattachClass(); }); // ah shiet, change to let resurrections
            //ue = null;
        }
        /// <summary>
        /// Call this to remove unit instantly.
        /// Not calling will leak.
        /// </summary>
        public void GracefulRemove()
        {
            Interfaces.DeattachClass();
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
            float dmg = source.Interfaces.WeaponDamage();

            source.DealPhysicalDamage(this, dmg, true, true, false, Ranged);
        }
        /// <summary>
        /// Ensure all damage goes through this.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        public void Damage(NoxUnit target, float damage)
        {
            DamageEngineIgnore = true;
            UnitDamageTarget(this, target, damage, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null);
            DamageEngineIgnore = false;
            if (IsUnitDeadBJ(target)) AwaitRemoval(target, this);// weird bug BECAUSE WC3REFUNDED SHIT HI-HI
        }

        /// <summary>
        /// Damage parsers that takes care of all calculations. Damage parser calculates outgoing damage from the unit.
        /// </summary>
        /// <param name="target">Whos is the target</param>
        /// <param name="damage"></param>
        /// <param name="triggerOnHit">Does it apply on-hit effects?</param>
        /// <param name="canCrit">Can it crit?</param>
        public void DealPhysicalDamage(NoxUnit target, float damage, bool triggerOnHit, bool canCrit, bool isSpell, bool isRanged)
        {
            location loc = Location(GetUnitX(target) + GetRandomReal(0, 10), GetUnitY(target) + GetRandomReal(0, 5));
            float pars = damage;
            if (!isSpell && !isRanged)
                if (GetRandomReal(0, 1) < target.Block)
                {
                    pars *= 0.05f;
                    Utils.DamageTextDirectionRandom("BLOCK!", loc, 9f, 75, 123, 189, 0, 1.5f, GetOwningPlayer(this), GetOwningPlayer(target));
                }
            // maths
            pars *= (1 - Math.Min(target.DamageReduction, 1));
            float armor = BlzGetUnitArmor(target);
            if (armor < 0)
                pars *= (1.71f - Pow(1f - ARMOR_CONST, -armor)); // war3 real armor reduction is 1.71-pow(xxx) - why? - no idea
            else pars *= 1 / (1 + armor * ARMOR_CONST * (1 - ArmorPenetration)); // Inverse armor reduction function, got by solving: Armor * CONST / (1 + ARMOR * CONST)

            pars = target.Interfaces.OnPhysicalDamage(this, damage, pars, triggerOnHit, canCrit, isSpell, isRanged);
            // The logic

            if (canCrit && GetRandomReal(0, 1) < CritChance)
            {
                pars *= CritDamage;

                Utils.DamageTextDirectionRandom(Utils.NotateNumber(R2I(pars)), loc, 7f, 255, 0, 0, 0, 1.5f, GetOwningPlayer(this), GetOwningPlayer(target));
            }
            else
            {
                Utils.DamageTextDirectionRandom(Utils.NotateNumber(R2I(pars)), loc, 5.9f, 255, 255, 255, 0, 1, GetOwningPlayer(this), GetOwningPlayer(target));
            }

            Damage(target, pars);
            if (isSpell) Heal(pars * SpellVamp);
            else Heal(pars * Lifesteal);
            // Now that's done
            // Onhits
            if (triggerOnHit)
            {
                List<OnHit> onhits = new List<OnHit>(OnHits.Values);
                foreach (OnHit onhit in onhits)
                    onhit.ApplyOnHit(this, target, damage, pars);
                //ApplyAmHits(source);
            }

            // cleanup
            RemoveLocation(loc);
            loc = null;
        }
        public float GetBaseMana() => BaseMana;
        public void SetBaseMana(float val)
        {
            BaseMana = val;
            Interfaces.CalculateTotalMana();
        }
        public void AddBaseMana(float val)
        {
            BaseMana += val;
            Interfaces.CalculateTotalMana();
        }
        public float GetBonusManaPercent() => TotalManaPercent;
        public void SetBonusManaPercent(float percent)
        {
            TotalHPPercent = percent;
            Interfaces.CalculateTotalMana();
        }
        public float GetBaseHP() => BaseHP;
        public void SetBaseHP(float val)
        {
            BaseHP = val;
            Interfaces.CalculateTotalHP();
        }
        public void AddBaseHP(float val)
        {
            BaseHP += val;
            Interfaces.CalculateTotalHP();
        }
        public float GetBonusHPPercent() => TotalHPPercent;
        public void SetBonusHPPercent(float percent)
        {
            TotalHPPercent = percent;
            Interfaces.CalculateTotalHP();
        }
        public float GetAttackReload() => BlzGetUnitAttackCooldown(_Self, 0);
        public float GetAttackCooldown() => BaseAttackCooldown;
        public void MultAttackCooldown(float val)
        {

            if (val < 0)
            {
                BaseAttackCooldown /= (1 + val);
            }
            else
            {
                BaseAttackCooldown *= (1 - val);
            }
            BlzSetUnitAttackCooldown(_Self, BaseAttackCooldown / AttackSpeed, 0);
        }
        public float GetAttackSpeed() => AttackSpeed;
        /// <summary>
        /// 5% = 0.05f
        /// </summary>
        /// <param name="attackSpeed"></param>
        public void AddAttackSpeed(float attackSpeed)
        {
            AttackSpeed += attackSpeed;
            BlzSetUnitAttackCooldown(_Self, BaseAttackCooldown / AttackSpeed, 0);
        }
        public int GetBaseDamage() => BlzGetUnitBaseDamage(_Self, 0);
        public int GetBonusDamage() => BonusDamage;

        public float GetBonusDamageMultiplier() => BonusDamagePercent;

        public void AddBonusDamagePercent(float val)
        {
            BonusDamagePercent += val;
            SetGreenDamage(BonusDamage + R2I(BlzGetUnitBaseDamage(_Self, 0) * BonusDamagePercent + Utils.ROUND_DOWN_CONST_OVERHEAD));
        }
        /// <summary>
        /// Get the +xxx on unit.
        /// </summary>
        /// <returns></returns>
        public int GetGreenDamage() => GreenDamage;
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
        public float GetGreyArmor() => GreyArmor;
        public void SetGreyArmor(float val)
        {
            GreyArmor = val;
            BlzSetUnitArmor(_Self, val + GreenArmor);
        }
        /// <summary>
        /// Get units's -xxx armor.
        /// </summary>
        /// <returns></returns>
        public float GetGreenArmor() => GreenArmor;
        /// <summary>
        /// Set unit's -xxx.x or +xxx.x armor. Does support single precision decimal point.
        /// </summary>
        /// <param name="val"></param>
        public void SetGreenArmor(float val)
        {
            int leftover = R2I(val);
            int decimals = R2I((val - R2I(val)) * 10);
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
        public void RemoveStatus(int id)
        {
            Statuses.Remove(id);
        }
        public Status GetStatus(int id)
        {
            return Statuses[id];
        }
        /// <summary>
        /// Heal unit by % missing hp. Unit with 50% HP and 10% Missing Healing will receive effective 5% heal.
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealPercentMissing(float percentHealed, bool show = false)
        {
            Heal(percentHealed * (BlzGetUnitMaxHP(this) - GetWidgetLife(this)), show);
        }
        /// <summary>
        /// Simple function that heals a unit by percentHealed (%) of Max HP.<para />
        /// Range of percentHealed: 0.00 - 1.00
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealPercentMax(float percentHealed, bool show = false)
        {
            Heal(percentHealed * BlzGetUnitMaxHP(this), show);
        }
        /// <summary>
        /// Simple function that heals a unit by howMuch amount (flat).
        /// </summary>
        /// <param name="howMuch"></param>
        public virtual void Heal(float howMuch, bool show = false)
        {
            float pars = howMuch * HealingFromAllSources;
            SetWidgetLife(this, GetWidgetLife(this) + pars);
            if (show)
            {
                location loc = Location(GetUnitX(this) + GetRandomReal(0, 10), GetUnitY(this) + GetRandomReal(0, 5));
                Utils.DamageTextDirectionRandom("+" + Utils.NotateNumber(R2I(pars)), loc, 7f, 55, 255, 55, 0, 1.2f, GetOwningPlayer(this), null);
                RemoveLocation(loc);
                loc = null;
            }
        }
        public virtual void ReplenishMana(float howMuch)
        {
            SetUnitManaBJ(this, GetUnitState(this, UNIT_STATE_MANA) + howMuch * ManaFromAllSources);
        }
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
}
