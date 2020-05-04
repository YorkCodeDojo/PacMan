namespace NPacMan.Game
{
    public class MoveHomeGhostStrategy : IGhostStrategy
    {
        public Direction? GetNextDirection(Ghost ghost, Game game)
        {
            return Direction.Down;
        }
    }
}