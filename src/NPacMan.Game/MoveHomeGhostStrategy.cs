namespace NPacMan.Game
{
    public class MoveHomeGhostStrategy : IGhostStrategy
    {
        public (int x, int y) Move(Ghost ghost, Game game)
        {
            return (ghost.HomeLocationX, ghost.HomeLocationY);
        }
    }
}