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
            MakeGhostsNotEdible(gameState);
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
            MakeGhostsNotEdible(gameState);
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

            if(!bonusLifeAlreadyAwarded && gameState.Score >= _gameSettings.PointsNeededForBonusLife)
            {
                gameState.AddBonusLife();
                _gameNotifications.Publish(GameNotification.ExtraPac);
            }
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
            SendGhostHome(gameState, ghost);
            IncreaseScoreAfterEatingGhost(gameState, game);
            MakeGhostNotEdible(gameState, ghost);
            _gameNotifications.Publish(GameNotification.EatGhost);
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
            if (!game.Walls.Contains(nextSpace))
            {
                gameState.ChangeDirectionOfPacMan(direction);
            }
        }

        internal void ScatterGhosts(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.Scatter());
        }

        internal void GhostToChase(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.Chase());
        }

        internal void MakeGhostsNotEdible(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToNotEdible());
        }

        internal async Task MovePacMan(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            var newPacManLocation = gameState.PacMan.Location + gameState.PacMan.Direction;

            if (game.Portals.TryGetValue(newPacManLocation, out var otherEndOfThePortal))
            {
                newPacManLocation = otherEndOfThePortal + gameState.PacMan.Direction;
            }

            if (!game.Walls.Contains(newPacManLocation))
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
            gameState.ApplyToGhosts(ghost => ghost.Move(game, gameState));

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
            IncreaseScoreAndCheckForBonusLife(gameState, increaseInScore);
        }

        private void MakeGhostNotEdible(GameState gameState, Ghost ghostToUpdate)
        {
            gameState.ApplyToGhost(ghost => ghost.SetToNotEdible(), ghostToUpdate);
        }

        private void MovePacManHome(GameState gameState) => gameState.ReplacePacMan(_gameSettings.PacMan);

        private void SendGhostHome(GameState gameState, Ghost ghostToUpdate)
        {
            gameState.ApplyToGhost(ghost => ghost.SetToHome(), ghostToUpdate);
        }

        private void MoveGhostsHome(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToHome());
        }

        private void MakeGhostsEdible(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToEdible(_gameSettings.DirectionPicker));
        }

        public void Start()
        {
            _gameNotifications.Publish(GameNotification.Beginning);
        }
    }
}
