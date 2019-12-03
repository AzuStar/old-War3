using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NoxRaven;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven.Units
{
    /// <summary>
    /// To use in own code simply create class which has inner reference to this class.
    /// Call function <see cref="Master.RunAfterExtensionsReady"/>. It will initialize indexing logic for all units. Put it somewhere after all players and custom classes were initialized.<para></para>
    /// <b>Never RemoveUnit() indexed unit or it will permaleak.</b>
    /// </summary>
    public class UnitEntity
    {
        protected static Dictionary<int, UnitEntity> Indexer = new Dictionary<int, UnitEntity>();
        private static Dictionary<int, Type> CustomTypes = new Dictionary<int, Type>();
        private static float KeepCorpsesFor = 25;
        private static bool DamageEngineIgnore = false;
        private static float RegenerationTimeout = 0.1f;
        internal static int[] Abilities_BonusDamage = new int[19];
        internal static int[] Abilities_Corruption = new int[13];
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
        public unit UnitRef;
        private trigger DamageTrig;
        private trigger DeathTrig;

        private Dictionary<int, Status> Statuses = new Dictionary<int, Status>();

        private List<OnHit> OnHits = new List<OnHit>();
        private List<OnHit> AmHits = new List<OnHit>();
        // Unit Extra Parameters
        // ******************
        // * Damage related *
        // ******************
        /// <summary>
        /// Later would like to create projectiles when they attack.
        /// </summary>
        public bool Ranged;
        /// <summary>
        /// This is +xxx damage.
        /// </summary>
        private int BonusDamage = 0;
        /// <summary>
        /// Total damage multiplication. Multiplicative(*)
        /// </summary>
        private float DamageMultiplier = 1;
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

        public float ManaBonusPercent = 1;

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
        /// Negative armor, cannot be negative.
        /// </summary>
        private int Corruption = 0;
        /// <summary>
        /// Chance to evade projectile (any tier 1).
        /// Additive(+)
        /// Range: 0.00-1.00
        /// </summary>
        public float Evasion = 0;
        /// <summary>
        /// Chance to reduce incoming damage by 95%  applies to melee attack.
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
        public int OnHitApplied_Times = 1;


        protected UnitEntity(unit u)
        {
            UnitRef = u;
            SetBaseHP(BlzGetUnitMaxHP(u));
            Ranged = IsUnitType(u, UNIT_TYPE_RANGED_ATTACKER);
            // Damage Utilization
            DamageTrig = CreateTrigger();
            TriggerRegisterUnitEvent(DamageTrig, u, EVENT_UNIT_DAMAGED);
            TriggerAddAction(DamageTrig, DamageHandler);

            //foreach(ability a in GetUnit)

            // Recycling of dead units
            DeathTrig = CreateTrigger();
            TriggerRegisterDeathEvent(DeathTrig, u);
            TriggerAddAction(DeathTrig, () => { AwaitRemoval(UnitRef); });
            u = null;
        }
        /// <summary>
        /// Put a custom type that will be attached to a unit when indexing.
        /// Custom type has to extend this class and invoke base(u) in the constructor.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t"></param>
        public static void AddCustomType(int unitId, Type t)
        {
            if (t.IsAssignableFrom(typeof(UnitEntity)))
            {
                Utils.Error("You have tried to parse type that does not inherit this class!", typeof(UnitEntity));
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

            foreach (NoxPlayer p in NoxPlayer.AllPlayers)
            {
                // Add existing units
                GroupEnumUnitsOfPlayer(g, p.PlayerRef, Filter(AttachClass));
                GroupClear(g);
            }
            // extra neutral players
            GroupEnumUnitsOfPlayer(g, Player(PLAYER_NEUTRAL_PASSIVE), Filter(AttachClass));
            GroupClear(g);
            GroupEnumUnitsOfPlayer(g, Player(PLAYER_NEUTRAL_AGGRESSIVE), Filter(AttachClass));
            GroupClear(g);


            // Deattach when unit leaves the map
            TriggerRegisterLeaveRegion(CreateTrigger(), reg, Filter(() => { AwaitRemoval(GetLeavingUnit()); return false; }));
            // Utility functions
            TimerStart(CreateTimer(), RegenerationTimeout, true, () => { foreach (UnitEntity ue in Indexer.Values) ue.Regenerate(); });

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

            if (CustomTypes.ContainsKey(GetUnitTypeId(u))) Indexer[GetHandleId(u)] = (UnitEntity)Activator.CreateInstance(CustomTypes[GetUnitTypeId(u)], u);
            else if (IsUnitType(u, UNIT_TYPE_HERO)) Indexer[GetHandleId(u)] = new HeroEntity(u);
            else Indexer[GetHandleId(u)] = new UnitEntity(u);
            u = null;
            return false;
        }
        /// <summary>
        /// Do not call RemoveUnit on indexed unit or permaleak.
        /// </summary>
        /// <param name="u"></param>
        private static void AwaitRemoval(unit u)
        {
            if (!Indexer.ContainsKey(GetHandleId(u))) return; // this is a very weird thing to happen, but will happen for Neutrals so yeah

            UnitEntity ue = Cast(u);

            if (ue.Corpse) return;

            if (ue.OnDeath())
            {
                ue.GracefulRemove();
                return;
            }

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return;
            if (IsUnitType(u, UNIT_TYPE_HERO)) return; // always leak heroes

            ue.Corpse = true;
            Utils.DelayedInvoke(KeepCorpsesFor, () => { ue.DeattachClass(); });
            //ue = null;
        }
        /// <summary>
        /// Called when unit dies and becomes corpe.<para></para>
        /// Return true to remove it instantly and false to leave corpse.
        /// </summary>
        protected virtual bool OnDeath()
        {
            return false;
        }
        /// <summary>
        /// Call this to remove unit instantly.
        /// Not calling will leak.
        /// </summary>
        public void GracefulRemove()
        {
            DeattachClass();
        }
        protected virtual void DeattachClass()
        {
            DestroyTrigger(DamageTrig);
            DestroyTrigger(DeathTrig);
            Indexer.Remove(GetHandleId(UnitRef));
            foreach (Status s in Statuses.Values)
                s.Reset();
            Statuses.Clear();
            Statuses = null;
            RemoveUnit(UnitRef);
            UnitRef = null;
            DamageTrig = null;
            DeathTrig = null;
        }


        // ****************
        // * Unit Methods *
        // ****************
        public void DamageHandler()
        {
            if (DamageEngineIgnore) return;
            if (GetEventDamage() < 1) return;
            UnitEntity source = Cast(GetEventDamageSource());
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.WeapondDamage();

            {
                location loc = GetUnitLoc(UnitRef);

                RemoveLocation(loc);
                loc = null;
            }
            PhysicalDamageParser(source, dmg, true, true, false);
        }
        protected void Damage(unit source, float damage)
        {
            DamageEngineIgnore = true;
            UnitDamageTarget(source, UnitRef, damage, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null);
            DamageEngineIgnore = false;
        }
        /// <summary>
        /// Damage parsers that takes care of all calculations.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="damage"></param>
        /// <param name="onHit">Does it apply on-hit effects?</param>
        /// <param name="crit">Can it crit?</param>
        protected virtual void PhysicalDamageParser(UnitEntity source, float damage, bool onHit, bool crit, bool spell)
        {
            location loc = Location(GetUnitX(UnitRef) + GetRandomReal(0, 20), GetUnitY(UnitRef) + GetRandomReal(0, 10));

            if (GetRandomReal(0, 1) < Block)
            {
                damage *= 0.05f;
                Utils.RandomDirectedFloatText("BLOCK!", loc, 9f, 75, 123, 189, 255, 1.5f);
            }
            // maths
            float pars = damage * source.DamageMultiplier * (1 - Math.Min(DamageReduction, 1));
            float armor = BlzGetUnitArmor(UnitRef);
            if (armor < 0)
                pars *= (1.71f - Pow(1f - ARMOR_CONST, -armor)); // war3 real armor reduction is 1.71 - xxx
            else pars *= 1 / (1 + armor * ARMOR_CONST * (1 - source.ArmorPenetration)); // Inverse armor reduction function, got by solving: Armor * CONST / (1 + ARMOR * CONST)

            // The logic

            if (crit && GetRandomReal(0, 1) < source.CritChance)
            {
                pars *= CritDamage;

                if (GetLocalPlayer() == GetOwningPlayer(UnitRef) || GetLocalPlayer() == GetOwningPlayer(source.UnitRef))
                {
                    Utils.RandomDirectedFloatText(Utils.NotateNumber(R2I(pars)), loc, 9f, 255, 0, 0, 0, 1.5f);
                }
            }
            else
            {
                if (GetLocalPlayer() == GetOwningPlayer(UnitRef) || GetLocalPlayer() == GetOwningPlayer(source.UnitRef))
                {
                    Utils.RandomDirectedFloatText(Utils.NotateNumber(R2I(pars)), loc, 7.9f, 255, 255, 255, 0, 1);
                }
            }

            Damage(source.UnitRef, pars);
            if (spell) source.Heal(pars * source.SpellVamp);
            else source.Heal(pars * source.Lifesteal);
            // Now that's done
            // Onhits
            if (onHit)
            {
                foreach (OnHit onhit in OnHits)
                    onhit.ApplyOnHit(source, this);
                //ApplyAmHits(source);
            }

            // cleanup
            RemoveLocation(loc);
            loc = null;
        }
        public virtual float AbilityDamage()
        {
            return BlzGetUnitBaseDamage(UnitRef, 0) + 1 + BonusDamage;
        }
        public virtual float WeapondDamage()
        {
            return BlzGetUnitBaseDamage(UnitRef, 0) + 1 + BonusDamage;
        }
        /// <summary>
        /// Override to your needs.
        /// This sets new total HP.
        /// Don't forget float poor precision offset.
        /// </summary>
        protected virtual void CalculateTotalHP()
        {
            BlzSetUnitMaxHP(UnitRef, R2I(BaseHP * TotalHPPercent + 0.19f));
        }
        public float GetBaseLife() => BaseHP;
        public void SetBaseHP(float val)
        {
            BaseHP = val;
            CalculateTotalHP();
        }
        public void AddBaseHP(float val)
        {
            BaseHP += val;
            CalculateTotalHP();
        }
        public float GetBonusHPPercent() => TotalHPPercent;
        public void SetBonusHPPercent(float percent)
        {
            TotalHPPercent = percent;
            CalculateTotalHP();
        }
        /// <summary>
        /// Get the +xxx on unit.
        /// </summary>
        /// <returns></returns>
        public int GetBonusDamgae() => BonusDamage;
        /// <summary>
        /// Sets the +xxx damage on unit.
        /// </summary>
        /// <param name="i"></param>
        public void SetBonusDamage(int val)
        {
            BonusDamage = val;
            for (int i = Abilities_BonusDamage.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(UnitRef, Abilities_BonusDamage[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(UnitRef, Abilities_BonusDamage[i]);
                    val -= comparator;
                }
            }
        }
        public void AddBonusDamage(int val)
        {
            SetBonusDamage(BonusDamage + val);
        }
        /// <summary>
        /// Get units's -xxx armor.
        /// </summary>
        /// <returns></returns>
        public int GetCorruption() => Corruption;
        /// <summary>
        /// Set unit's -xxx armor. Does not support decimal point.
        /// </summary>
        /// <param name="val"></param>
        public void SetCorruption(int val)
        {
            if (val < 0) Corruption = 0;
            else Corruption = val;

            for (int i = Abilities_Corruption.Length - 1; i >= 0; i--)
            {
                UnitRemoveAbility(UnitRef, Abilities_Corruption[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(UnitRef, Abilities_Corruption[i]);
                    val -= comparator;
                }
            }
        }
        public void AddCorruption(int val)
        {
            SetCorruption(Corruption + val);
        }
        public bool ContainsStatusType(StatusType st, player p)
        {
            return Statuses.ContainsKey(st.GetHashCode() * 100 + GetPlayerId(p));
        }
        public void AddStatus(StatusType st, player p, Status toAdd)
        {
            Statuses.Add(st.GetHashCode() * 100 + GetPlayerId(p), toAdd);
        }
        public void RemoveStatus(StatusType st, player p)
        {
            Statuses.Remove(st.GetHashCode() * 100 + GetPlayerId(p));
        }
        public Status GetStatus(StatusType st, player p)
        {
            return Statuses[st.GetHashCode() * 100 + GetPlayerId(p)];
        }
        /// <summary>
        /// This is called every time regeneration handles.
        /// </summary>
        protected virtual void Regenerate()
        {
            Heal(RegenFlat * RegenerationTimeout);
            AddMana(RegenManaFlat * RegenerationTimeout);
        }
        /// <summary>
        /// Heal unit by % missing hp. Unit with 50% HP and 10% Missing Healing will receive effective 5% heal.
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealPercentMissing(float percentHealed)
        {
            Heal(percentHealed * (BlzGetUnitMaxHP(UnitRef) - GetWidgetLife(UnitRef)));
        }
        /// <summary>
        /// Simple function that heals a unit by percentHealed (%) of Max HP.<para />
        /// Range of percentHealed: 0.00 - 1.00
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealPercentMax(float percentHealed)
        {
            Heal(percentHealed * BlzGetUnitMaxHP(UnitRef));
        }
        /// <summary>
        /// Simple function that heals a unit by howMuch amount (flat).
        /// </summary>
        /// <param name="howMuch"></param>
        public void Heal(float howMuch)
        {
            SetWidgetLife(UnitRef, GetWidgetLife(UnitRef) + howMuch * HealingFromAllSources);
        }
        public void AddMana(float howMuch)
        {
            SetUnitManaBJ(UnitRef, GetUnitState(UnitRef, UNIT_STATE_MANA) + howMuch * ManaBonusPercent);
        }
        // may be implicit operator, Ive been down this road
        public static UnitEntity Cast(unit u)
        {
            return Indexer[GetHandleId(u)];
        }
        public static implicit operator UnitEntity(unit u)
        {
            return Cast(u);
        }

    }
}
