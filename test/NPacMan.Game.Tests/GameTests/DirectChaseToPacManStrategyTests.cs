using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class DirectChaseToPacManStrategyTests
    {

        [Theory]
        [InlineData(0,5,4,5)]
        [InlineData(10,5,6,5)]
        [InlineData(5,0,5,4)]
        [InlineData(5,10,5,6)]
        public void ShouldMoveTowardsPacMan(int pacManX, int pacManY, int expectedGhostPositionX, int expectedGhostPositionY)
        {
            var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, 5, 5, CellLocation.TopLeft, new DirectChaseToPacManStrategy(), new StandingStillGhostStrategy()) },
                PacMan = new PacMan(pacManX, pacManY, Direction.Left, PacManStatus.Alive, 3)
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();

            game.Ghosts[name].Should().BeEquivalentTo(new
            {
                X = expectedGhostPositionX,
                Y = expectedGhostPositionY
            });
        }

        [Fact]
        public void ShouldNotWalkInToWall()
        {
            var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, 5, 5, CellLocation.TopLeft, new DirectChaseToPacManStrategy(), new StandingStillGhostStrategy()) },
                PacMan = new PacMan(3, 5, Direction.Left, PacManStatus.Alive, 3),
                Walls = {(4,5)}
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[name].Should().NotBeEquivalentTo(new 
            {
                X = 4,
                Y = 5
            });

            game.Ghosts[name].Should().NotBeEquivalentTo(new
            {
                X = 5,
                Y = 5
            });
        }


        [Fact]
        public void ShouldNotWalkInToWall2()
        {
            var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, 5, 5, CellLocation.TopLeft, new DirectChaseToPacManStrategy(), new StandingStillGhostStrategy()) },
                PacMan = new PacMan(5, 3, Direction.Left, PacManStatus.Alive, 3),
                Walls = { (5, 4) }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[name].Should().NotBeEquivalentTo(new
            {
                X = 5,
                Y = 4
            });

            game.Ghosts[name].Should().NotBeEquivalentTo(new
            {
                X = 5,
                Y = 5
            });
        }
    }
}
