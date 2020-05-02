namespace NPacMan.Game
{
    public class MoveHomeGhostStrategy : IGhostStrategy
    {
        public int X { get; }
        public int Y { get; }

        public MoveHomeGhostStrategy(int homeX, int homeY)
        {
            X = homeX;
            Y = homeY;
        }

        public (int x, int y) Move(Ghost ghost, Game game)
        {
            return (X, Y);
        }
    }
}