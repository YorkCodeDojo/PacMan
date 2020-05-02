namespace NPacMan.Game
{
    public class MoveHomeGhostStrategy : IGhostStrategy
    {
        private readonly int _homeX;
        private readonly int _homeY;

        public MoveHomeGhostStrategy(int homeX, int homeY)
        {
            _homeX = homeX;
            _homeY = homeY;
        }

        public (int x, int y) Move(Ghost ghost, Game game)
        {
            return (_homeX, _homeY);
        }
    }
}