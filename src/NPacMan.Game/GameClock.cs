using System;
using System.Threading;

namespace NPacMan.Game
{
    public class GameClock : IGameClock
    {
        private Action? _action;
        private Timer _timer;

        public GameClock()
        {
            _timer = new System.Threading.Timer((state) => _action?.Invoke(), null, 0, 200);
        }
        public void Subscribe(Action action)
        {
            _action = action;
        }
    }
}