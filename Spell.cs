using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven;
using static War3Api.Common;

namespace NoxRaven
{
    public class Spell
    {
        //public static Dictionary<int, Spell> Indexer = new Dictionary<int, Spell>();
        public int SpellId;
        public Spell(string spellStringId, Action effect)
        {
            trigger tr = CreateTrigger();
            SpellId = FourCC(spellStringId);
            foreach (NoxPlayer p in NoxPlayer.AllPlayers)
                TriggerRegisterPlayerUnitEvent(tr, p.PlayerRef, EVENT_PLAYER_UNIT_SPELL_EFFECT, null);
            // check if correct spell fired
            TriggerAddCondition(tr, Filter(() => GetSpellAbilityId() == SpellId));
            // this is truly amazing how you can add delegates with this thing
            TriggerAddAction(tr, effect);
            //Indexer.Add(SpellId, this);
        }
    }
}
