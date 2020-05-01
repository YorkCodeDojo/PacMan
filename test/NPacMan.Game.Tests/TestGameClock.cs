using System;

namespace NPacMan.Game.Tests
{
    public class TestGameClock : IGameClock
    {
        private Action? _action;

        public void Subscribe(Action action)
        {
            _action = action;
        }

        public void Tick()
        {
            _action?.Invoke();
        }
    }
}