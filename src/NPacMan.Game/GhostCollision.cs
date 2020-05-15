namespace NPacMan.Game
{
    internal class GhostCollision
    {
        public GhostCollision(Ghost ghost)
        {
            Ghost = ghost;
        }
        
        public Ghost Ghost { get; }
    }
}