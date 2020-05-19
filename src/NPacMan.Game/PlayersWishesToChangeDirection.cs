namespace NPacMan.Game
{
    internal class PlayersWishesToChangeDirection
    {
        public PlayersWishesToChangeDirection(Direction newDirection)
        {
            NewDirection = newDirection;
        }

        public Direction NewDirection { get; }
    }

}