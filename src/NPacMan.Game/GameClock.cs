using System;
using System.Threading;

namespace NPacMan.Game
{
    public class GameClock : IGameClock, IDisposable
    {
        private Action<DateTime>? _action;
        private readonly Timer _timer;

        public GameClock()
        {
            _timer = new Timer((state) => _action?.Invoke(DateTime.Now), null, 0, 200);
        }

        public void Subscribe(Action<DateTime> action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}