using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Events.EventTypes;

namespace NoxRaven
{
    public class NoxAbility
    {
        public static trigger tr;

        protected static Dictionary<int, NoxAbility> Indexer = new Dictionary<int, NoxAbility>();
        private static Dictionary<int, Type> CustomTypes = new Dictionary<int, Type>();
        public int AbilityID;
        public ability AbilityStruct;

        //public static void InitAbilityLogic()
        //{
        //    tr = CreateTrigger();
        //    TriggerRegisterAnyUnitEventBJ(tr, EVENT_PLAYER_UNIT_SPELL_CAST);
        //    TriggerAddAction(tr, () =>
        //    {
        //        if (Indexer.ContainsKey(GetSpellAbilityId()))
        //            Indexer[GetSpellAbilityId()].Callback();
        //    });
        //}
        protected NoxAbility(int spellId, ability abill)
        {
            AbilityID = spellId;
            AbilityStruct = abill;
        }
        /// <summary>
        /// Put a custom type that will be attached to an ability when indexing.
        /// Custom type has to extend this class and invoke base(spell) in the constructor.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t">hit type</param>
        public static void AddCustomType(int spellId, Type t)
        {
            if (typeof(NoxAbility).IsAssignableFrom(t))
            {
                Utils.Error("You have tried to parse type that does not inherit this class!", typeof(NoxAbility));
                return;
            }
            CustomTypes[spellId] = t;
        }
        /// <summary>
        /// Put a custom type that will be attached to a unit when indexing.
        /// Custom type has to extend this class and invoke base(u) in the constructor.
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="t"></param>
        public static void AddCustomType(string spellString, Type t)
        {
            AddCustomType(FourCC(spellString), t);
        }

        //public void AddSpellToUnit(NoxUnit target)
        //{
        //    Ability.Instance inst = new Ability.Instance(this, target);
        //}

        /// <summary>
        /// Do this for active spells only (or special spells, I guess?)
        /// </summary>
        /// <param name="effect"></param>
        //public void RegisterEffect(Action effect)
        //{
        //    trigger tr = CreateTrigger();
        //    foreach (NoxPlayer p in NoxPlayer.AllPlayers.Values)
        //        TriggerRegisterPlayerUnitEvent(tr, p._Self, EVENT_PLAYER_UNIT_SPELL_EFFECT, null);
        //    // check if correct spell fired
        //    TriggerAddCondition(tr, Filter(() => GetSpellAbilityId() == AbilityID));
        //    // this is truly amazing how you can add delegates with this thing
        //    TriggerAddAction(tr, effect);
        //}

        //public class Instance
        //{
        //    private static Action<RemovalEvent> Removal = (ev) => { ev.Target.OnRemoval -= Removal; };
        //    public Ability AbilRef;
        //    public NoxUnit Unit;
        //    public Instance(Ability abil, NoxUnit u)
        //    {
        //        AbilRef = abil;
        //        Unit = u;
        //        //UnitAddAbility(u._Self, AbilRef.AbilityID);
        //        u.OnRemoval += Removal;
        //    }
        //}
    }
}
