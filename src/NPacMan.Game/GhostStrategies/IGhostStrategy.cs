namespace NPacMan.Game
{
    public interface IGhostStrategy
    {
        Direction? GetNextDirection(Ghost ghost, Game game);
    }
}