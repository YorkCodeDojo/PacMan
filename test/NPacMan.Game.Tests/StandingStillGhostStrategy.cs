namespace NPacMan.Game.Tests
{
    public class StandingStillGhostStrategy : IGhostStrategy
    {
        public (int x, int y) Move(Ghost ghost, Game game) => (ghost.X, ghost.Y);
    }

 
}