using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;
using NoxRaven.Frames;

namespace NoxRaven
{
    public static class Master
    {
        public const float TICKS_PER_SECOND = 128; // internal wc3:reforged logic tick is about 128
        public const float TICK_DELTA = 1 / TICKS_PER_SECOND;
        public const int VERSION = 1;
        public static bool s_badLoad = false;
        public static int s_errCount = 0;
        private static unit _selectedUnit = null;
        private static group _selectedGroup = CreateGroup();
        public static List<Action<float>> s_globalTick = new List<Action<float>>();

        private static timer s_sanityTimer = CreateTimer();
        private static bool s_isSane = false;
        public static bool s_numbersOn = true;
        public static float s_corpseFadeTime = 10;

        /// <summary>
        /// Retrieves first selected unit, for which unit info would have been displayed normally.
        /// </summary>
        public static NUnit GetSelectedUnit()
        {
            player p = null;
            p = GetLocalPlayer();
            // do no simplify!
            //_selectedUnit and _selectedGroup are needed to prevent desync
            GroupEnumUnitsSelected(_selectedGroup, p, null);
            _selectedUnit = FirstOfGroup(_selectedGroup);
            GroupClear(_selectedGroup);
            GetUnitName(_selectedUnit);
            if (_selectedUnit == null)
            {
                return null;
            }
            return _selectedUnit;
        }
        /// <summary>
        /// Run when all static data is initialized.<br />
        /// Types taht need to be initialized before running: UnitEntity Custom Classes, Players, Items
        /// </summary>
        public static void RunAfterExtensionsReady()
        {
            BlzLoadTOCFile("noxraven\\NUnitFrames.toc");
            timer ticker = CreateTimer();
            TimerStart(ticker, TICK_DELTA, true, () =>
            {
                foreach (var action in s_globalTick)
                    action(TimerGetElapsed(ticker));
            });
            NUnit.InitUnitLogic();
            NItem._InitItemLogic();
            Tooltip.InitCustomTooltip();
            StatCell.InitCustomInfoPanel();
            HotReload.ReloaderInit();
            TimerStart(s_sanityTimer, 5, false, () =>
            {
                if (!s_isSane)
                    Utils.DisplayMessageToEveryone("Failed sanity check (or it has not been called).", 999f);
                DestroyTimer(s_sanityTimer);
            });
            TimerStart(CreateTimer(), 1800, true, GCRoutine); // funny but really helps
        }
        /// <summary>
        /// Put at the very end of Main.
        /// </summary>
        public static void RunAtEndOfMain()
        {
            if (s_badLoad)
            {
                Utils.DisplayMessageToEveryone("Something went wrong OwO\n" +
                    "Errors encountered: " + I2S(s_errCount), 999f);
                return;
            }
            Utils.DisplayMessageToEveryone("|cffacf2f0Correct load!|r", 2f);// Do not remove this, I promise this will hurt
            s_isSane = true;
        }
        /// <summary>
        /// TODO: Dynamically adjust this
        /// </summary>
        private static void GCRoutine()
        {
            GC.Collect();
        }
    }
}
