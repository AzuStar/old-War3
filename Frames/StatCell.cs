using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NoxRaven.Frames;
using static NoxRaven.UnitAgents.EUnitState;
using static War3Api.Common;

namespace NoxRaven.Frames
{
    /// <summary>
    /// Position map:<br/>
    /// [big 0 _ big 3] [small 0 _ small 4]
    /// [big 1 _ big 4] [small 1 _ small 5]
    /// [big 2 _ big 5] [small 2 _ small 6]
    /// [_____________] [small 3 _ small 7]
    /// </summary>
    public class StatCell
    {
        public int arraypos;
        public framehandle iconFrame;
        public framehandle textFrame;
        public framehandle buttonFrame;
        public Tooltip tooltip;
        public Action<StatCell, NAgent> updateCallback = null;

        // hide stat creation
        private StatCell(int iterator)
        {
            arraypos = iterator;
            if (iterator < 6)
            {
                buttonFrame = BlzGetFrameByName("NU_IPE_BigButton" + iterator, 0);
                iconFrame = BlzGetFrameByName("NU_IPE_BigButtonIcon" + iterator, 0);
                textFrame = BlzGetFrameByName("NU_IPE_BigButtonText" + iterator, 0);
                BlzFrameSetParent(buttonFrame, s_statsFrame);
                tooltip = new Tooltip(buttonFrame, null);
            }
            else
            {
                iterator -= 6;
                buttonFrame = BlzGetFrameByName("NU_IPE_SmallButton" + iterator, 0);
                iconFrame = BlzGetFrameByName("NU_IPE_SmallButtonIcon" + iterator, 0);
                textFrame = BlzGetFrameByName("NU_IPE_SmallButtonText" + iterator, 0);
                BlzFrameSetParent(buttonFrame, s_statsFrame);
                tooltip = new Tooltip(buttonFrame, null);
            }

            // BlzTriggerRegisterFrameEvent(s_tooltipTrig, frame, FRAMEEVENT_CONTROL_CLICK);
            // UnitInfoPanelAddTooltipListener(tooltip, function(unit) return Data[int][1] .. BlzFrameGetText(frameObject[int].Text).."\n"..Data[int][3] end)
            // s_tooltipListener.Add(delegate(framehandle tooltip)
            // {
            //     return Data[i][1] + BlzFrameGetText(textFrame) + "\n" + Data[i][3];
            // });
            // function UnitInfoPanelAddTooltipListener(frame, code)
            //     if not tooltipListener[frame] then
            //         table.insert(tooltipListener, frame)
            //         tooltipListener[frame] = code
            //     end
            // end
        }

        public static void DefineBigStat(
            int pos,
            string iconTexture,
            Action<StatCell, NAgent> update,
            Func<NAgent, string> tooltipUpdate
        )
        {
            StatCell cell = s_statCells[pos];
            BlzFrameSetTexture(cell.iconFrame, iconTexture, 0, false);
            cell.updateCallback = update;
            cell.tooltip.updateCallback = tooltipUpdate;
        }

        public static void DefineSmallStat(
            int pos,
            string iconTexture,
            Action<StatCell, NAgent> update,
            Func<NAgent, string> tooltipUpdate
        )
        {
            StatCell cell = s_statCells[pos + 6];
            BlzFrameSetTexture(cell.iconFrame, iconTexture, 0, false);
            cell.updateCallback = update;
            cell.tooltip.updateCallback = tooltipUpdate;
        }

        private static void MakeSub(framehandle frame)
        {
            BlzFrameSetParent(frame, s_parent);
        }

        public static group s_group = CreateGroup();
        public static unit s_unit = null;

        public static framehandle s_parent;
        public static framehandle s_unitInfoFrame;
        public static framehandle s_statsFrame;

        public static StatCell[] s_statCells = new StatCell[14];

        // this will be implementation of function Init()
        public static void InitCustomInfoPanel()
        {
            s_unitInfoFrame = BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0);
            s_parent = BlzCreateFrameByType("SIMPLEFRAME", "", s_unitInfoFrame, "", 0);
            // hide default info
            // do not destroy or move (or anything)
            // attach to dummy parent and hide the parent
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconDamage", 0));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconDamage", 1));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconArmor", 2));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconRank", 3));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconFood", 4));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconGold", 5));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconHero", 6));
            MakeSub(BlzGetFrameByName("SimpleInfoPanelIconAlly", 7));
            BlzFrameSetVisible(s_parent, false);

            s_parent = BlzGetFrameByName("ConsoleUIBackdrop", 0);

            s_statsFrame = BlzCreateSimpleFrame(
                "NU_IP_Stats",
                BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0),
                0
            );
            BlzFrameSetParent(s_statsFrame, BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0));

            for (int i = 0; i < s_statCells.Length; i++)
            {
                s_statCells[i] = new StatCell(i);
            }

            InitDefaultStats();

            // run update timer
            // this is real time, at max of 60 FPS
            Master.s_globalTick.Add(
                (delta) =>
                {
                    // problematic, because a lot of things can backfire
                    try
                    {
                        if (BlzFrameIsVisible(s_unitInfoFrame))
                        {
                            NAgent u = Master.GetSelectedUnit();
                            if (u != null)
                                foreach (StatCell stframe in s_statCells)
                                {
                                    if (stframe.updateCallback != null)
                                        stframe.updateCallback(stframe, u);
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        // Utils.Debug(e.Message);
                    }
                }
            );
        }

        public static void InitDefaultStats()
        {
            StatCell.DefineBigStat(
                0,
                "noxraven\\icon_DMG.blp",
                (cell, u) =>
                {
                    string val =
                        u.state[GREY_ATK] + UIUtils.BonusStringFromValue(u.state[GREEN_ATK]);
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val =
                        u.state[GREY_ATK] + UIUtils.BonusStringFromValue(u.state[GREEN_ATK]);
                    return "Damage: "
                        + u.state[ATK]
                        + " = ("
                        + val
                        + ")\n\nYour basic attacks deal this much damage.";
                }
            );
            StatCell.DefineBigStat(
                1,
                "noxraven\\icon_ARM.blp",
                (cell, u) =>
                {
                    string val =
                        u.state[GREY_ARMOR] + UIUtils.BonusStringFromValue(u.state[GREEN_ARMOR]);
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val =
                        u.state[GREY_ARMOR] + UIUtils.BonusStringFromValue(u.state[GREEN_ARMOR]);
                    return "Armor: "
                        + u.state[ARMOR]
                        + " = ("
                        + val
                        + ")\n"
                        + "Physical Damage Reduction: "
                        + R2SW(
                            100 - UnitUtils.GetDamageReductionFromArmor(u.state[ARMOR]) * 100,
                            2,
                            2
                        )
                        + "%\n\nPhysical damage that you take will be reduced the higher the armour of your character is.";
                }
            );
            StatCell.DefineBigStat(
                2,
                "noxraven\\icon_AS.blp",
                (cell, u) =>
                {
                    string val;
                    if (u.state[RELOAD_TIME] <= 0.001f)
                        val = "0.00";
                    else
                    {
                        float aps = 1 / u.state[RELOAD_TIME];
                        val = R2SW(aps, 2, 2); //.ToString("0.00");
                    }
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val;
                    if (u.state[RELOAD_TIME] <= 0.001f)
                        val = "0.00";
                    else
                    {
                        float aps = 1 / u.state[RELOAD_TIME];
                        val = R2SW(aps, 2, 2); //.ToString("0.00");
                    }
                    return "Attacks Per Second: "
                        + val
                        + "\n"
                        + "Base Attack Cooldown: "
                        + BlzGetUnitAttackCooldown(u, 0)
                        + "sec"
                        + "\n"
                        + "Attack Speed: "
                        + u.state[ATTACK_SPEED]
                        + "%\n\n"
                        + "This is how often your character can attack.";
                }
            );

            StatCell.DefineBigStat(
                3,
                "noxraven\\icon_AP.blp",
                (cell, u) =>
                {
                    string val = u.state[GREY_AP] + UIUtils.BonusStringFromValue(u.state[GREEN_AP]);
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val = u.state[GREY_AP] + UIUtils.BonusStringFromValue(u.state[GREEN_AP]);
                    return "Ability Power: "
                        + u.state[AP]
                        + " = ("
                        + val
                        + ")"
                        + "\n\n"
                        + "Ability Power impoves abilities in various way.";
                }
            );
            StatCell.DefineBigStat(
                4,
                "noxraven\\icon_MR.blp",
                (cell, u) =>
                {
                    string val =
                        u.state[GREY_MAGIC_RESIST]
                        + UIUtils.BonusStringFromValue(u.state[GREEN_MAGIC_RESIST]);
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val =
                        u.state[GREY_MAGIC_RESIST]
                        + UIUtils.BonusStringFromValue(u.state[GREEN_MAGIC_RESIST]);
                    return "Magic Resist: "
                        + u.state[MAGIC_RESIST]
                        + " = ("
                        + val
                        + ")\n"
                        + "Magical Damage Reduction: "
                        + R2SW(
                            100
                                - UnitUtils.GetDamageReductionFromArmor(u.state[MAGIC_RESIST])
                                    * 100,
                            2,
                            2
                        )
                        + "%\n\n"
                        + "Magical and spell damage that you take will be reduced the higher the armour of your character is.";
                }
            );
            StatCell.DefineBigStat(
                5,
                "noxraven\\icon_SPD.blp",
                (cell, u) =>
                {
                    string val;
                    int ms = R2I(GetUnitMoveSpeed(u));
                    if (ms > 50)
                        val = ms.ToString();
                    else
                        val = "0";
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val;
                    int ms = R2I(GetUnitMoveSpeed(u));
                    if (ms > 50)
                        val = ms.ToString();
                    else
                        val = "0";
                    return "Movement Speed: "
                        + val
                        + " = ("
                        + u.state[MOVEMENT_SPEED]
                        + UIUtils.BonusStringFromValue(
                            u.state[BASE_MOVE_SPEED] * u.state[BASE_MOVE_SPEED_PERCENT_BASE]
                        )
                        + ")\n\n"
                        + "Movement speed is how fast character is moving";
                }
            );

            StatCell.DefineSmallStat(
                0,
                "noxraven\\icon_lifesteal.blp",
                (cell, u) =>
                {
                    string val = u.state[LIFESTEAL] * 100 + "%";
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val = u.state[LIFESTEAL] * 100 + "%";
                    return "Life Vamp: "
                        + val
                        + " = ("
                        + u.state[LIFESTEAL]
                        + ")\n\n"
                        + "All basic attacks and on-hit effects will heal your character by percent of damage dealt.";
                }
            );
            StatCell.DefineSmallStat(
                4,
                "noxraven\\icon_spellvamp.blp",
                (cell, u) =>
                {
                    string val = u.state[SPELLVAMP] * 100 + "%";
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val = u.state[SPELLVAMP] * 100 + "%";
                    return "Spell Vamp: "
                        + val
                        + " = ("
                        + u.state[SPELLVAMP]
                        + ")\n\n"
                        + "All spell damage will heal your character by percent of damage dealt.";
                }
            );
            StatCell.DefineSmallStat(
                6,
                "noxraven\\icon_HPregen.blp",
                (cell, u) =>
                {
                    string val = (u.state[HP_REG]) + "";
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val = u.state[HP_REG] + "";
                    string incomingHeal = UIUtils.BonusStringFromValue(u.state[INCOMING_HEALING]);
                    return "HitPoints Regeneration: "
                        + val
                        + " = ("
                        + u.state[BASE_HP_REGEN]
                        + UIUtils.BonusStringFromValue(
                            u.state[BASE_HP_REGEN] * u.state[BASE_HP_PERCENT_BASE]
                        )
                        + ")"
                        + "\n"
                        + "Incoming Healing: "
                        + (String.IsNullOrEmpty(incomingHeal) ? "0" : incomingHeal)
                        + "%\n\n"
                        + "This indicates how much your character regenerates health per second."
                        + "\n"
                        + "Incoming healing is the amount of extra healing you receive from all sources.";
                }
            );
            StatCell.DefineSmallStat(
                7,
                "noxraven\\icon_MPregen.blp",
                (cell, u) =>
                {
                    string val = (u.state[MP_REG]) + "";
                    BlzFrameSetText(cell.textFrame, val);
                },
                (u) =>
                {
                    string val = u.state[MP_REG] + "";
                    string incomingMana = UIUtils.BonusStringFromValue(u.state[INCOMING_MANA]);
                    return "Mana Regeneration: "
                        + val
                        + " = ("
                        + u.state[BASE_MP_REGEN]
                        + UIUtils.BonusStringFromValue(
                            u.state[BASE_MP_REGEN] * u.state[BASE_MP_REGEN_PERCENT_BASE]
                        )
                        + ")"
                        + "\n"
                        + "Incoming Mana: "
                        + (String.IsNullOrEmpty(incomingMana) ? "0" : incomingMana)
                        + "%\n\n"
                        + "This indicates how much your character replenishes mana per second."
                        + "\n"
                        + "Incoming mana is the amount of extra mana you receive from all sources.";
                }
            );

            StatCell.DefineSmallStat(
                2,
                "noxraven\\icon_SPD.blp",
                (cell, u) =>
                {
                    BlzFrameSetVisible(cell.buttonFrame, false);
                },
                null
            );
            StatCell.DefineSmallStat(
                3,
                "noxraven\\icon_SPD.blp",
                (cell, u) =>
                {
                    BlzFrameSetVisible(cell.buttonFrame, false);
                },
                null
            );
            StatCell.DefineSmallStat(
                1,
                "noxraven\\icon_SPD.blp",
                (cell, u) =>
                {
                    BlzFrameSetVisible(cell.buttonFrame, false);
                },
                null
            );
            StatCell.DefineSmallStat(
                5,
                "noxraven\\icon_SPD.blp",
                (cell, u) =>
                {
                    BlzFrameSetVisible(cell.buttonFrame, false);
                },
                null
            );
        }
    }
}
