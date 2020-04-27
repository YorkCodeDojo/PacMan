namespace NPacMan.Game
{
    public interface IGhostStrategy
    {
        (int x, int y) Move(Ghost ghost, Game game);
    }
}