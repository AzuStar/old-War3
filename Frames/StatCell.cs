using System;
using System.Collections.Generic;
using System.Linq;
using NoxRaven.Units;
using NoxRavent.Frames;
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
        public Action<StatCell, NoxUnit> updateCallback = null;

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

        public static void DefineBigStat(int pos, string iconTexture, Action<StatCell, NoxUnit> update, Func<NoxUnit, string> tooltipUpdate)
        {
            StatCell cell = s_statCells[pos];
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

            s_statsFrame = BlzCreateSimpleFrame("NU_IP_Stats", BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0), 0);
            BlzFrameSetParent(s_statsFrame, BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0));

            for (int i = 0; i < s_statCells.Length; i++)
            {
                s_statCells[i] = new StatCell(i);
            }

            StatCell.DefineBigStat(0, "assetsnoxraven\\icon_DMG.blp",
            (frame, u) =>
                {
                    string val = u.ui_baseDMG + UIUtils.BonusStringFromValue(u.ui_bonusDMG);
                    BlzFrameSetText(frame.textFrame, val);
                },
                (u) =>
                {
                    string val = u.ui_baseDMG + UIUtils.BonusStringFromValue(u.ui_bonusDMG);
                    return "Damage: " + u.DMG + " = (" + val + ")\n\nYour basic attacks deal this much damage.";
                }
            );
            StatCell.DefineBigStat(1, "assetsnoxraven\\icon_ARM.blp",
            (frame, u) =>
                {
                    string val = u.ui_baseARM + UIUtils.BonusStringFromValue(u.ui_bonusARM);
                    BlzFrameSetText(frame.textFrame, val);
                },
                (u) =>
                {
                    string val = u.ui_baseARM + UIUtils.BonusStringFromValue(u.ui_bonusARM);
                    return "Armor: " + u.ARM + " = (" + val + ")\n" +
                    "Physical Damage Reduction: " + R2SW(100 - UnitUtils.GetDamageReductionFromArmor(u.ARM) * 100, 2, 2) + "%\n\nPhysical damage that you take will be reduced the higher the armour of your character is.";
                }
            );
            StatCell.DefineBigStat(2, "assetsnoxraven\\icon_AS.blp",
            (frame, u) =>
                {
                    string val;
                    if (u.lookupBaseAttackCooldown <= 0.001f)
                        val = "0.00";
                    else
                    {
                        float aps = (1 + u.lookupAttackSpeed) / u.lookupBaseAttackCooldown;
                        val = R2SW(aps, 2, 2);//.ToString("0.00");
                    }
                    BlzFrameSetText(frame.textFrame, val);
                },
                (u) =>
                {
                    string val;
                    if (u.lookupBaseAttackCooldown <= 0.001f)
                        val = "0.00";
                    else
                    {
                        float aps = (1 + u.lookupAttackSpeed) / u.lookupBaseAttackCooldown;
                        val = R2SW(aps, 2, 2);//.ToString("0.00");
                    }
                    return "Attacks Per Second: " + val + "\n" +
                    "Base Attack Cooldown: " + BlzGetUnitAttackCooldown(u, 0) + "sec" + "\n" +
                    "Attack Speed: " + u.lookupAttackSpeed + "%\n\n" +
                    "This is how often your character can attack.";
                }
            );

            StatCell.DefineBigStat(3, "assetsnoxraven\\icon_AP.blp",
            (frame, u) =>
                {
                    string val = u.ui_baseAP + UIUtils.BonusStringFromValue(u.ui_bonusAP);
                    BlzFrameSetText(frame.textFrame, val);
                },
                (u) =>
                {
                    string val = u.ui_baseAP + UIUtils.BonusStringFromValue(u.ui_bonusAP);
                    return "Ability Power: " + u.AP + " = (" + val + ")" + "\n\n" +
                    "Ability Power impoves abilities in various way.";
                }
            );
            StatCell.DefineBigStat(4, "assetsnoxraven\\icon_MR.blp",
            (frame, u) =>
                {
                    string val = u.ui_baseMR + UIUtils.BonusStringFromValue(u.ui_bonusMR);
                    BlzFrameSetText(frame.textFrame, val);
                },
                (u) =>
                {
                    string val = u.ui_baseMR + UIUtils.BonusStringFromValue(u.ui_bonusMR);
                    return "Magic Resist: " + u.MR + " = (" + val + ")\n" +
                    "Magical Damage Reduction: " + R2SW(100 - UnitUtils.GetDamageReductionFromArmor(u.MR) * 100, 2, 2) + "%\n\n" +
                    "Magical and spell damage that you take will be reduced the higher the armour of your character is.";
                }
            );
            StatCell.DefineBigStat(5, "assetsnoxraven\\icon_SPD.blp",
            (frame, u) =>
                {
                    string val;
                    int ms = R2I(GetUnitMoveSpeed(u));
                    if (ms > 50)
                        val = ms.ToString();
                    else val = "0";
                    BlzFrameSetText(frame.textFrame, val);
                },
                (u) =>
                {
                    string val;
                    int ms = R2I(GetUnitMoveSpeed(u));
                    if (ms > 50)
                        val = ms.ToString();
                    else val = "0";
                    return "Movement Speed: " + val + " = (" + u.lookupBaseMS + UIUtils.BonusStringFromValue(u.lookupBaseMS * u.lookupBaseMSPercent) + ")\n\n" +
                    "Movement speed is how fast character is moving";
                }
            );

            // run update timer
            // this is real time, at max of 60 FPS
            Master.s_globalTick.Add(() =>
            {
                // problematic, because a lot of things can backfire
                try
                {
                    if (BlzFrameIsVisible(s_unitInfoFrame))
                    {
                        NoxUnit u = Master.GetSelectedUnit();
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
                    Utils.Debug(e.Message);
                }
            });

        }

    }
}