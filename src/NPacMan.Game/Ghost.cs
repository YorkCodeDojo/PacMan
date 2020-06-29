using System;
using System.Collections.Generic;
using System.Linq;
using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game
{
    public interface IMoveClock
    {
        bool ShouldGhostMove(Ghost ghost);
        bool ShouldPacManMove();
        void UpdateTime(TimeSpan deltaTime);
    }

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
                if ((ghostLastMoved + movingAtFrightenedSpeed) < _internalClock)
                {
                    _ghostsLastMoves[ghost.Name] = ghostLastMoved + movingAtFrightenedSpeed;
                    return true;
                }
            }
            else
            {
                if ((ghostLastMoved + movingAtFullSpeed) < _internalClock)
                {
                    _ghostsLastMoves[ghost.Name] = ghostLastMoved + movingAtFullSpeed;
                    return true;
                }
            }

            return false;
        }

        public bool ShouldPacManMove()
        {
            var movingAtFullSpeed = PercentageToTime(80);

            if (_pacManLastMoved == DateTime.MinValue)
            {
                _pacManLastMoved = _internalClock;
                return true;
            }

            if ((_pacManLastMoved + movingAtFullSpeed) < _internalClock)
            {
                _pacManLastMoved = _pacManLastMoved + movingAtFullSpeed;
                return true;
            }

            return false;
        }

        private TimeSpan PercentageToTime(int percent) => TimeSpan.FromMilliseconds(100 / (percent / 100f));
    }


    public class Ghost
    {
        public string Name { get; }

        public CellLocation Location { get; }
        public Direction Direction { get; }

        public CellLocation Home { get; }
        private IGhostStrategy ScatterStrategy { get; }
        private IGhostStrategy ChaseStrategy { get; }
        private IGhostStrategy FrightenedStrategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public GhostStatus Status { get; }

        public bool Edible => Status == GhostStatus.Edible || Status == GhostStatus.Flash;

        public int NumberOfCoinsRequiredToExitHouse { get; }

        public Ghost(string name,
                     CellLocation location,
                     Direction direction,
                     IGhostStrategy chaseStrategy,
                     IGhostStrategy scatterStrategy,
                     IGhostStrategy frightenedStrategy,
                     int numberOfCoinsRequiredToExitHouse = 0)
        : this(name, location, location, direction, chaseStrategy, scatterStrategy, frightenedStrategy, chaseStrategy, GhostStatus.Alive, numberOfCoinsRequiredToExitHouse)
        {
        }

        private Ghost(string name,
                      CellLocation homeLocation,
                      CellLocation currentLocation,
                      Direction direction,
                      IGhostStrategy chaseStrategy,
                      IGhostStrategy scatterStrategy,
                      IGhostStrategy frightenedStrategy,
                      IGhostStrategy currentStrategy,
                      GhostStatus ghostStatus,
                      int numberOfCoinsRequiredToExitHouse)
        {
            Name = name;
            Home = homeLocation;
            Location = currentLocation;
            Direction = direction;
            ChaseStrategy = chaseStrategy;
            ScatterStrategy = scatterStrategy;
            FrightenedStrategy = frightenedStrategy;
            CurrentStrategy = currentStrategy;
            Status = ghostStatus;
            NumberOfCoinsRequiredToExitHouse = numberOfCoinsRequiredToExitHouse;
        }
        private bool GhostWalkingOutOfGhostHouse(Game game)
        {
            return Status == GhostStatus.Alive && (game.GhostHouse.Contains(Location) || game.Doors.Contains(Location));
        }
        internal Ghost Move(Game game, IReadOnlyGameState gameState)
        {
            if (GhostWalkingOutOfGhostHouse(game))
            {
                if (game.StartingCoins.Count - game.Coins.Count >= NumberOfCoinsRequiredToExitHouse)
                {
                    var outDirection = Direction.Up;
                    var target = game.Doors.First().Above;
                    if(target.X < Location.X)
                    {
                        outDirection = Direction.Left;
                    }
                    else if(target.X > Location.X)
                    {
                        outDirection = Direction.Right;
                    }
                    var newGhostLocation = Location + outDirection;
                    
                    return WithNewLocationAndDirection(newGhostLocation, outDirection);
                }
                else
                {
                    return this;
                }
            }
          
            var nextDirection = CurrentStrategy.GetNextDirection(this, game);
            if (nextDirection is Direction newDirection)
            {
                var newGhostLocation = Location + newDirection;
                if (game.Portals.TryGetValue(newGhostLocation, out var otherEndOfThePortal))
                {
                    newGhostLocation = otherEndOfThePortal + newDirection;
                }
                if(game.GhostHouseMiddle == newGhostLocation) {
                    return WithNewLocationAndDirection(newGhostLocation, newDirection)
                        .WithNewStatusAndStrategy(GhostStatus.Alive, ChaseStrategy);
                }
                return WithNewLocationAndDirection(newGhostLocation, newDirection);
            }
            else
                return this;
        }

        internal Ghost SetToFlash()
        {
            return WithNewStatus(GhostStatus.Flash);
        }

        internal Ghost Chase()
        {
            return WithNewCurrentStrategyAndDirection(ChaseStrategy, Direction.Opposite());
        }

        internal Ghost Scatter()
        {
            return WithNewCurrentStrategyAndDirection(ScatterStrategy, Direction.Opposite());
        }

        internal Ghost SetToHome() => WithNewLocation(Home);

        internal Ghost SendHome() => WithNewStatusAndStrategy(GhostStatus.RunningHome, new GetGhostHomeStrategy());

        internal Ghost SetToEdible()
        {
            return WithNewEdibleAndDirectionAndStrategy(GhostStatus.Edible, Direction.Opposite(), FrightenedStrategy);
        }

        internal Ghost  SetToAlive() => WithNewEdibleAndDirectionAndStrategy(GhostStatus.Alive, Direction, ChaseStrategy);

        internal Ghost SetToScore() => WithNewStatusAndStrategy(GhostStatus.Score, ChaseStrategy);

        private Ghost WithNewStatusAndStrategy(GhostStatus newGhostStatus,  IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, Direction, ChaseStrategy, ScatterStrategy, FrightenedStrategy, newCurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewEdibleAndDirectionAndStrategy(GhostStatus newGhostStatus, Direction newDirection, IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, newDirection, ChaseStrategy, ScatterStrategy, FrightenedStrategy, newCurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewCurrentStrategyAndDirection(IGhostStrategy newCurrentStrategy, Direction newDirection)
            => new Ghost(Name, Home, Location, newDirection, ChaseStrategy, ScatterStrategy, FrightenedStrategy, newCurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocation(CellLocation newLocation)
            => new Ghost(Name, Home, newLocation, Direction, ChaseStrategy, ScatterStrategy, FrightenedStrategy, CurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocationAndDirection(CellLocation newLocation, Direction newDirection)
            => new Ghost(Name, Home, newLocation, newDirection, ChaseStrategy, ScatterStrategy, FrightenedStrategy, CurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewStatus(GhostStatus newGhostStatus)
            => new Ghost(Name, Home, Location, Direction, ChaseStrategy, ScatterStrategy, FrightenedStrategy, CurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);
    }
}
