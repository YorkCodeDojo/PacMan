using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game.Tests.GhostStrategiesForTests
{
    public class GhostGoesInDirectionStrategy : IGhostStrategy
    {
        public Direction? NextDirection { get; set; }

        public GhostGoesInDirectionStrategy(Direction? direction)
        {
            NextDirection = direction;
        }
        public Direction? GetNextDirection(Ghost ghost, Game game) => NextDirection;
    }
}