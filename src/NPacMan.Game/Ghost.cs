using System.Linq;
using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game
{
    public enum GhostStatus
    {
        Alive,
        Edible,
        Score,
        RunningHome
    }
    public class Ghost
    {
        public string Name { get; }

        public CellLocation Location { get; }
        public CellLocation ScatterTarget { get; }

        public Direction Direction { get; }

        public CellLocation Home { get; }
        private IGhostStrategy ChaseStrategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public GhostStatus Status { get; }

        public bool Edible => Status == GhostStatus.Edible;

        public int NumberOfCoinsRequiredToExitHouse { get; }

        public Ghost(string name, CellLocation location, Direction direction, CellLocation scatterTarget, IGhostStrategy chaseStrategy, int numberOfCoinsRequiredToExitHouse = 0)
        : this(name, location, location, direction, scatterTarget, chaseStrategy, chaseStrategy, GhostStatus.Alive, numberOfCoinsRequiredToExitHouse)
        {
        }

        private Ghost(string name, CellLocation homeLocation, CellLocation currentLocation, Direction direction, CellLocation scatterTarget, IGhostStrategy chaseStrategy, IGhostStrategy currentStrategy, GhostStatus ghostStatus, int numberOfCoinsRequiredToExitHouse)
        {
            Name = name;
            Home = homeLocation;
            Location = currentLocation;
            Direction = direction;
            ChaseStrategy = chaseStrategy;
            CurrentStrategy = currentStrategy;
            ScatterTarget = scatterTarget;
            Status = ghostStatus;
            NumberOfCoinsRequiredToExitHouse = numberOfCoinsRequiredToExitHouse;
        }
        private bool GhostWalkingOutOfGhostHouse(Game game)
        {
            return Status == GhostStatus.Alive && (game.GhostHouse.Contains(Location) || game.Doors.Contains(Location));
        }
        internal Ghost Move(Game game, IReadOnlyGameState gameState)
        {
            if (Edible && gameState.TickCounter % 2 == 1) return this;

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
                if(game.GhostHouse.Contains(newGhostLocation)){
                    return WithNewLocationAndDirection(newGhostLocation, newDirection)
                        .WithNewStatusAndStrategy(GhostStatus.Alive, ChaseStrategy);
                }
                return WithNewLocationAndDirection(newGhostLocation, newDirection);
            }
            else
                return this;
        }

        internal Ghost Chase()
        {
            return WithNewCurrentStrategyAndDirection(ChaseStrategy, Direction.Opposite());
        }

        internal Ghost Scatter()
        {
            var strategy = new DirectToStrategy(new DirectToGhostScatterTarget(this));
            return WithNewCurrentStrategyAndDirection(strategy, Direction.Opposite());
        }

        internal Ghost SetToHome() => WithNewLocation(Home);

        internal Ghost SendHome() => WithNewStatusAndStrategy(GhostStatus.RunningHome, new DirectToStrategy(new DirectToGhostHouseLocation(), allowGhostsToWalkThroughDoors: true));

        internal Ghost SetToEdible(IDirectionPicker directionPicker)
        {
            var strategy = new RandomStrategy(directionPicker);
            return WithNewEdibleAndDirectionAndStrategy(GhostStatus.Edible, Direction.Opposite(), strategy);
        }

        internal Ghost  SetToAlive() => WithNewEdibleAndDirectionAndStrategy(GhostStatus.Alive, Direction, ChaseStrategy);

        internal Ghost SetToScore() => WithNewStatusAndStrategy(GhostStatus.Score, ChaseStrategy);

        private Ghost WithNewStatusAndStrategy(GhostStatus newGhostStatus,  IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, Direction, ScatterTarget, ChaseStrategy, newCurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewEdibleAndDirectionAndStrategy(GhostStatus newGhostStatus, Direction newDirection, IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, newDirection, ScatterTarget, ChaseStrategy, newCurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewCurrentStrategyAndDirection(IGhostStrategy newCurrentStrategy, Direction newDirection)
            => new Ghost(Name, Home, Location, newDirection, ScatterTarget, ChaseStrategy, newCurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocation(CellLocation newLocation)
            => new Ghost(Name, Home, newLocation, Direction, ScatterTarget, ChaseStrategy, CurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocationAndDirection(CellLocation newLocation, Direction newDirection)
            => new Ghost(Name, Home, newLocation, newDirection, ScatterTarget, ChaseStrategy, CurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

    }
}
