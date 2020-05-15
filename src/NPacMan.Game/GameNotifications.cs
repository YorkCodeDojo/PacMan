using System;
using System.Collections.Generic;

namespace NPacMan.Game
{
    public enum GameNotification
    {
        EatCoin,
        EatFruit,
        Beginning,
        Dying,
        Respawning,
        Reborn,
        Intermission,
        ExtraPac,
        EatGhost,
        EatPowerPill
    }

    internal class GameNotifications
    {
        private readonly Dictionary<GameNotification, List<Action>> _registrations = new Dictionary<GameNotification, List<Action>>();

        public void Subscribe(GameNotification gameNotification, Action action)
        {
            if (!_registrations.TryGetValue(gameNotification, out var actions))
            {
                actions = new List<Action>();
                _registrations[gameNotification] = actions;
            }
            actions.Add(action);
        }

        public void Publish(GameNotification gameNotification)
        {
            if (_registrations.TryGetValue(gameNotification, out var actions))
            {
                foreach (var action in actions)
                {
                    action();
                }
            }
        }
    }
}
