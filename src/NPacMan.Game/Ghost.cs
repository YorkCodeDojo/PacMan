using System.Linq;
using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game
{

    public class Ghost
    {
        public string Name { get; }

        public CellLocation Location { get; }
        public CellLocation ScatterTarget { get; }

        public Direction Direction { get; }

        public CellLocation Home { get; }
        private IGhostStrategy ChaseStrategy { get; }
        private IGhostStrategy CurrentStrategy { get; }

        public bool Edible { get; }
        public int NumberOfCoinsRequiredToExitHouse { get; }

        public Ghost(string name, CellLocation location, Direction direction, CellLocation scatterTarget, IGhostStrategy chaseStrategy, int numberOfCoinsRequiredToExitHouse = 0)
        : this(name, location, location, direction, scatterTarget, chaseStrategy, chaseStrategy, false, numberOfCoinsRequiredToExitHouse)
        {
        }

        private Ghost(string name, CellLocation homeLocation, CellLocation currentLocation, Direction direction, CellLocation scatterTarget, IGhostStrategy chaseStrategy, IGhostStrategy currentStrategy, bool edible, int numberOfCoinsRequiredToExitHouse)
        {
            Name = name;
            Home = homeLocation;
            Location = currentLocation;
            Direction = direction;
            ChaseStrategy = chaseStrategy;
            CurrentStrategy = currentStrategy;
            ScatterTarget = scatterTarget;
            Edible = edible;
            NumberOfCoinsRequiredToExitHouse = numberOfCoinsRequiredToExitHouse;
        }

        internal Ghost Move(Game game, IReadOnlyGameState gameState)
        {
            if (Edible && gameState.TickCounter % 2 == 1) return this;

            if (game.GhostHouse.Contains(Location) || game.Doors.Contains(Location))
            {
                if (game.StartingCoins.Count - game.Coins.Count >= NumberOfCoinsRequiredToExitHouse)
                {
                    var outDirection = Direction.Up;
                    var target = game.Doors.First().Above;
                    if (target.X < Location.X)
                    {
                        outDirection = Direction.Left;
                    }
                    else if (target.X > Location.X)
                    {
                        outDirection = Direction.Right;
                    }
                    var newGhostLocation = Location + outDirection;

                    PacManEventSource.Log.GhostMoved(Name, gameState.TickCounter, Location.X, Location.Y, newGhostLocation.X, newGhostLocation.Y);

                    return WithNewLocationAndDirection(newGhostLocation, outDirection);
                }
                else
                {
                    PacManEventSource.Log.GhostStuckInHouse(Name, gameState.TickCounter, Location.X, Location.Y, NumberOfCoinsRequiredToExitHouse);

                    return this;
                }
            }

            var nextDirection = CurrentStrategy.GetNextDirection(this, game);
            if (nextDirection is Direction newDirection)
            {
                var newGhostLocation = Location + newDirection;
                PacManEventSource.Log.GhostMoved(Name, gameState.TickCounter, Location.X, Location.Y, newGhostLocation.X, newGhostLocation.Y);

                if (game.Portals.TryGetValue(newGhostLocation, out var otherEndOfThePortal))
                {
                    newGhostLocation = otherEndOfThePortal + newDirection;
                    PacManEventSource.Log.GhostMoved(Name, gameState.TickCounter, Location.X, Location.Y, newGhostLocation.X, newGhostLocation.Y);
                }

                return WithNewLocationAndDirection(newGhostLocation, newDirection);
            }
            else
                return this;
        }

        internal Ghost Chase(IReadOnlyGameState gameState)
        {
            PacManEventSource.Log.GhostChangedState(Name, gameState.TickCounter, Location.X, Location.Y, "Chase", Edible);

            return WithNewCurrentStrategyAndDirection(ChaseStrategy, Direction.Opposite());
        }

        internal Ghost Scatter(IReadOnlyGameState gameState)
        {
            PacManEventSource.Log.GhostChangedState(Name, gameState.TickCounter, Location.X, Location.Y, "Scatter", Edible);

            var strategy = new DirectToStrategy(new DirectToGhostScatterTarget(this));
            return WithNewCurrentStrategyAndDirection(strategy, Direction.Opposite());
        }

        internal Ghost SetToHome(IReadOnlyGameState gameState)
        {
            PacManEventSource.Log.GhostMoved(Name, gameState.TickCounter, Location.X, Location.Y, Home.X, Home.Y);

            return WithNewLocation(Home);
        }

        internal Ghost SetToEdible(IReadOnlyGameState gameState, IDirectionPicker directionPicker)
        {
            PacManEventSource.Log.GhostChangedState(Name, gameState.TickCounter, Location.X, Location.Y, "Random", edible: true);

            var strategy = new RandomStrategy(directionPicker);
            return WithNewEdibleAndDirectionAndStrategy(true, Direction.Opposite(), strategy);
        }

        internal Ghost SetToNotEdible(IReadOnlyGameState gameState)
        {
            PacManEventSource.Log.GhostChangedState(Name, gameState.TickCounter, Location.X, Location.Y, "Chase", edible: false);

            return WithNewEdibleAndDirectionAndStrategy(false, Direction, ChaseStrategy);
        }

        private Ghost WithNewEdibleAndDirectionAndStrategy(bool isEdible, Direction newDirection, IGhostStrategy newCurrentStrategy)
            => new Ghost(Name, Home, Location, newDirection, ScatterTarget, ChaseStrategy, newCurrentStrategy, isEdible, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewCurrentStrategyAndDirection(IGhostStrategy newCurrentStrategy, Direction newDirection)
            => new Ghost(Name, Home, Location, newDirection, ScatterTarget, ChaseStrategy, newCurrentStrategy, Edible, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocation(CellLocation newLocation)
            => new Ghost(Name, Home, newLocation, Direction, ScatterTarget, ChaseStrategy, CurrentStrategy, Edible, NumberOfCoinsRequiredToExitHouse);

        private Ghost WithNewLocationAndDirection(CellLocation newLocation, Direction newDirection)
            => new Ghost(Name, Home, newLocation, newDirection, ScatterTarget, ChaseStrategy, CurrentStrategy, Edible, NumberOfCoinsRequiredToExitHouse);

    }
}
