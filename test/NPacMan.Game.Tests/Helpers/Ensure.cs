namespace NPacMan.Game.Tests.Helpers
{
    public static class Ensure
    {
        public static EnsureThatGhost WeExpectThat(Ghost ghost) => new EnsureThatGhost(ghost);
        public static EnsureThatPacMan WeExpectThat(PacMan pacMan) => new EnsureThatPacMan(pacMan);
    }
}