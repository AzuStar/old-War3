using System;

using static War3Api.Common;

namespace War3.NoxRaven
{
    public sealed class Message
    {
        public const int RECIPIENT_ALL = 0;
        public const int RECIPIENT_ALLIES = 1;
        public const int RECIPIENT_OBSERVERS = 2;
        public const int RECIPIENT_PRIVATE = 3;

        private readonly timer _timer;
        private readonly player _player;
        private readonly int _recipient;
        private readonly string _message;

        public Message(player player, int recipient, string message)
        {
            _timer = CreateTimer();
            _player = player;
            _recipient = recipient;
            _message = message;
        }

        public void Display(float timeout)
        {
            TimerStart(_timer, timeout, false, OnDisplay);
        }

        private void OnDisplay()
        {
            BlzDisplayChatMessage(_player, _recipient, _message);
            DestroyTimer(_timer);
        }
    }
}