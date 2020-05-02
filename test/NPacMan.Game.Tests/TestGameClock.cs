using System;

namespace NPacMan.Game.Tests
{
    public class TestGameClock : IGameClock
    {
        private Action<DateTime>? _action;

        public void Subscribe(Action<DateTime> action)
        {
            _action = action;
        }

        public void Tick()
        {
            Tick(DateTime.UtcNow);
        }

        public void Tick(DateTime now)
        {
            _action?.Invoke(now);
        }
    }
}