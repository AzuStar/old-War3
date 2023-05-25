using System;
using System.Collections.Generic;
using System.Text;
using static War3Api.Common;
using static War3Api.Blizzard;

namespace NoxRaven.IO
{
    public class File
    {
        public static int ReadCount = 0;
        public static readonly int AbilID = FourCC("SaLo");
        /// <summary>
        /// Hard limit appears to be 255 (sendsync) 220 (write).
        /// </summary>
        public const int ChunkLen = 150; // you can fit upto 15,000 long stirng
        public readonly string Path;
        public readonly NPlayer TargetPlayer;

        

        public File(string path, NPlayer forWhatPlayer)
        {
            TargetPlayer = forWhatPlayer;
            Path = path;
        }
        public void WriteRaw(string payload)
        {
            int len = payload.Length;
            if (GetLocalPlayer() == TargetPlayer.wc3agent)
            {
                PreloadGenClear();
                PreloadGenStart();
                Preload("\")\n");
                for (int i = 0; i < len;)
                {
                    string chunk = SubString(payload, i, i + ChunkLen);
                    Preload("\")\n" + chunk + "\n");
                    i += ChunkLen;
                }
                PreloadGenEnd(Path);
            }
        }
        public void Write(string payload)
        {
            int len = payload.Length;
            int level = 0;
            payload = Base64.Base64Encode(payload);
            if (GetLocalPlayer() == TargetPlayer.wc3agent)
            {
                PreloadGenClear();
                PreloadGenStart();
                for (int i = 0; i < len;) // yet another bug
                {
                    string chunk = SubString(payload, i, i + ChunkLen);
                    Preload("\")\ncall BlzSetAbilityExtendedTooltip(" + I2S(AbilID) + ",\"" + chunk + "\"," + I2S(level) + ")\n//");
                    level++;
                    i += ChunkLen;
                }
                Preload("\")\nendfunction\nfunction emptyfun takes nothing returns nothing\n//");
                PreloadGenEnd(Path);
            }
        }
        /// <summary>
        /// This method locks execution AND must be called within trigger or it will break data (or crash).
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            ReadCount++;
            PromisedString ret = new PromisedString();
            trigger SyncThread = CreateTrigger();
            for (int i = 0; i < 100; i++)
                foreach (NPlayer pl in NPlayer.players.Values)
                    BlzTriggerRegisterPlayerSyncEvent(SyncThread, pl.wc3agent, I2S(i) + "sync" + I2S(ReadCount), false);
            foreach (NPlayer pl in NPlayer.players.Values)
                BlzTriggerRegisterPlayerSyncEvent(SyncThread, pl.wc3agent, "async" + I2S(ReadCount), false); // util
            TriggerAddAction(SyncThread, () =>
            {
                string prefix = BlzGetTriggerSyncPrefix().Split("sync")[0];
                if (prefix == "a")
                    ret.Size = S2I(BlzGetTriggerSyncData());
                else
                    ret.Chunks.Add(S2I(prefix), BlzGetTriggerSyncData());
                ret.CheckComplete();

            });

            if (GetLocalPlayer() == TargetPlayer.wc3agent)
            {
                Preloader(Path);
                int i = 0;
                for (; i < 100; i++)
                {
                    string chunk = BlzGetAbilityExtendedTooltip(AbilID, i);
                    BlzSetAbilityExtendedTooltip(AbilID, "0", i);
                    if (chunk == "0")
                        break;
                    BlzSendSyncData(I2S(i) + "sync" + I2S(ReadCount), chunk);
                }
                BlzSendSyncData("async" + I2S(ReadCount), I2S(i));
            }

            while (!ret.HaveString)
                TriggerSleepAction(0.1f);
            DestroyTrigger(SyncThread);
            return ret.BuildString();
        }

    }
}
