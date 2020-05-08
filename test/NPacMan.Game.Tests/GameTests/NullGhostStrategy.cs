namespace NPacMan.Game.Tests.GameTests
{
    public class NullGhostStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game)
            => null;
    }
}