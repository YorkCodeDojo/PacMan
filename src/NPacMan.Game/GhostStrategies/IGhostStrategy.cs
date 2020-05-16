namespace NPacMan.Game.GhostStrategies
{
    public interface IGhostStrategy
    {
        Direction? GetNextDirection(Ghost ghost, Game game);
    }
}