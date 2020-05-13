using NPacMan.Game.GhostStrategies;

namespace NPacMan.Game.Tests.GhostStrategiesForTests
{
    public class StandingStillGhostStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game) => null;
    }
}