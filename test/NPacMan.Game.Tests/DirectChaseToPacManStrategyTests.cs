using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class DirectChaseToPacManStrategyTests
    {
        [Fact]
        public void ShouldMoveTowardsPacMan()
        {
            var board = new TestGameBoard()
            {
                Ghosts = { new Ghost("Bob", 10, 0, new DirectChaseToPacManStrategy()) },
                
            };
          
            // var nextPosition = strategy.Move(ghost, pacman);
            //
            // nextPosition.Should().Be((9, 0));
        }
    }
}
