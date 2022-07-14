using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven;
using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven
{
    public class NoxAbility
    {
        public static trigger tr;

        protected static Dictionary<int, NoxAbility> s_index = new Dictionary<int, NoxAbility>();

        public static void RegisterAbility(string v, Action p)
        {
            throw new NotImplementedException();
        }

        public int id;
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
            id = spellId;
            AbilityStruct = abill;
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
