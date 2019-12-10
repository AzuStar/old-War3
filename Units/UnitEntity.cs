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
        /// Negative and positive green armor.
        /// </summary>
        private int GreenArmor = 0;
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

            foreach (Status s in ue.Statuses.Values)
                s.Remove();
            ue.Statuses.Clear();// just in case

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
            Statuses = null;
            RemoveUnit(this);
            UnitRef = null;
            DamageTrig = null;
            DeathTrig = null;
        }


        // ****************
        // * Unit Methods *
        // ****************
        private void DamageHandler()
        {
            if (DamageEngineIgnore) return;
            if (GetEventDamage() < 1) return;
            UnitEntity source = Cast(GetEventDamageSource());
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.WeapondDamage();

            source.DealPhysicalDamage(this, dmg, true, true, false);
        }
        protected void Damage(unit target, float damage)
        {
            DamageEngineIgnore = true;
            UnitDamageTarget(this, target, damage, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null);
            DamageEngineIgnore = false;
        }
        /// <summary>
        /// Damage parsers that takes care of all calculations. Damage parser calculates outgoing damage from the unit.
        /// </summary>
        /// <param name="target">Whos is the target</param>
        /// <param name="damage"></param>
        /// <param name="onHit">Does it apply on-hit effects?</param>
        /// <param name="crit">Can it crit?</param>
        public virtual void DealPhysicalDamage(UnitEntity target, float damage, bool onHit, bool crit, bool spell)
        {
            location loc = Location(GetUnitX(target) + GetRandomReal(0, 20), GetUnitY(target) + GetRandomReal(0, 10));

            if (!spell)
                if (GetRandomReal(0, 1) < target.Block)
                {
                    damage *= 0.05f;
                    Utils.RandomDirectedFloatText("BLOCK!", loc, 9f, 75, 123, 189, 255, 1.5f);
                }
            // maths
            float pars = damage * DamageMultiplier * (1 - Math.Min(target.DamageReduction, 1));
            float armor = BlzGetUnitArmor(target);
            if (armor < 0)
                pars *= (1.71f - Pow(1f - ARMOR_CONST, -armor)); // war3 real armor reduction is 1.71 - xxx
            else pars *= 1 / (1 + armor * ARMOR_CONST * (1 - ArmorPenetration)); // Inverse armor reduction function, got by solving: Armor * CONST / (1 + ARMOR * CONST)

            // The logic

            if (crit && GetRandomReal(0, 1) < CritChance)
            {
                pars *= CritDamage;

                if (GetLocalPlayer() == GetOwningPlayer(this) || GetLocalPlayer() == GetOwningPlayer(target))
                {
                    Utils.RandomDirectedFloatText(Utils.NotateNumber(R2I(pars)), loc, 9f, 255, 0, 0, 0, 1.5f);
                }
            }
            else
            {
                if (GetLocalPlayer() == GetOwningPlayer(this) || GetLocalPlayer() == GetOwningPlayer(target))
                {
                    Utils.RandomDirectedFloatText(Utils.NotateNumber(R2I(pars)), loc, 7.9f, 255, 255, 255, 0, 1);
                }
            }

            Damage(target.UnitRef, pars);
            if (spell) Heal(pars * SpellVamp);
            else Heal(pars * Lifesteal);
            // Now that's done
            // Onhits
            if (onHit)
            {
                foreach (OnHit onhit in OnHits)
                    onhit.ApplyOnHit(this, target);
                //ApplyAmHits(source);
            }

            // cleanup
            RemoveLocation(loc);
            loc = null;
        }
        public virtual float AbilityDamage()
        {
            return (BlzGetUnitBaseDamage(this, 0) + 1 + BonusDamage) * DamageMultiplier;
        }
        public virtual float WeapondDamage()
        {
            return (BlzGetUnitBaseDamage(this, 0) + 1 + BonusDamage) * DamageMultiplier;
        }
        /// <summary>
        /// Override to your needs.
        /// This sets new total HP.
        /// Don't forget float poor precision offset.
        /// </summary>
        protected virtual void CalculateTotalHP()
        {
            BlzSetUnitMaxHP(this, R2I(BaseHP * TotalHPPercent + 0.19f));
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
                UnitRemoveAbility(this, Abilities_BonusDamage[i]);
                int comparator = R2I(Pow(2, i));
                if (comparator <= val)
                {
                    UnitAddAbility(this, Abilities_BonusDamage[i]);
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
        public int GetGreenArmor() => GreenArmor;
        /// <summary>
        /// Set unit's -xxx or +xxx armor. Does not support decimal point yet.
        /// </summary>
        /// <param name="val"></param>
        public void SetGreenArmor(int val)
        {
            GreenArmor = val;
            foreach (int abil in Abilities_BonusArmor)
                UnitRemoveAbility(this, abil);
            foreach (int abil in Abilities_Corruption)
                UnitRemoveAbility(this, abil);
            if (val < 0)
            {
                val = -val;
                for (int i = Abilities_Corruption.Length - 1; i >= 0; i--)
                {
                    int comparator = R2I(Pow(2, i));
                    if (comparator <= val)
                    {
                        UnitAddAbility(this, Abilities_Corruption[i]);
                        val -= comparator;
                    }
                }
            }
            else
                for (int i = Abilities_BonusArmor.Length - 1; i >= 0; i--)
                {
                    int comparator = R2I(Pow(2, i));
                    if (comparator <= val)
                    {
                        UnitAddAbility(this, Abilities_BonusArmor[i]);
                        val -= comparator;
                    }
                }
        }
        public bool ContainsStatus(int id)
        {
            //if(st.GetHashCode()<=100)
            return Statuses.ContainsKey(id);
        }
        public Status AddStatus(int id, Status toAdd)
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
            Heal(percentHealed * (BlzGetUnitMaxHP(this) - GetWidgetLife(this)));
        }
        /// <summary>
        /// Simple function that heals a unit by percentHealed (%) of Max HP.<para />
        /// Range of percentHealed: 0.00 - 1.00
        /// </summary>
        /// <param name="percentHealed"></param>
        public void HealPercentMax(float percentHealed)
        {
            Heal(percentHealed * BlzGetUnitMaxHP(this));
        }
        /// <summary>
        /// Simple function that heals a unit by howMuch amount (flat).
        /// </summary>
        /// <param name="howMuch"></param>
        public void Heal(float howMuch)
        {
            SetWidgetLife(this, GetWidgetLife(this) + howMuch * HealingFromAllSources);
        }
        public void AddMana(float howMuch)
        {
            SetUnitManaBJ(this, GetUnitState(this, UNIT_STATE_MANA) + howMuch * ManaBonusPercent);
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
        public static implicit operator unit(UnitEntity ue)
        {
            return ue.UnitRef;
        }

    }
}
