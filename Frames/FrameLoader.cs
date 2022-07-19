using System;
using System.Collections.Generic;
using static War3Api.Common;

namespace NoxRaven.Frames
{
    // No idea what this is for
    // Not gonna use at first, but it supposed to solve the game loading issue with frames dunno
    public static class FrameLoader
    {
        public static timer s_tim = CreateTimer();
        public static trigger s_trig = CreateTrigger();
        public static List<Action> s_frames = new List<Action>();
        static FrameLoader()
        {
            TriggerRegisterGameEvent(s_trig, EVENT_GAME_LOADED);
            TriggerAddAction(s_trig, OnLoadAction);
        }

        public static void Add(Action func)
        {
            s_frames.Add(func);
        }
        public static void OnLoadAction()
        {
            TimerStart(s_tim, 0, false, OnLoadTimer);
        }
        public static void OnLoadTimer()
        {
            foreach (Action frame in s_frames)
                frame();
        }

    }
}