using System.Linq;
using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game
{
    public class Ghost
    {
        public string Name { get; }

        public CellLocation Location { get; }
        public Direction Direction { get; }

        public CellLocation Home { get; }
        private IGhostStrategy ScatterStrategy { get; }
        private IGhostStrategy ChaseStrategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public GhostStatus Status { get; }

        public bool Edible => Status == GhostStatus.Edible || Status == GhostStatus.Flash;

        public int NumberOfCoinsRequiredToExitHouse { get; }

        public Ghost(string name, CellLocation location, Direction direction, IGhostStrategy chaseStrategy, IGhostStrategy scatterStrategy, int numberOfCoinsRequiredToExitHouse = 0)
        : this(name, location, location, direction, chaseStrategy, scatterStrategy, chaseStrategy, GhostStatus.Alive, numberOfCoinsRequiredToExitHouse)
        {
        }

        private Ghost(string name, CellLocation homeLocation, CellLocation currentLocation, Direction direction, IGhostStrategy chaseStrategy, IGhostStrategy scatterStrategy, IGhostStrategy currentStrategy, GhostStatus ghostStatus, int numberOfCoinsRequiredToExitHouse)
        {
            Name = name;
            Home = homeLocation;
            Location = currentLocation;
            Direction = direction;
            ChaseStrategy = chaseStrategy;
            ScatterStrategy = scatterStrategy;
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

        internal Ghost SetToEdible(IDirectionPicker directionPicker)
        {
            var strategy = new RandomStrategy(directionPicker);
            return WithNewEdibleAndDirectionAndStrategy(GhostStatus.Edible, Direction.Opposite(), strategy);
        }

        internal Ghost  SetToAlive() => WithNewEdibleAndDirectionAndStrategy(GhostStatus.Alive, Direction, ChaseStrategy);

        internal Ghost SetToScore() => WithNewStatusAndStrategy(GhostStatus.Score, ChaseStrategy);

        private Ghost WithNewStatusAndStrategy(GhostStatus newGhostStatus,  IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, Direction, ChaseStrategy, ScatterStrategy, newCurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewEdibleAndDirectionAndStrategy(GhostStatus newGhostStatus, Direction newDirection, IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, newDirection, ChaseStrategy, ScatterStrategy, newCurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewCurrentStrategyAndDirection(IGhostStrategy newCurrentStrategy, Direction newDirection)
            => new Ghost(Name, Home, Location, newDirection, ChaseStrategy, ScatterStrategy, newCurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocation(CellLocation newLocation)
            => new Ghost(Name, Home, newLocation, Direction, ChaseStrategy, ScatterStrategy, CurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocationAndDirection(CellLocation newLocation, Direction newDirection)
            => new Ghost(Name, Home, newLocation, newDirection, ChaseStrategy, ScatterStrategy, CurrentStrategy, Status, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewStatus(GhostStatus newGhostStatus)
            => new Ghost(Name, Home, Location, Direction, ChaseStrategy, ScatterStrategy, CurrentStrategy, newGhostStatus, NumberOfCoinsRequiredToExitHouse);
    }
}
