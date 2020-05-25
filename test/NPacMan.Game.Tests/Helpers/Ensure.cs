using NPacMan.Game.Tests.Helpers;

namespace NPacMan.Game.Tests.GameTests
{
    public static class Ensure
    {
        public static EnsureThatGhost WeExpectThat(Ghost ghost) => new EnsureThatGhost(ghost);
        public static EnsureThatPacMan WeExpectThat(PacMan pacMan) => new EnsureThatPacMan(pacMan);
    }
}