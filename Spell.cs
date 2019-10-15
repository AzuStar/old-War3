using System;
using System.Collections.Generic;
using System.Text;
using War3.NoxRaven;
using static War3Api.Common;

namespace War3.NoxRaven
{
    public class Spell
    {
        public int SpellId;
        public Spell(string spellStringId, Action effect)
        {
            trigger tr = CreateTrigger();
            SpellId = FourCC(spellStringId);
            foreach (Player p in Player.AllPlayers)
                TriggerRegisterPlayerUnitEvent(tr, p.PlayerRef, EVENT_PLAYER_UNIT_SPELL_EFFECT, null);
            // check if correct spell fired
            TriggerAddCondition(tr, Filter(() => GetSpellAbilityId() == SpellId));
            // this is truly amazing how you can add delegates with this thing
            TriggerAddAction(tr, effect);
        }
    }
}
