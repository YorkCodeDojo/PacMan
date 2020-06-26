using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class InMemoryHighScoreStorageTests
    {
        [Fact]
        public async Task ShouldInitallyHaveTheHighScoreSetToZeroAsync()
        {
            var storage = new InMemoryHighScoreStorage();

            (await storage.GetHighScore())
                .Should().Be(0);
        }

        [Fact]
        public async Task ShouldReturnLastStoredHighScore()
        {
            var storage = new InMemoryHighScoreStorage();

            await storage.SetHighScore(1);
            await storage.SetHighScore(2);
            await storage.SetHighScore(3);

            (await storage.GetHighScore())
                .Should().Be(3);
        }
    }
}