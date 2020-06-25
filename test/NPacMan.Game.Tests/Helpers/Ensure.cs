using System.Collections.Generic;

namespace NPacMan.Game.Tests.Helpers
{
    public static class Ensure
    {
        public static EnsureThatGhost WeExpectThat(Ghost ghost) => new EnsureThatGhost(ghost);
        public static EnsureThatGhosts WeExpectThat(IEnumerable<Ghost> ghosts) => new EnsureThatGhosts(ghosts);
        public static EnsureThatPacMan WeExpectThat(Game game, PacMan pacMan) => new EnsureThatPacMan(game, pacMan);
    }
}