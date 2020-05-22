using System;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    public interface IGameClock
    {
        void Subscribe(Func<DateTime, Task> action);
        void Pause();
        void Resume();
    }
}