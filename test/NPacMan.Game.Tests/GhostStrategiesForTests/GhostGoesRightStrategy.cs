namespace NPacMan.Game.Tests
{
    public class GhostGoesRightStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game) => Direction.Right;
    }
}