using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game.Tests.GhostStrategiesForTests
{
    public class GhostGoesRightStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game) => Direction.Right;
    }
}