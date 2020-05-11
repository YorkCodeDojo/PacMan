using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class DirectChaseToPacManStrategyTests
    {

        [Theory]
        [InlineData(0,5,4,5)]
        [InlineData(5,0,5,4)]
        [InlineData(5,10,5,6)]
        public void ShouldMoveTowardsPacMan(int pacManX, int pacManY, int expectedGhostPositionX, int expectedGhostPositionY)
        {
            //pacManX: 10, pacManY: 5, expectedGhostPositionX: 6, expectedGhostPositionY: 5) [FAIL
            var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, new CellLocation(5, 5), Direction.Left, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())) },
                PacMan = new PacMan((pacManX, pacManY), Direction.Left)
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();

            game.Ghosts[name].Should().BeEquivalentTo(new
            {
                Location = new {
                    X = expectedGhostPositionX,
                    Y = expectedGhostPositionY
                }
            });
        }

        [Fact]
        public void ShouldNotWalkInToWall()
        {
            var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, new CellLocation(5, 5), Direction.Left, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())) },
                PacMan = new PacMan((3, 5), Direction.Left),
                Walls = {(4,5)}
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[name].Should().NotBeEquivalentTo(new 
            {
                Location = new {
                    X = 4,
                    Y = 5
                }
            });

            game.Ghosts[name].Should().NotBeEquivalentTo(new
            {
                Location = new {
                    X = 5,
                    Y = 5
                }
            });
        }


        [Fact]
        public void ShouldNotWalkInToWall2()
        {
            var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, new CellLocation(5, 5), Direction.Left, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())) },
                PacMan = new PacMan((5, 3), Direction.Left),
                Walls = { (5, 4) }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[name].Should().NotBeEquivalentTo(new
            {
                Location = new {
                    X = 5,
                    Y = 4
                }
            });

            game.Ghosts[name].Should().NotBeEquivalentTo(new
            {
                Location = new {
                    X = 5,
                    Y = 5
                }
            });
        }

        [Fact]
        public void ShouldTurnAtCorner()
        {
            // X X X X 
            // X ^ ▶ ▶
            // X S X X

             var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, new CellLocation(1, 2), Direction.Up, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())) },
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { (0, 0), (1, 0), (2, 0), (3, 0),
                    (0,1), (0,2), (2,2), (3,2)
                }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();
            gameClock.Tick();
            gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[name].Should().BeEquivalentTo(new
            {
                Location = new {
                    X = 3,
                    Y = 1
                },
                Direction = Direction.Right
            });
        }

        [Fact]
        public void ShouldTurnTowardsPacManAtJunction()
        {
            // X   X X 
            //   ^ ▶ ▶ C
            // X S X X

             var name = "Bob";
            var board = new TestGameSettings()
            {
                Ghosts = { new Ghost(name, new CellLocation(1, 2), Direction.Up, CellLocation.TopLeft, new DirectToStrategy(new DirectToPacManLocation())) },
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { (0, 0), (2, 0), (3, 0),
                   (0,2), (2,2), (3,2)
                }
            };

            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);
            gameClock.Tick();
            gameClock.Tick();
            gameClock.Tick();

            using var _ = new AssertionScope();
            game.Ghosts[name].Should().BeEquivalentTo(new
            {
                Location = new {
                    X = 3,
                    Y = 1
                },
                Direction = Direction.Right
            });
        }


        [Fact]
        public void ShouldBeAbleToTargetWherePacManWillBe()
        {
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { }
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);

            var directToExpectedPacManLocation = new DirectToExpectedPacManLocation();
            var targetLocation = directToExpectedPacManLocation.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 8,
                Y = 1
            });
        }

        [Fact]
        public void ShouldBeAbleInterceptPacMan()
        {
// ghost = 1,2
// pacman (+2) will be at= 6,1
// difference 5,-1
// target vector = (difference * 2) = 10, -2
// ghost + vector = 11, 0

            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Ghosts = { new Ghost("ghost-1", new CellLocation(1, 2), Direction.Up, CellLocation.TopLeft, new NullGhostStrategy()) },                
                Walls = { }
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);

            var inkyTargetCell = new InterceptPacManLocation("ghost-1");
            var targetLocation = inkyTargetCell.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 0
            });
        }

        [Fact]
        public void StayCloseToPacManMovesTowardsHimWhileOutsideOf8Cells()
        {
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((14, 1), Direction.Right),
                Walls = { },
                Ghosts = { new Ghost("ghost-1", new CellLocation(1, 2), Direction.Up, CellLocation.TopLeft, new NullGhostStrategy()) },                
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);

            var staysCloseToPacManLocation = new StaysCloseToPacManLocation("ghost-1");
            var targetLocation = staysCloseToPacManLocation.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 14,
                Y = 1
            });
        }
        
        [Fact]
        public void StayCloseToPacManScattersWhileInsideOf8Cells()
        {
            var board = new TestGameSettings()
            {
                PacMan = new PacMan((4, 1), Direction.Right),
                Walls = { },
                Ghosts = { new Ghost("ghost-1", new CellLocation(1, 2), Direction.Up, new CellLocation(11, 12), new NullGhostStrategy()) },                
            };
            var gameClock = new TestGameClock();
            var game = new Game(gameClock, board);

            var staysCloseToPacManLocation = new StaysCloseToPacManLocation("ghost-1");
            var targetLocation = staysCloseToPacManLocation.GetLocation(game);

            targetLocation.Should().BeEquivalentTo(new
            {
                X = 11,
                Y = 12
            });
        }
    }
}
