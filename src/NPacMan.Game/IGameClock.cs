using System;

namespace NPacMan.Game
{
    public interface IGameClock
    {
        void Subscribe(Action action);
    }
}