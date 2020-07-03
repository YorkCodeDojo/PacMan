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

        public bool ShouldGhostMove(int gameLevel, int coinsRemaining, string ghostName, GhostStatus ghostStatus)
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

            int GetPercentageCruiseElroy()
            {
                var points = gameLevel == 1 ? 20 :30;

                if(coinsRemaining <= points / 2){
                    return gameLevel == 1 ? 85 : 95;
                }

                if(coinsRemaining < points){
                    return gameLevel == 1 ? 80 : 90;
                }
                
                return GetPercentageAlive();
            }

            var speed = (ghostStatus, ghostName) switch
            {
                (GhostStatus.RunningHome, _) => PercentageToTime(160),
                (GhostStatus.Edible, _) => PercentageToTime(GetPercentageFrightened()),
                (GhostStatus.Flash, _) => PercentageToTime(GetPercentageFrightened()),
                (_, GhostNames.Blinky) => PercentageToTime(GetPercentageCruiseElroy()),
                _ => PercentageToTime(GetPercentageAlive())
            };

            if ((ghostLastMoved + speed) <= _internalClock)
            {
                _ghostsLastMoves[ghostName] = ghostLastMoved + speed;
                return true;
            }

            return false;
        }
        
        public bool ShouldPacManMove(int gameLevel, bool isFrightened)
        {
            if (_pacManLastMoved == DateTime.MinValue)
            {
                _pacManLastMoved = _internalClock;
                return true;
            }

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