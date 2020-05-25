using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace NPacMan.Game.Tests
{
    public class RandomDirectionPickerTests
    {
        [Fact]
        public void ShouldPickSinglePseudoRandom()
        {
            var picker = new RandomDirectionPicker();

            var direction1 = picker.Pick(new []{Direction.Up});
            var direction2 = picker.Pick(new []{Direction.Down});
            var direction3 = picker.Pick(new []{Direction.Left});
            var direction4 = picker.Pick(new []{Direction.Right});

            using var _ = new AssertionScope();
            direction1.Should().Be(Direction.Up);
            direction2.Should().Be(Direction.Down);
            direction3.Should().Be(Direction.Left);
            direction4.Should().Be(Direction.Right);
        }
        
        [Fact]
        public void ShouldPickMultiplePseudoRandom()
        {
            var picker = new RandomDirectionPicker();

            var direction1 = picker.Pick(new []{Direction.Up, Direction.Down, Direction.Right});
            var direction2 = picker.Pick(new []{Direction.Down, Direction.Right, Direction.Left});
            var direction3 = picker.Pick(new []{Direction.Left, Direction.Right});
            var direction4 = picker.Pick(new []{Direction.Right, Direction.Left, Direction.Up});

            using var _ = new AssertionScope();
            direction1.Should().Be(Direction.Up);
            direction2.Should().Be(Direction.Right);
            direction3.Should().Be(Direction.Left);
            direction4.Should().Be(Direction.Right);
        }
    }
}