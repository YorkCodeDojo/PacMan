using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game.Tests.GhostStrategiesForTests
{
    public class GhostGoesInDirectionStrategy : IGhostStrategy
    {
        private readonly Direction? _direction;

        public GhostGoesInDirectionStrategy(Direction? direction)
        {
            _direction = direction;
        }
        public Direction? GetNextDirection(Ghost ghost, Game game) => _direction;
    }

    public class ManuallyControlGhostStrategy : IGhostStrategy
    {
        public Direction? NextDirection { get; set; }

        public ManuallyControlGhostStrategy(Direction? direction)
        {
            NextDirection = direction;
        }
        public Direction? GetNextDirection(Ghost ghost, Game game) => NextDirection;
    }
}