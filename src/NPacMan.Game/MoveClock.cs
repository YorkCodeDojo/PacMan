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
                int GetPoints(){
                    switch (gameLevel)
                    {
                        case 1:
                            return 20;
                        case 2:
                            return 30;
                        case 3:
                        case 4:
                        case 5:
                            return 40;
                        case 6:
                        case 7:
                        case 8:
                            return 50;
                        case 9:
                        case 10:
                        case 11:
                            return 60;
                        case 12:
                        case 13:
                        case 14:
                            return 80;
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                            return 100;
                        default:
                            return 120;
                    }
                }
                var points = GetPoints();

                if (coinsRemaining <= points / 2)
                {
                    if (gameLevel == 1) return 85;
                    if (gameLevel < 5) return 95;
                    return 105;
                }

                if (coinsRemaining < points)
                {
                    if (gameLevel == 1) return 80;
                    if (gameLevel < 5) return 90;
                    return 100;
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

        private TimeSpan PercentageToTime(int percent) => TimeSpan.FromMilliseconds((int)(107 / (percent / 100f)));
    }
}