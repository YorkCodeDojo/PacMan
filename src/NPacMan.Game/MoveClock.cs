using System;
using System.Collections.Generic;

namespace NPacMan.Game
{
    public class MoveClock : IMoveClock
    {
        private IDictionary<string, DateTime> _ghostsLastMoves
            = new Dictionary<string, DateTime>();
        private DateTime _pacManLastMoved;
        private DateTime _internalClock;

        public void UpdateTime(TimeSpan deltaTime)
        {
            _internalClock = _internalClock.Add(deltaTime);
        }

        public bool ShouldGhostMove(Ghost ghost)
        {
            if (!_ghostsLastMoves.TryGetValue(ghost.Name, out var ghostLastMoved))
            {
                ghostLastMoved = _internalClock;
                _ghostsLastMoves[ghost.Name] = ghostLastMoved;
                return true;
            }

            var movingAtFullSpeed = PercentageToTime(75);
            var movingAtFrightenedSpeed = PercentageToTime(50);

            if (ghost.Edible)
            {
                if ((ghostLastMoved + movingAtFrightenedSpeed) <= _internalClock)
                {
                    _ghostsLastMoves[ghost.Name] = ghostLastMoved + movingAtFrightenedSpeed;
                    return true;
                }
            }
            else
            {
                if ((ghostLastMoved + movingAtFullSpeed) <= _internalClock)
                {
                    _ghostsLastMoves[ghost.Name] = ghostLastMoved + movingAtFullSpeed;
                    return true;
                }
            }

            return false;
        }
        
        public bool ShouldPacManMove(int gameLevel, bool isFrightened)
        {
            int GetPercentageNormal()
            {
                if (gameLevel == 1) return 80;
                if (gameLevel >= 5 && gameLevel <= 20) return 100;
                return 90;
            }
            int GetPercentageFrightened()
            {
                if (gameLevel == 1) return 90;
                if (gameLevel >= 5 && gameLevel <= 20) return 100;
                return 95;
            }
            var time = PercentageToTime(isFrightened ? GetPercentageFrightened() : GetPercentageNormal());

            if (_pacManLastMoved == DateTime.MinValue)
            {
                _pacManLastMoved = _internalClock;
                return true;
            }

            if ((_pacManLastMoved + time) <= _internalClock)
            {
                _pacManLastMoved = _pacManLastMoved + time;
                return true;
            }

            return false;
        }

        private TimeSpan PercentageToTime(int percent) => TimeSpan.FromMilliseconds((int)(100 / (percent / 100f)));
    }
}