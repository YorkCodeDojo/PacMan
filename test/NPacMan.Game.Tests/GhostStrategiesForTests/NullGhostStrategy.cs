using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game.Tests.GhostStrategiesForTests
{
    public class NullGhostStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game)
            => null;
    }
}
