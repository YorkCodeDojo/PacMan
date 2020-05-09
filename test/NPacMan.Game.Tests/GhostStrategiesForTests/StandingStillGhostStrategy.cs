namespace NPacMan.Game.Tests
{
    public class StandingStillGhostStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game) => null;
    }
}