using System;
using System.Collections.Generic;
using System.Text;
using NoxRaven.Units;
using static War3Api.Common;
using static War3Api.Blizzard;
namespace NoxRaven
{
    public static class Master
    {
        public static float FPS = 64;
        public static float FPS_TIME = 1 / FPS;
        public static int Version = 1;
        public static bool BadLoad = false;
        public static int ErrorCount = 0;
        /// <summary>
        /// Run when all static data is initialized.<para></para>
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
            for (int i = 0; i < NoxHero.Abilities_Strength.Length; i++)
            {
                NoxHero.Abilities_Strength[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("AG00");
            for (int i = 0; i < NoxHero.Abilities_Agility.Length; i++)
            {
                NoxHero.Abilities_Agility[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("IN00");
            for (int i = 0; i < NoxHero.Abilities_Intelligence.Length; i++)
            {
                NoxHero.Abilities_Intelligence[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            NoxUnit.InitUnitLogic();
            NoxItem.InitItemLogic();
            TimerStart(CreateTimer(), 1800, true, GCRoutine);
        }
        /// <summary>
        /// Put at the very end of Main.
        /// </summary>
        public static void RunAtEndOfMain()
        {
            if (BadLoad)
            {
                Utils.DisplayMessageToEveryone("Something went wrong OwO\n" +
                    "Errors encountered: "+I2S(ErrorCount), 999f);
                return;
            }
            Utils.DisplayMessageToEveryone("|cffacf2f0Map loaded correctly!|r", 2f);// Do not remove this, I promise this will hurt
            Utils.DisplayMessageToEveryone("|cffacf2f0NoxRaven Version: |r|cffff0000"+Version+"|r", 2f);//you can remove this :)
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
