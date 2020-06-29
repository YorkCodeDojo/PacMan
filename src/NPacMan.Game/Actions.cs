using Automatonymous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    internal class Actions
    {
        private readonly IGameSettings _gameSettings;
        private readonly GameNotifications _gameNotifications;

        internal Actions(IGameSettings gameSettings, GameNotifications gameNotifications)
        {
            _gameSettings = gameSettings;
            _gameNotifications = gameNotifications;
        }

        internal void Tick(GameState gameState, DateTime now)
        {
            _gameNotifications.Publish(GameNotification.PreTick);
            gameState.RecordLastTick(now);
        }

        internal async Task ReloadHighScore(GameState gameState)
        {
            var previousHighScore = await _gameSettings.HighScoreStorage.GetHighScore();
            gameState.UpdateHighScore(previousHighScore);
        }

        internal void BeginDying(GameState gameState)
        {
            gameState.HideGhosts();
            _gameNotifications.Publish(GameNotification.Dying);
        }

        internal void GetReadyForNextLevel(GameState gameState)
        {
            gameState.IncrementLevel();
            ResetBoard(gameState);
        }

        private void ResetBoard(GameState gameState)
        {
            MovePacManHome(gameState);
            MoveGhostsHome(gameState);
            ResetAllGhosts(gameState);
            gameState.ShowGhosts();
            gameState.ReplaceCoins(_gameSettings.Coins);
            gameState.ReplacePowerPills(_gameSettings.PowerPills);
            gameState.HideFruit();
        }

        internal void BeginRespawning()
        {
            _gameNotifications.Publish(GameNotification.Respawning);
        }

        internal void CompleteRespawning(GameState gameState)
        {
            ResetAllGhosts(gameState);
            MoveGhostsHome(gameState);
            MovePacManHome(gameState);
            gameState.ShowGhosts();
        }

        internal void SetupGame(GameState gameState)
        {
            ResetBoard(gameState);
            gameState.ResetLives(_gameSettings.InitialLives);
            gameState.ResetScore();
        }


        private void IncreaseScoreAndCheckForBonusLife(GameState gameState, int amount)
        {
            var bonusLifeAlreadyAwarded = (gameState.Score >= _gameSettings.PointsNeededForBonusLife);

            gameState.IncreaseScore(amount);

            if (gameState.Score > gameState.HighScore)
            {
                gameState.UpdateHighScore(gameState.Score);
            }

            if(!bonusLifeAlreadyAwarded && gameState.Score >= _gameSettings.PointsNeededForBonusLife)
            {
                gameState.AddBonusLife();
                _gameNotifications.Publish(GameNotification.ExtraPac);
            }
        }

        internal void MakeGhostsFlash(GameState gameState)
        {
            var edibleGhosts = gameState.Ghosts.Values.Where(x => x.Status == GhostStatus.Edible);

            gameState.ApplyToGhosts(ghost => ghost.SetToFlash(), edibleGhosts);
        }

        internal void CoinEaten(Game game, GameState gameState, CellLocation coinLocation)
        {
            gameState.RemoveCoin(coinLocation);
            IncreaseScoreAndCheckForBonusLife(gameState, 10);
            if(_gameSettings.FruitAppearsAfterCoinsEaten.Contains(game.StartingCoins.Count - game.Coins.Count))
            {
                var fruitType = Fruits.FruitForLevel(gameState.Level).FruitType;
                gameState.ShowFruit(_gameSettings.FruitVisibleForSeconds, fruitType);
            }
            
            _gameNotifications.Publish(GameNotification.EatCoin);
        }

        internal void PowerPillEaten(GameState gameState, CellLocation powerPillLocation)
        {
            IncreaseScoreAndCheckForBonusLife(gameState, 50);
            _gameNotifications.Publish(GameNotification.EatPowerPill);
            MakeGhostsEdible(gameState);
            gameState.RemovePowerPill(powerPillLocation);
        }

        internal void GhostEaten(GameState gameState, Ghost ghost, Game game)
        {
            IncreaseScoreAfterEatingGhost(gameState, game);
            gameState.ApplyToGhost(ghost => ghost.SetToScore(), ghost);
            _gameNotifications.Publish(GameNotification.EatGhost);
        }

        internal void UpdateMoveClock(GameState instance)
        {
            _gameSettings.MoveClock.UpdateTime(instance.Delta);
        }

        internal void FruitEaten(Game game, GameState gameState, CellLocation location)
        {
            var scoreIncrement = Fruits.FruitForLevel(gameState.Level).Score;
            IncreaseScoreAndCheckForBonusLife(gameState, scoreIncrement);
            gameState.HideFruit();
            _gameNotifications.Publish(GameNotification.EatFruit);
        }

        internal void EatenByGhost(GameState gameState)
        {
            gameState.DecreaseLives();
        }

        internal void ChangeDirection(Game game, GameState gameState, Direction direction)
        {
            var nextSpace = gameState.PacMan.Location + direction;
            if (!game.WallsAndDoors.Contains(nextSpace))
            {
                gameState.ChangeDirectionOfPacMan(direction);
            }
        }

        internal void ScatterGhosts(GameState gameState)
        {
            var ghostsToScatter = gameState.Ghosts.Values.Where(x => x.Status == GhostStatus.Alive);
            gameState.ApplyToGhosts(ghost => ghost.Scatter(), ghostsToScatter);
        }

        internal void GhostToChase(GameState gameState)
        {
            var ghostsToChase = gameState.Ghosts.Values.Where(x => x.Status == GhostStatus.Alive);
            gameState.ApplyToGhosts(ghost => ghost.Chase(), ghostsToChase);
        }

        internal async Task EndGame(GameState instance)
        {
            await _gameSettings.HighScoreStorage.SetHighScore(instance.HighScore);
        }

        internal void ResetAllGhosts(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToAlive());
        }

        internal void MakeGhostsNotFrightened(GameState gameState)
        {
            var edibleGhosts = gameState.Ghosts.Values.Where(x => x.Edible);
            gameState.ApplyToGhosts(ghost => ghost.SetToAlive(), edibleGhosts);
        }

        internal async Task MovePacMan(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            if(!_gameSettings.MoveClock.ShouldPacManMove(game.Level, gameState.Status == gameStateMachine.Frightened.Name))
            {
                return;
            }

            var newPacManLocation = gameState.PacMan.Location + gameState.PacMan.Direction;

            if (game.Portals.TryGetValue(newPacManLocation, out var otherEndOfThePortal))
            {
                newPacManLocation = otherEndOfThePortal + gameState.PacMan.Direction;
            }

            if (!game.WallsAndDoors.Contains(newPacManLocation))
            {
                gameState.MovePacManTo(newPacManLocation);
            }

            var ghosts = GhostsCollidedWithPacMan(gameState);
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }

            if (gameState.RemainingCoins.Contains(newPacManLocation))
            {
                await context.Raise(gameStateMachine.CoinCollision, new CoinCollision(newPacManLocation));
            }

            if (gameState.RemainingPowerPills.Contains(newPacManLocation))
            {
                await context.Raise(gameStateMachine.PowerPillCollision, new PowerPillCollision(newPacManLocation));
            }

            if (_gameSettings.Fruit == newPacManLocation && gameState.FruitVisible )
            {
                await context.Raise(gameStateMachine.FruitCollision, new FruitCollision(newPacManLocation));
            }
        }

        internal async Task MoveGhosts(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            var ghostsToMove = game.Ghosts.Values.Where(x => _gameSettings.MoveClock.ShouldGhostMove(x));
                                
            gameState.ApplyToGhosts(ghost => ghost.Move(game, gameState), ghostsToMove);

            var ghosts = GhostsCollidedWithPacMan(gameState);
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }
        }

        private IEnumerable<Ghost> GhostsCollidedWithPacMan(GameState gameState)
        {
            return gameState.Ghosts.Values.Where(ghost => ghost.Location == gameState.PacMan.Location);
        }

        private void IncreaseScoreAfterEatingGhost(GameState gameState, Game game)
        {
            var numberOfInEdibleGhosts = game.Ghosts.Values.Count(g => !g.Edible);
            var increaseInScore = (int)Math.Pow(2, numberOfInEdibleGhosts) * 200;
            gameState.RecordPointsForEatingLastGhost(increaseInScore);
            IncreaseScoreAndCheckForBonusLife(gameState, increaseInScore);
        }

        private void MovePacManHome(GameState gameState) => gameState.ReplacePacMan(_gameSettings.PacMan);


        public void SendGhostHome1(GameState gameState)
        {
            var ghosts = gameState.Ghosts.Values.Where(x => x.Status == GhostStatus.Score);

            gameState.ApplyToGhosts(ghost => ghost.SendHome(), ghosts);
        }

        private void MoveGhostsHome(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToHome());
        }

        private void MakeGhostsEdible(GameState gameState)
        {
            var ghosts = gameState.Ghosts.Values.Where(x => x.Status == GhostStatus.Alive);
            gameState.ApplyToGhosts(ghost => ghost.SetToEdible(), ghosts);
        }

        public void Start()
        {
            _gameNotifications.Publish(GameNotification.Beginning);
        }
    }
}
