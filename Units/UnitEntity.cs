using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using War3.NoxRaven;

using static War3Api.Common;
using static War3Api.Blizzard;

namespace War3.NoxRaven.Units
{
    /// <summary>
    /// To use in own code simply create class which has inner reference to this class.
    /// Call function <see cref="InitUnitLogic"/>. It will initialize indexing logic for all units. Put it somewhere after all players were initialized.
    /// </summary>
    public class UnitEntity
    {
        public static Dictionary<unit, UnitEntity> Indexer = new Dictionary<unit, UnitEntity>();
        public static float KeepCorpsesFor = 0;
        public static bool DamageEngineIgnore = false;
        public static float RegenerationTimeout = 0.1f;
        public bool Corpse = false;
        /// <summary>
        /// Unit itself.
        /// </summary>
        public unit UnitRef;
        private trigger DamageTrig;
        private trigger DeathTrig;
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
        /// Additive(+)
        /// </summary>
        private float BonusPercentHP = 0;
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
        /// This value is percentage of total health.
        /// Additive(+)
        private float BonusLifePercent = 0;
        /// <summary>
        /// Additive(+)
        /// </summary>
        public float RegenFlat = 2;
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


        //public void ddd()
        //{
        //    Utils.DisplayMessageToEveryone("k",20);
        //}

        public UnitEntity(unit u)
        {
            UnitRef = u;
            BaseHP = BlzGetUnitMaxHP(u);
            Ranged = IsUnitType(u, UNIT_TYPE_RANGED_ATTACKER);
            // Damage Utilization
            DamageTrig = CreateTrigger();
            TriggerRegisterUnitEvent(DamageTrig, u, EVENT_UNIT_DAMAGED);
            TriggerAddAction(DamageTrig, DamageHandler);

            // Recycling of dead units
            DeathTrig = CreateTrigger();
            TriggerRegisterUnitEvent(DeathTrig, u, EVENT_UNIT_DEATH);
            TriggerAddAction(DeathTrig, ParseRemovalDeath);
            u = null;
        }

        /// <summary>
        /// Put this initializer somewhere after all players have been initialized.
        /// </summary>
        public static void InitUnitLogic()
        {
            region reg = CreateRegion();
            group g = CreateGroup();
            rect rec = GetWorldBounds();
            RegionAddRect(reg, rec);
            TriggerRegisterEnterRegion(CreateTrigger(), reg, Filter(AttachClass));

            foreach (Player p in Player.AllPlayers)
            {
                // Add existing units
                GroupEnumUnitsOfPlayer(g, p.PlayerRef, Filter(AttachClass));
                GroupClear(g);
            }

            // Deattach when unit leaves the map
            TriggerRegisterLeaveRegion(CreateTrigger(), reg, Filter(ParseRemovalLeave));
            TimerStart(CreateTimer(), RegenerationTimeout, true, () => { foreach (UnitEntity ue in Indexer.Values) ue.Regenerate(); });
            // Recycle stuff
            RemoveRect(rec);
            DestroyGroup(g);
            g = null;
            rec = null;
            reg = null;
        }

        private static bool AttachClass()
        {
            unit u = GetFilterUnit();

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return false;
            if (Indexer.ContainsKey(u)) return false;

            if (IsUnitType(u, UNIT_TYPE_HERO)) Indexer[u] = new HeroEntity(u);
            else if (IsUnitType(u, UNIT_TYPE_STRUCTURE)) Indexer[u] = new BuildingEntity(u);
            else Indexer[u] = new UnitEntity(u);
            u = null;
            return false;
        }
        private static bool ParseRemovalLeave()
        {
            AwaitRemoval(GetLeavingUnit());
            return false;
        }
        private static void ParseRemovalDeath()
        {
            AwaitRemoval(GetDyingUnit());
        }
        /// <summary>
        /// Do not call RemoveUnit on indexed unit or permaleak.
        /// </summary>
        /// <param name="u"></param>
        private static void AwaitRemoval(unit u)
        {
            UnitEntity ue = Cast(u);

            if (GetUnitAbilityLevel(u, FourCC("Aloc")) > 0) return;
            if (!Indexer.ContainsKey(u)) return; // this is a very weird thing to happen, but will happen for Neutrals so yeah
            if (IsUnitType(u, UNIT_TYPE_HERO)) return; // always leak heroes
            if (ue.Corpse) return;

            ue.Corpse = true;
            Utils.DelayedInvoke(KeepCorpsesFor, ue.DeattachClass);
            ue = null;
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
            Indexer.Remove(UnitRef);
            RemoveUnit(UnitRef);
            UnitRef = null;
            DamageTrig = null;
            DeathTrig = null;
        }




        public void DamageHandler()
        {
            if (DamageEngineIgnore) return;
            if (GetEventDamage() < 1) return;
            UnitEntity source = Cast(GetEventDamageSource());
            // ~this is the target
            //if (!Ranged) // later
            float dmg = source.WeapondDamage();
            if (GetRandomReal(0, 1) < Block) dmg *= 0.05f;
            PhysicalDamageParser(source, dmg, true, true, false);
        }
        public void Damage(unit source, float damage)
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
        public void PhysicalDamageParser(UnitEntity source, float damage, bool onHit, bool crit, bool spell)
        {
            // maths
            float pars = damage * source.DamageMultiplier * (1 - Math.Min(DamageReduction, 1));
            float armor = BlzGetUnitArmor(UnitRef);
            if (armor < 0)
                pars *= (2 - Pow(0.9f, -armor));
            else pars *= 100 / (100 + armor * 6 * source.ArmorPenetration);

            // The logic
            location loc = Location(GetUnitX(UnitRef) + GetRandomReal(0, 20), GetUnitY(UnitRef) + GetRandomReal(0, 10));
            if (crit && GetRandomReal(0, 1) < source.CritChance)
            {
                pars *= CritDamage;

                Damage(source.UnitRef, pars);
                if (spell) source.Heal(pars * source.SpellVamp);
                else source.Heal(pars * source.Lifesteal);

                if (GetLocalPlayer() == GetOwningPlayer(UnitRef) || GetLocalPlayer() == GetOwningPlayer(source.UnitRef))
                {
                    texttag tt = CreateTextTagLocBJ(Utils.NotateNumber(R2I(pars)), loc, 0, 9, 255, 0, 0, 0);
                    SetTextTagVelocityBJ(tt, 40, 90 + GetRandomReal(-10, 10));
                    SetTextTagPermanent(tt, false);
                    SetTextTagFadepoint(tt, 2);
                    SetTextTagLifespan(tt, 4);
                    tt = null;
                }
            }
            else
            {
                Damage(source.UnitRef, pars);
                if (spell) source.Heal(pars * source.SpellVamp);
                else source.Heal(pars * source.Lifesteal);

                if (GetLocalPlayer() == GetOwningPlayer(UnitRef) || GetLocalPlayer() == GetOwningPlayer(source.UnitRef))
                {
                    texttag tt = CreateTextTagLocBJ(Utils.NotateNumber(R2I(pars)), loc, 0, 7.9f, 255, 255, 255, 0);
                    SetTextTagVelocityBJ(tt, 40, 90 + GetRandomReal(-10, 10));
                    SetTextTagPermanent(tt, false);
                    SetTextTagFadepoint(tt, 1);
                    SetTextTagLifespan(tt, 2);
                    tt = null;
                }
            }
            // Now that's done
            // Onhits
            if (onHit)
            {
                //source.ApplyOnHits();
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
        /// This is called every time regeneration handles.
        /// </summary>
        public virtual void Regenerate()
        {
            Heal((RegenFlat) * RegenerationTimeout);
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
        // may be implicit operator, Ive been down this road
        public static UnitEntity Cast(unit u)
        {
            return Indexer[u];
        }

    }
}
