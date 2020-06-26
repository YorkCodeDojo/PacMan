using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class InMemoryHighScoreStorageTests
    {
        [Fact]
        public void ShouldInitallyHaveTheHighScoreSetToZero()
        {
            var storage = new InMemoryHighScoreStorage();

            storage.GetHighScore()
                .Should().Be(0);
        }

        [Fact]
        public void ShouldReturnLastStoredHighScore()
        {
            var storage = new InMemoryHighScoreStorage();

            storage.SetHighScore(1);
            storage.SetHighScore(2);
            storage.SetHighScore(3);

            storage.GetHighScore()
                .Should().Be(3);
        }
    }
}