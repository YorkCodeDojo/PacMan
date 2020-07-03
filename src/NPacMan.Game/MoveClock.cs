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

        public bool ShouldGhostMove(int gameLevel, string ghostName, GhostStatus ghostStatus)
        {
            if (!_ghostsLastMoves.TryGetValue(ghostName, out var ghostLastMoved))
            {
                ghostLastMoved = _internalClock;
                _ghostsLastMoves[ghostName] = ghostLastMoved;
                return true;
            }

            int GetPercentageFrightened()
            {
                if (gameLevel == 1) return 50;
                if (gameLevel < 5) return 55;
                return 60;
            }

            int GetPercentageAlive()
            {
                if (gameLevel == 1) return 75;
                if (gameLevel < 5) return 85;
                return 95;
            }

            if (ghostStatus == GhostStatus.RunningHome)
            {
                var movingAtRunningSpeed = PercentageToTime(160);

                if ((ghostLastMoved + movingAtRunningSpeed) <= _internalClock)
                {
                    _ghostsLastMoves[ghostName] = ghostLastMoved + movingAtRunningSpeed;
                    return true;
                }
            } 
            else if (ghostStatus == GhostStatus.Edible || ghostStatus == GhostStatus.Flash)
            {
                var movingAtFrightenedSpeed = PercentageToTime(GetPercentageFrightened());

                if ((ghostLastMoved + movingAtFrightenedSpeed) <= _internalClock)
                {
                    _ghostsLastMoves[ghostName] = ghostLastMoved + movingAtFrightenedSpeed;
                    return true;
                }
            }
            else
            {
                var movingAtFullSpeed = PercentageToTime(GetPercentageAlive());

                if ((ghostLastMoved + movingAtFullSpeed) <= _internalClock)
                {
                    _ghostsLastMoves[ghostName] = ghostLastMoved + movingAtFullSpeed;
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