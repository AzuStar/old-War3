using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven;
using NoxRaven.Units;
using static War3Api.Common;

namespace NoxRaven
{
    public class Spell
    {
        public static Dictionary<int, Spell> Indexer = new Dictionary<int, Spell>();
        public int SpellId;
        public Spell(string spellStringId)
        {
            SpellId = FourCC(spellStringId);

            Indexer.Add(SpellId, this);
        }

        public void AddSpellToUnit(NoxUnit target)
        {
            UnitAddAbility(target.UnitRef, SpellId);
        }

        /// <summary>
        /// Do this for active spells only (or special spells, I guess?)
        /// </summary>
        /// <param name="effect"></param>
        public void RegisterEffect(Action effect)
        {
            trigger tr = CreateTrigger();
            foreach (NoxPlayer p in NoxPlayer.AllPlayers)
                TriggerRegisterPlayerUnitEvent(tr, p.PlayerRef, EVENT_PLAYER_UNIT_SPELL_EFFECT, null);
            // check if correct spell fired
            TriggerAddCondition(tr, Filter(() => GetSpellAbilityId() == SpellId));
            // this is truly amazing how you can add delegates with this thing
            TriggerAddAction(tr, effect);
        }
    }
}
