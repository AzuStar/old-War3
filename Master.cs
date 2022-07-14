using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;
using NoxRaven.Units;

namespace NoxRaven
{
    public static class Master
    {
        public const float FPS = 64;
        public const float FPS_TIME = 1 / FPS;
        public const int s_version = 1;
        public static bool s_badLoad = false;
        public static int s_errCount = 0;

        private static timer s_sanityTimer = CreateTimer();
        private static bool s_isSane = false;
        public static bool s_numbersOn = true;
        /// <summary>
        /// Run when all static data is initialized.<br />
        /// Types taht need to be initialized before running: UnitEntity Custom Classes, Players, Items
        /// </summary>
        public static void RunAfterExtensionsReady()
        {
            int abilid = FourCC("AD00");
            for (int i = 0; i < NoxUnit.Abilities_BonusDamage.Length; i++)
            {
                NoxUnit.Abilities_BonusDamage[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("AR00");
            for (int i = 0; i < NoxUnit.Abilities_BonusArmor.Length; i++)
            {
                NoxUnit.Abilities_BonusArmor[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("CR00");
            for (int i = 0; i < NoxUnit.Abilities_Corruption.Length; i++)
            {
                NoxUnit.Abilities_Corruption[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("ST00");
            for (int i = 0; i < NoxHero.ABIL_STR.Length; i++)
            {
                NoxHero.ABIL_STR[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("AG00");
            for (int i = 0; i < NoxHero.ABIL_AGI.Length; i++)
            {
                NoxHero.ABIL_AGI[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("IN00");
            for (int i = 0; i < NoxHero.ABIL_INT.Length; i++)
            {
                NoxHero.ABIL_INT[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            NoxUnit.InitUnitLogic();
            NoxItem.InitItemLogic();
            TimerStart(s_sanityTimer, 5, false, () =>
            {
                if (!s_isSane)
                    Utils.DisplayMessageToEveryone("Failed sanity check (or it has not been called).", 999f);
                DestroyTimer(s_sanityTimer);
            });
            TimerStart(CreateTimer(), 1800, true, GCRoutine);
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
