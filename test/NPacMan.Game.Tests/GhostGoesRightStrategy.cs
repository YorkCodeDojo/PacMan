namespace NPacMan.Game.Tests
{
    public class GhostGoesRightStrategy : IGhostStrategy
    {
        public (int x, int y) Move(Ghost ghost, Game game) => (ghost.X+1, ghost.Y);
    }
}