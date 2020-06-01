using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.GhostStrategiesForTests;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostBuilder
    {
        private int _numberOfCoinsRequiredToExitHouse = 0;
        private IGhostStrategy _chaseStrategy = new StandingStillGhostStrategy();
        private CellLocation _scatterTarget = CellLocation.TopLeft;
        private Direction _direction = Direction.Left;
        private CellLocation _location = new CellLocation(0, 0);

        private GhostBuilder()
        {
            
        }

        public static GhostBuilder New()
        {
            return new GhostBuilder();
        }

        public GhostBuilder WithNumberOfCoinsRequiredToExitHouse(int numberOfCoinsRequiredToExitHouse)
        {
            _numberOfCoinsRequiredToExitHouse = numberOfCoinsRequiredToExitHouse;

            return this;
        }

        public GhostBuilder WithChaseStrategy(IGhostStrategy ghostStrategy)
        {
            _chaseStrategy = ghostStrategy;

            return this;
        }


        public GhostBuilder WithScatterTarget(CellLocation scatterTarget)
        {
            _scatterTarget = scatterTarget;

            return this;
        }


        public GhostBuilder WithDirection(Direction direction)
        {
            _direction = direction;

            return this;
        }

        public GhostBuilder WithLocation(CellLocation location)
        {
            _location = location;

            return this;
        }

        private static int ghostNumber = 0;
        public Ghost Create()
        {
            var next = Interlocked.Increment(ref ghostNumber);
            var name = $"Ghost{next}";

            return new Ghost(name, _location, _direction, _scatterTarget, _chaseStrategy,
                _numberOfCoinsRequiredToExitHouse);
        }

        public IList<Ghost> CreateMany(int count)
            => Enumerable.Range(0, count).Select(x => Create()).ToList();

        public GhostBuilder WithChaseStrategyRight()
            => WithChaseStrategy(new GhostGoesRightStrategy());
    }
}