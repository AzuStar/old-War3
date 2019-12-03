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
        public static bool BadLoad = false;
        public static int ErrorCount = 0;
        /// <summary>
        /// Run when all static data is initialized.<para></para>
        /// Types taht need to be initialized before running: UnitEntity Custom Classes, Players
        /// </summary>
        public static void RunAfterExtensionsReady()
        {
            int abilid = FourCC("AD00");
            for (int i = 0; i < UnitEntity.Abilities_BonusDamage.Length; i++)
            {
                UnitEntity.Abilities_BonusDamage[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("CR00");
            for (int i = 0; i < UnitEntity.Abilities_Corruption.Length; i++)
            {
                UnitEntity.Abilities_Corruption[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("ST00");
            for (int i = 0; i < HeroEntity.Abilities_Strength.Length; i++)
            {
                HeroEntity.Abilities_Strength[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("AG00");
            for (int i = 0; i < HeroEntity.Abilities_Agility.Length; i++)
            {
                HeroEntity.Abilities_Agility[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            abilid = FourCC("IN00");
            for (int i = 0; i < HeroEntity.Abilities_Intelligence.Length; i++)
            {
                HeroEntity.Abilities_Intelligence[i] = abilid;
                if ((i + 1) % 10 == 0) abilid += 246;
                abilid++;
            }
            UnitEntity.InitUnitLogic();
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
            Utils.DisplayMessageToEveryone("|cffacf2f0Map loaded correctly!|r", 2f);
        }
    }
}
