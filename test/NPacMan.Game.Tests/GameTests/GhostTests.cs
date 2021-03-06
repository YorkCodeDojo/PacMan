﻿using FluentAssertions;
using FluentAssertions.Execution;
using NPacMan.Game.GhostStrategies;
using NPacMan.Game.Tests.GhostStrategiesForTests;
using NPacMan.Game.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NPacMan.Game.Tests.GameTests
{
    public class GhostTests
    {
        private readonly TestGameSettings _gameSettings;

        public GhostTests()
        {
            _gameSettings = new TestGameSettings
            {
                PowerPills = { new CellLocation(50, 50) }
            };
        }

        [Fact]
        public async Task GhostMovesInDirectionOfStrategy()
        {
            var ghost = GhostBuilder.New()
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.Move();
            gameHarness.Game.Ghosts[ghost.Name].Should().BeEquivalentTo(new
            {
                Location = new
                {
                    X = 1,
                    Y = 0
                }
            });
        }

        [Fact]
        public async Task GhostShouldNotMoveWhenPacManIsDying()
        {
            var homeLocation = _gameSettings.PacMan.Location.Left.Left;
            var ghost = GhostBuilder.New()
                .WithLocation(homeLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()))
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .Create();

            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.Move();
            await gameHarness.GetEatenByGhost(ghost);

            var ghostLocation = ghost.Location;

            await gameHarness.NOP();

            await gameHarness.WaitToFinishDying();
            await gameHarness.WaitToRespawn();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost.Name]
                .Should().BeEquivalentTo(new
                {
                    Location = ghostLocation
                });
        }

        [Fact]
        public async Task GhostShouldBeHiddenWhenPacManIsReSpawning()
        {
            var homeLocation = _gameSettings.PacMan.Location.Left.Left;
            var ghost = GhostBuilder.New()
                .WithLocation(homeLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategy(new DirectToStrategy(new DirectToPacManLocation()))
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .Create();

            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.Move();
            await gameHarness.GetEatenByGhost(ghost);
            await gameHarness.WaitToFinishDying();

            gameHarness.EnsureGameStatus(GameStatus.Respawning);

            gameHarness.Game.Ghosts.Should().BeEmpty();
        }

        [Fact]
        public async Task GhostShouldBeBackAtHomeAfterPacManDiesAndComesBackToLife()
        {
            _gameSettings.PacMan = new PacMan((3, 2), Direction.Left);
            var homeLocation = new CellLocation(1, 2);
            var ghost = GhostBuilder.New()
                .WithLocation(homeLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();

            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.GetEatenByGhost(ghost);
            await gameHarness.WaitToFinishDying();
            await gameHarness.WaitToRespawn();

            gameHarness.Game.Ghosts[ghost.Name]
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = homeLocation.X,
                        Y = homeLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldScatterToStartWith()
        {
            _gameSettings.PacMan = new PacMan((10, 10), Direction.Left);
            var startingLocation = new CellLocation(3, 1);
            var scatterLocation = new CellLocation(1, 1);

            var ghost = GhostBuilder.New()
                .WithLocation(startingLocation)
                .WithScatterTarget(scatterLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.Move();
            await gameHarness.Move();

            gameHarness.Game.Ghosts[ghost.Name]
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = scatterLocation.X,
                        Y = scatterLocation.Y
                    }
                });
        }

        [Fact]
        public async Task GhostShouldChaseAfter7Seconds()
        {
            var startingLocation = _gameSettings.PacMan.Location.Right;

            var ghost = GhostBuilder.New()
                .WithLocation(startingLocation)
                .WithScatterStrategyUp()
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();
            await gameHarness.ChangeDirection(Direction.Left);

            // Ghost starts by moving Up in scatter mode.
            await gameHarness.Move();
            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation.Above);

            // Ghost then moves right in Chase mode.
            await gameHarness.WaitForScatterToComplete();
            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation.Above.Right);
            await gameHarness.Move();

            gameHarness.Game.Ghosts[ghost.Name]
                .Should().BeEquivalentTo(new
                {
                    Location = startingLocation.Above.Right.Right
                });
        }

        [Fact]
        public async Task GhostShouldScatter7SecondsAfterChase()
        {
            _gameSettings.PacMan = new PacMan((29, 10), Direction.Left);
            var startingLocation = new CellLocation(30, 1);
            var scatterLocation = new CellLocation(1, 1);

            var ghost = GhostBuilder.New()
                .WithLocation(startingLocation)
                .WithScatterTarget(scatterLocation)
                .WithDirection(Direction.Right)
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation);

            await gameHarness.Move();
            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation.Left);

            await gameHarness.WaitForScatterToComplete();
            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation.Left.Right);

            await gameHarness.Move();
            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation.Left.Right.Right);

            await gameHarness.WaitForChaseToComplete();
            gameHarness.WeExpectThatGhost(ghost).IsAt(startingLocation.Left.Right.Right.Left);

            await gameHarness.Move();

            gameHarness.Game.Ghosts[ghost.Name]
                .Should().BeEquivalentTo(new
                {
                    Location = new
                    {
                        X = 29,
                        Y = 1
                    }
                });

        }

        [Fact]
        public async Task GhostsDoNotStartOffAsEdible()
        {
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Right)
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.Move();

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }

        public static IEnumerable<object[]> GetAllGhostsShouldReturnToNonEdibleAfterGivenAmountOfSecondsTestData()
        {
            object[] CreateTestData(int level, int seconds)
            {
                return new object[]{level, seconds};
            }

            yield return CreateTestData(1, 6);
            yield return CreateTestData(2, 5);
            yield return CreateTestData(3, 4);
            yield return CreateTestData(4, 3);
            yield return CreateTestData(5, 2);
            yield return CreateTestData(6, 5);
            yield return CreateTestData(7, 2);
            yield return CreateTestData(8, 2);
            yield return CreateTestData(9, 1);
            yield return CreateTestData(10, 5);
            yield return CreateTestData(11, 2);
            yield return CreateTestData(12, 1);
            yield return CreateTestData(13, 1);
            yield return CreateTestData(14, 3);
            yield return CreateTestData(15, 1);
            yield return CreateTestData(16, 1);
            yield return CreateTestData(17, 0);
            yield return CreateTestData(18, 1);

            for(var i = 19; i <= 25; i++)
            {
                yield return CreateTestData(i, 0);
            }
        }

        [Theory]
        [MemberData(nameof(GetAllGhostsShouldReturnToNonEdibleAfterGivenAmountOfSecondsTestData))]
        public async Task AllGhostsShouldReturnToNonEdibleAfterGivenAmountOfSeconds(int level, int seconds)
        {
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Clear();
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right.Right.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            // Complete previous levels
            
            for ( var currentLevel = 1; currentLevel < level; currentLevel ++)
            {
                await gameHarness.ChangeDirection(Direction.Right);

                await gameHarness.EatPill();

                await gameHarness.Move();

                await gameHarness.EatPill();

                await gameHarness.WaitForEndOfLevelFlashingToComplete();
            }

            // Actual level to test

            await gameHarness.ChangeDirection(Direction.Right);

            await gameHarness.EatPill();

            if (!gameHarness.Game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await gameHarness.WaitFor(TimeSpan.FromSeconds(seconds).Subtract(TimeSpan.FromMilliseconds(100)));

            if (!gameHarness.Game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await gameHarness.WaitFor(TimeSpan.FromMilliseconds(200));

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }

        [Fact]
        public async Task GhostShouldReturnToNonEdibleAfterGivenAmountSecondsWhenEatingSecondPowerPills()
        {
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right.Right.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);

            await gameHarness.EatPill();
            await gameHarness.Move();
            await gameHarness.EatPill();

            if (!gameHarness.Game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await gameHarness.WaitFor(TimeSpan.FromSeconds(GameHarness.LevelOneFrightenedTimeInSeconds).Subtract(TimeSpan.FromMilliseconds(100)));

            if (!gameHarness.Game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await gameHarness.WaitFor(TimeSpan.FromMilliseconds(200));

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false
            });
        }

        [Fact]
        public async Task AllGhostsShouldRemainInLocationAfterPowerPillIsEaten()
        {
            var ghostStart = _gameSettings.PacMan.Location.Below.Right;
            var ghosts = GhostBuilder.New()
                .WithLocation(ghostStart)
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);

            await gameHarness.EatPill();

            if (!gameHarness.Game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Location = ghostStart
            });
        }

        [Fact]
        public async Task AllGhostsShouldShouldStayInSameLocationWhenTransitioningToNoneEdible()
        {
            var ghostStart = _gameSettings.PacMan.Location.Below.Right;
            var ghosts = GhostBuilder.New()
                .WithLocation(ghostStart)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .CreateMany(3);

            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);

            await gameHarness.EatPill();

            if (!gameHarness.Game.Ghosts.Values.All(g => g.Edible))
                throw new Exception("All ghosts are meant to be edible.");

            await gameHarness.WaitForFrightenedTimeToComplete();

            if (!gameHarness.Game.Ghosts.Values.All(g => !g.Edible))
                throw new Exception("All ghosts are meant to be nonedible.");

            using var _ = new AssertionScope();

            foreach (var ghost in gameHarness.Game.Ghosts.Values)
            {
                ghost.Should().NotBeEquivalentTo(new
                {
                    Location = ghostStart
                });
            }
        }

        [Fact]
        public async Task TheEdibleGhostBecomesNonEdibleAfterBeingEaten()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghostStart2 = _gameSettings.PacMan.Location.Below.Below.Below;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(ghostStart2)
                .WithScatterStrategyRight()
                .WithChaseStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart1.Right);

            await gameHarness.EatGhost(ghost1);
            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left.Left);

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Edible = false
            });

            gameHarness.Game.Ghosts[ghost2.Name].Should().BeEquivalentTo(new
            {
                Edible = true
            });
        }

        [Fact]
        public async Task TheEdibleGhostMovesAtHalfSpeedWhileFrightened()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatPill();

            gameHarness.Label("Ghost are Frightened");
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1.Right.Right.Right
            });
        }

        [Fact]
        public async Task GhostMovesRandomlyWhileFrightened()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.FarAway();
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .WithDirection(Direction.Right)
                .WithFrightenedStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);
            await gameHarness.EatPill();

            gameHarness.Label("Ghost are Frightened");
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1.Right.Right
            });
        }


        [Fact]
        public async Task GhostIsTeleportedWhenWalkingIntoAPortal()
        {
            var portalEntrance = new CellLocation(10, 1);
            var portalExit = new CellLocation(1, 5);
            var ghost1 = GhostBuilder.New()
                .WithLocation(portalEntrance.Left)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();

            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Portals.Add(portalEntrance, portalExit);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.Move();

            gameHarness.Game.Ghosts.Values.First()
                .Should()
                .BeEquivalentTo(new
                {
                    Location = portalExit.Right
                });
        }

        [Fact]
        public async Task GhostsShouldStandStillInGhostHouse()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.FarAway();
            _gameSettings.GhostHouse.Add(ghostStart1);

            var ghosts = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithNumberOfCoinsRequiredToExitHouse(int.MaxValue)
                .CreateMany(2);
            _gameSettings.Ghosts.AddRange(ghosts);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.Move();
            await gameHarness.Move();

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Location = ghostStart1
            });
        }

        [Fact]
        public async Task GhostsShouldLeaveGhostHouseWhenPillCountHasBeenReached()
        {
            // X
            // -
            // H G

            var ghostStart1 = new CellLocation(1, 2);
            _gameSettings.PacMan = new PacMan(ghostStart1.FarAway(), Direction.Left);
            _gameSettings.GhostHouse.AddRange(new[] { ghostStart1, ghostStart1.Left });
            _gameSettings.Doors.Add(ghostStart1.Left.Above);
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithNumberOfCoinsRequiredToExitHouse(1)
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithNumberOfCoinsRequiredToExitHouse(10)
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.FarAway());
            _gameSettings.Coins.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatCoin();

            // Still in House, under door
            await gameHarness.Move();

            // On ghost door
            await gameHarness.Move();

            // Out of house
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1.Left.Above.Above
            });
            gameHarness.Game.Ghosts[ghost2.Name].Should().BeEquivalentTo(new
            {
                Location = ghostStart1
            });
        }

        [Fact]
        public async Task GhostsShouldBeNotEdibleAfterPacManComesBackToLife()
        {
            var ghost1 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Above.Above)
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .WithFrightenedStrategyDown()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);

            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .WithFrightenedStrategyDown()
                .Create();
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.CreateBoard("  G             ",
                                      "                ",
                                      "P * #           ",
                                      "  -             ",
                                      "  H             ");


            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.EatPill();

            gameHarness.AssertBoard("  G             ",
                                    "                ",
                                    "  P #           ",
                                    "  -             ",
                                    "  H             ");


            gameHarness.WeExpectThatGhost(ghost1).IsEdible();
            gameHarness.WeExpectThatGhost(ghost2).IsEdible();

            await gameHarness.ChangeDirection(Direction.Up);
            await gameHarness.Move();


            gameHarness.AssertBoard("  G             ",
                                    "  P             ",
                                    "    #           ",
                                    "  -             ",
                                    "  H             ");

            await gameHarness.EatGhost(ghost1);

            gameHarness.WeExpectThatGhost(ghost1).IsNotEdible();
            gameHarness.WeExpectThatGhost(ghost2).IsEdible();

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.ChangeDirection(Direction.Down);

            // Move to Ghost House
            await gameHarness.Move();
            await gameHarness.Move();

            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.GetEatenByGhost(ghost1);

            gameHarness.EnsureGameStatus(GameStatus.Dying);

            await gameHarness.WaitToFinishDying();

            gameHarness.EnsureGameStatus(GameStatus.Respawning);

            await gameHarness.WaitToRespawn();

            gameHarness.EnsureGameStatus(GameStatus.Alive);

        }

        [Fact]
        public async Task EatenGhostNotificationShouldBeFiredAfterEatingAGhost()
        {
            var ghostStart = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghost1 = GhostBuilder.New()
                                     .WithLocation(ghostStart)
                                     .WithChaseStrategyRight()
                                     .WithScatterStrategyRight()
                                     .Create();

            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart.Right);

            await gameHarness.AssertSingleNotificationFires(GameNotification.EatGhost, async () =>
            {
                await gameHarness.EatGhost(ghost1);
                gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left.Left);
            });
        }

        [Fact]
        public async Task GhostsShouldBeInScatterModeAfterPacManComesBackToLife()
        {
            //   G         
            //
            // > .       
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Above.Above)
                .WithChaseStrategyLeft()
                .WithFrightenedStrategyRight()
                .WithScatterStrategyDown()
                .Create();
            _gameSettings.Ghosts.Add(ghost);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.Move();

            await gameHarness.ChangeDirection(Direction.Up);
            await gameHarness.GetEatenByGhost(ghost);

            gameHarness.EnsureGameStatus(GameStatus.Dying);

            await gameHarness.WaitToFinishDying();

            gameHarness.EnsureGameStatus(GameStatus.Respawning);

            await gameHarness.WaitToRespawn();

            gameHarness.EnsureGameStatus(GameStatus.Alive);

            await gameHarness.Move();

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Edible = false,
                Location = _gameSettings.PacMan.Location.Right.Above
            });
        }

        [Fact]
        public async Task TheGamePausesAfterGhostIsEaten()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Left.Left.Left;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Left);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Left);

            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart1.Right);

            await gameHarness.EatGhost(ghost1);
            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Left.Left);

            var pacManLocation = gameHarness.Game.PacMan.Location;
            var ghostLocations = gameHarness.Game.Ghosts.Values.Select(x => new
            {
                x.Name,
                x.Location,
                Status = x.Name == ghost1.Name ? GhostStatus.Score : GhostStatus.Edible
            }).ToDictionary(x => x.Name);

            await gameHarness.NOP();
            await gameHarness.NOP();
            await gameHarness.NOP();

            using var _ = new AssertionScope();
            gameHarness.Game.Should().BeEquivalentTo(new
            {
                PacMan = new
                {
                    Location = pacManLocation
                },
                Ghosts = ghostLocations
            });
        }

        [Fact]
        public async Task TheGameResumesAfterBeingPausedForOneSecondAfterGhostIsEaten()
        {
            //      P
            //   .  *
            // G .  .

            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below.Left.Left;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .WithFrightenedStrategyRight()
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);


            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart1.Right);

            await gameHarness.Move();

            await gameHarness.EatGhost(ghost1);
            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below.Below.Below);

            var pacManLocation = gameHarness.Game.PacMan.Location;
            var ghostLocations = gameHarness.Game.Ghosts.Values.Select(x => new
            {
                x.Name,
                x.Location
            }).ToDictionary(x => x.Name);

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.PacMan.Should().NotBeEquivalentTo(new
            {
                Location = pacManLocation
            });
            gameHarness.Game.Ghosts.Should().NotBeEmpty();
            gameHarness.Game.Ghosts.Should().NotBeEquivalentTo(ghostLocations);
        }

        [Fact]
        public async Task GhostShouldBeRunningHomeAfterThePauseAfterBeingTheEaten()
        {
            //      P
            //   .  *
            // G .  .

            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below.Left.Left;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .WithFrightenedStrategyRight()
                .Create();
            var ghost2 = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();
            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(ghost2);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);


            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart1.Right);

            await gameHarness.Move();

            await gameHarness.EatGhost(ghost1);
            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below.Below.Below);

            var pacManLocation = gameHarness.Game.PacMan.Location;
            var ghostLocations = gameHarness.Game.Ghosts.Values.Select(x => new
            {
                x.Name,
                x.Location
            }).ToDictionary(x => x.Name);

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Status.Should().Be(GhostStatus.RunningHome);
            gameHarness.Game.Ghosts[ghost2.Name].Status.Should().Be(GhostStatus.Edible);
        }

        [Fact]
        public async Task GhostShouldBeInTheHouseAfterBeingEatingAndWaitingForElapsedTime()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below.Left;
            var topLeftWall = _gameSettings.PacMan.Location.Right.Right.Right.Below;
            var ghostHouse = topLeftWall.Below.Right;

            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .WithFrightenedStrategy(new StandingStillGhostStrategy())
                .Create();

            _gameSettings.Ghosts.Add(ghost1);

            _gameSettings.CreateBoard("  P             ",
                                      "  . *   # - - # ",
                                      "G . .   # H H # ",
                                      "        # # # # ");

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.Move();

            gameHarness.AssertBoard("    P           ",
                                    "  . *   # - - # ",
                                    "  G .   # H H # ",
                                    "        # # # # ");

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.AssertBoard("                ",
                                    "  . P   # - - # ",
                                    "    G   # H H # ",
                                    "        # # # # ");

            await gameHarness.EatGhost(ghost1);

            gameHarness.AssertBoard("                ",
                                    "  .     # - - # ",
                                    "    P   # H H # ",
                                    "        # # # # ");

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();

            gameHarness.AssertBoard("                 ",
                                    "  .      # G - # ",
                                    "         # H H # ",
                                    "         # # # # ",
                                    "                 ",
                                    "                 ",
                                    "                 ",
                                    "                 ",
                                    "     P           ");

            gameHarness.AssertBoard("                 ",  // NOTE: This is the same example assertion but without P for PacMan
                                    "  .      # G - # ",
                                    "         # H H # ",
                                    "         # # # # ");

            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Status.Should().Be(GhostStatus.Alive);
            gameHarness.Game.Ghosts[ghost1.Name].Location.Should().Be(ghostHouse);
        }


        [Fact]
        public async Task GhostShouldBeInMiddleOfHouseAfterBeingEatingAndWaitingForElapsedTime()
        {
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below.Left.Left;
            var ghost1 = GhostBuilder.New()
                                     .WithLocation(ghostStart1)
                                     .WithChaseStrategyRight()
                                     .WithScatterStrategyRight()
                                     .WithFrightenedStrategyRight()
                                     .Create();

            _gameSettings.Ghosts.Add(ghost1);

            _gameSettings.CreateBoard("    P              ",
                                      "  . *   # # - - # #",
                                      "G .     # H H H H #",
                                      "        # H H H H #",
                                      "        # H H H H #",
                                      "        # # # # # #");

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.AssertBoard("                   ",
                                    "  . P   # # - - # #",
                                    "  G     # H H H H #",
                                    "        # H H H H #",
                                    "        # H H H H #",
                                    "        # # # # # #");

            await gameHarness.Move();

            await gameHarness.EatGhost(ghost1);

            gameHarness.AssertBoard("                   ",
                                    "  .     # # - - # #",
                                    "        # H H H H #",
                                    "   P    # H H H H #",
                                    "        # H H H H #",
                                    "        # # # # # #");

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();
            await gameHarness.Move();

            var ghostHouse = _gameSettings.PacMan.Location.Right.Right.Below;

            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostHouse.Right.Right);
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Status.Should().Be(GhostStatus.Alive);

            var possibleGhostLocations = new[] {
                ghostHouse.Right.Right.Below.Below,
                ghostHouse.Right.Right.Below.Below.Right
            };

            gameHarness.Game.Ghosts.Values.Select(x => x.Location)
                .Should().BeSubsetOf(possibleGhostLocations);
        }

        [Fact]
        public async Task GhostShouldStillGetToTheHouseAfterTransitions()
        {
            //     P     
            //   . *   
            // G . . . . . .
            //             H
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below.Left.Left;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .Create();

            var ghostHouse = ghostStart1.Right.Right.Right.Right.Right.Right.Below;
            _gameSettings.GhostHouse.Clear();
            _gameSettings.GhostHouse.Add(ghostHouse);

            _gameSettings.Ghosts.Add(ghost1);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart1.Right);

            await gameHarness.Move();
            await gameHarness.EatGhost(ghost1);

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.Move();
            await gameHarness.WaitForFrightenedTimeToComplete();
            await gameHarness.WaitForScatterToComplete();
            await gameHarness.Move();
            await gameHarness.Move();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Status.Should().Be(GhostStatus.Alive);
            gameHarness.Game.Ghosts[ghost1.Name].Location.Should().Be(ghostHouse);
        }

        [Fact]
        public async Task GhostShouldNotTransitionToEdibleWhenInRunningHome()
        {
            //      P     
            //   .  *          
            //      G                H  
            //                 
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .WithDirection(Direction.Left)
                .Create();

            _gameSettings.GhostHouse.Add(_gameSettings.PacMan.Location.FarAway());

            _gameSettings.Ghosts.Add(ghost1);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below);

            await gameHarness.EatGhost(ghost1);
            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below.Below);

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.Move();
            await gameHarness.WaitForFrightenedTimeToComplete();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts[ghost1.Name].Status.Should().Be(GhostStatus.RunningHome);
        }

        [Fact]
        public async Task GhostNotBeRunningHomeAfterPacManDiesAndGameResets()
        {
            //       P     
            //             
            //     . *          
            // G G . .                H  
            //                 
            var ghostStart1 = _gameSettings.PacMan.Location.Below.Below.Below.Left.Left;
            var ghost1 = GhostBuilder.New()
                .WithLocation(ghostStart1)
                .WithChaseStrategyRight()
                .WithScatterStrategyRight()
                .WithFrightenedStrategy(new StandingStillGhostStrategy())
                .Create();
            var killerGhost = GhostBuilder.New()
                .WithLocation(ghostStart1.Left)
                .WithChaseStrategyRight()
                .WithScatterTarget(ghostStart1.Right.Right.Right)
                .WithFrightenedStrategy(new StandingStillGhostStrategy())
                .WithDirection(Direction.Right)
                .Create();

            _gameSettings.Walls.Add(_gameSettings.PacMan.Location.Below.Below.Below.Below);

            _gameSettings.GhostHouse.Add(_gameSettings.PacMan.Location.FarAway());

            _gameSettings.Ghosts.Add(ghost1);
            _gameSettings.Ghosts.Add(killerGhost);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Below.Below);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Down);
            await gameHarness.Move();

            await gameHarness.EatPill();

            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below.Below);
            gameHarness.WeExpectThatGhost(ghost1).IsAt(ghostStart1.Right.Right);

            await gameHarness.EatGhost(ghost1);
            gameHarness.WeExpectThatPacMan().IsAt(_gameSettings.PacMan.Location.Below.Below.Below);

            await gameHarness.WaitForPauseToComplete();
            await gameHarness.WaitForFrightenedTimeToComplete();
            await gameHarness.Move();
            await gameHarness.GetEatenByGhost(killerGhost);

            await gameHarness.WaitToFinishDying();
            await gameHarness.WaitToRespawn();

            using var _ = new AssertionScope();
            gameHarness.Game.Ghosts.Values
                .Select(x => x.Status)
                .Should().AllBeEquivalentTo(GhostStatus.Alive);
        }

        [Fact]
        public async Task GhostsShouldFlashJustBeforeAlive()
        {
            var ghosts = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.FarAway())
                .WithScatterStrategy(new StandingStillGhostStrategy())
                .CreateMany(3);
            _gameSettings.Ghosts.AddRange(ghosts);

            _gameSettings.PowerPills.Add(_gameSettings.PacMan.Location.Right);

            var gameHarness = new GameHarness(_gameSettings);
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.EatPill();

            gameHarness.EnsureAllGhostsAreEdible();

            await gameHarness.Move();

            gameHarness.EnsureAllGhostsAreEdible();

            await gameHarness.WaitForGhostFlash();

            gameHarness.Game.Ghosts.Values.Should().AllBeEquivalentTo(new
            {
                Status = GhostStatus.Flash,
                Edible = true
            });
        }

        [Fact]
        public async Task CanEatGhostAndCoinOnSameLocation()
        {
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Right)
                .WithScatterStrategyStill()
                .WithFrightenedStrategyStill()
                .WithChaseStrategyStill()
                .Create();

            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.CreateBoard("P * .",
                                      ".    ",
                                      "H    ");

            var gameHarness = new GameHarness(_gameSettings);
            
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.EatPill();

            if (gameHarness.Game.Coins.Count != 2)
            {
                throw new Exception($"There should be 2 coins not {gameHarness.Game.Coins.Count} coins.");
            }

            await gameHarness.EatGhost(ghost);
            
            gameHarness.Game.Coins.Should().HaveCount(1);
        }

        [Fact]
        public async Task CanEatGhostAndPowerPillOnSameLocation()
        {
            var ghost = GhostBuilder.New()
                .WithLocation(_gameSettings.PacMan.Location.Right.Right)
                .WithScatterStrategyStill()
                .WithFrightenedStrategyStill()
                .WithChaseStrategyStill()
                .Create();

            _gameSettings.Ghosts.Add(ghost);
            _gameSettings.CreateBoard("P * *",
                                      ".    ",
                                      "H    ");

            var gameHarness = new GameHarness(_gameSettings);
            
            await gameHarness.PlayGame();

            await gameHarness.ChangeDirection(Direction.Right);
            await gameHarness.EatPill();

            if (gameHarness.Game.PowerPills.Count != 2)
            {
                throw new Exception($"There should be 2 pills not {gameHarness.Game.PowerPills.Count} pills.");
            }

            await gameHarness.EatGhost(ghost);
            
            gameHarness.Game.PowerPills.Should().HaveCount(1);
        }
    }
}
