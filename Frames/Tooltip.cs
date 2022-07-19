using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using NoxRaven;
using NoxRaven.Units;
using static War3Api.Common;

namespace NoxRavent.Frames
{
    public class Tooltip : IDisposable
    {
        public framehandle tooltipFrame;
        public Func<NoxUnit, string> updateCallback;
        public Tooltip(framehandle buttonParent, Func<NoxUnit, string> update)
        {
            updateCallback = update;
            tooltipFrame = BlzCreateFrameByType("SIMPLEFRAME", "", buttonParent, "", 0);
            BlzFrameSetTooltip(buttonParent, tooltipFrame);
            BlzFrameSetVisible(tooltipFrame, false);
            s_tooltips.Add(this);
        }
        public static framehandle s_parent;
        public static framehandle s_tooltipBox;
        public static framehandle s_tooltipTextTitle;
        public static framehandle s_tooltipHorizonatSeparator;
        public static framehandle s_tooltipTextDescription;
        public static HashSet<Tooltip> s_tooltips = new HashSet<Tooltip>();

        public static void InitCustomTooltip()
        {
            s_parent = BlzGetFrameByName("ConsoleUIBackdrop", 0);
            s_tooltipBox = BlzCreateFrame("NU_TT_Command_TextBox", s_parent, 0, 0);
            s_tooltipTextTitle = BlzCreateFrame("NU_TTE_Command_TextTitle", s_tooltipBox, 0, 0);
            s_tooltipTextDescription = BlzCreateFrame("NU_TTE_Command_TextDescription", s_tooltipBox, 0, 0);

            BlzFrameSetAbsPoint(s_tooltipTextTitle, FRAMEPOINT_BOTTOMRIGHT, 0.79f, 0.18f);
            BlzFrameSetSize(s_tooltipTextTitle, 0.275f, 0);
            BlzFrameSetPoint(s_tooltipBox, FRAMEPOINT_TOPLEFT, s_tooltipTextTitle, FRAMEPOINT_TOPLEFT, -0.004f, 0.004f);
            BlzFrameSetPoint(s_tooltipBox, FRAMEPOINT_BOTTOMRIGHT, s_tooltipTextTitle, FRAMEPOINT_BOTTOMRIGHT, 0.004f, -0.004f);
            BlzFrameSetVisible(s_tooltipBox, false);

            // TriggerAddAction(s_tooltipTrig, () =>
            // {
            // // request currently used unit of the clicking player
            // var unit = UnitInfoGetUnit(GetTriggerPlayer());
            // // get the clicked buttonIndex
            // var buttonIndex = s_statFrames[BlzGetTriggerFrame()].Index;
            // // do something base on the unit and buttonIndex
            // var text = s_tooltipListener[buttonIndex](s_statFrames[BlzGetTriggerFrame()]);
            // BlzFrameSetText(s_tooltipText, text);
            // BlzFrameSetVisible(s_tooltipBox, true);
            // });

            Master.s_globalTick.Add(() =>
            {
                try
                {
                    bool flag = false;
                    foreach (var tooltip in s_tooltips)
                    {
                        if (BlzFrameIsVisible(tooltip.tooltipFrame))
                        {
                            string text;
                            NoxUnit u = Master.GetSelectedUnit();

                            if (u != null && tooltip.updateCallback != null)
                            {
                                text = tooltip.updateCallback(u);
                            }
                            else text = "Tooltip is missing!";
                            BlzFrameSetText(s_tooltipTextTitle, text);
                            flag = true;
                            break;
                        }
                    }
                    BlzFrameSetVisible(s_tooltipBox, flag);
                }
                catch (Exception e)
                {
                    Utils.Debug(e.Message);
                }
            });
        }


        public void Dispose()
        {
            BlzDestroyFrame(this.tooltipFrame);
            s_tooltips.Remove(this);
        }
    }
}