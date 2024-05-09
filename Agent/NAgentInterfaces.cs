using System;
using System.Diagnostics;
using NoxRaven.Data;
using NoxRaven.Events;
using NoxRaven.UnitAgents;
using static NoxRaven.UnitAgents.EUnitState;
using static War3Api.Blizzard;
using static War3Api.Common;

namespace NoxRaven
{
    // protected and virtual fields/methods
    public partial class NAgent
    {
        public NAgent(unit u, NUnitInitializer initializer = null)
        {
            wc3agent = u;
            if (initializer != null)
            {
                if (initializer.starterStats != null)
                {
                    state.ResetFromModifier(initializer.starterStats);
                }
                if (initializer.starterAbilities != null)
                    foreach (NAbility ab in initializer.starterAbilities)
                    {
                        AddAbility(ab);
                    }

                if (initializer.constructor != null)
                {
                    initializer.constructor(this);
                }
            }
            else
            {
                _SetStarterStats();
            }
            state.AddListener(
                MAX_HP,
                (evt) =>
                {
                    BlzSetUnitMaxHP(this, R2I(evt.stackedValue + Utils.ROUND_DOWN_CONST_OVERHEAD));
                }
            );
            // max_mp
            state.AddListener(
                MAX_MP,
                (evt) =>
                {
                    BlzSetUnitMaxMana(
                        this,
                        R2I(evt.stackedValue + Utils.ROUND_DOWN_CONST_OVERHEAD)
                    );
                }
            );
            state.AddListener(
                MOVEMENT_SPEED,
                (evt) =>
                {
                    SetUnitMoveSpeed(wc3agent, evt.stackedValue);
                }
            );
            state.AddListener(
                RELOAD_TIME,
                (evt) =>
                {
                    BlzSetUnitAttackCooldown(wc3agent, evt.stackedValue, 0);
                }
            );
            // when unit indexed (aka created) it must have full vitals
            SetUnitState(u, UNIT_STATE_LIFE, 999999999);
            SetUnitState(u, UNIT_STATE_MANA, 999999999);

            _dmgHookTrig = CreateTrigger();
            TriggerRegisterUnitEvent(_dmgHookTrig, u, EVENT_UNIT_DAMAGED);
            TriggerAddAction(_dmgHookTrig, DamageHandler);

            _attackHookTrig = CreateTrigger();
            TriggerRegisterUnitEvent(_attackHookTrig, u, EVENT_UNIT_ATTACKED);
            TriggerAddAction(
                _attackHookTrig,
                () =>
                {
                    NAgent attacker = GetAttacker();
                    OnAttack attackEvt = new OnAttack() { caller = attacker, target = this };
                    attacker.TriggerEvent(attackEvt);
                }
            );

            _spellEffectTrig = CreateTrigger();
            TriggerRegisterUnitEvent(_spellEffectTrig, u, EVENT_UNIT_SPELL_EFFECT);
            TriggerAddAction(_spellEffectTrig, () => { });
        }
    }
}
