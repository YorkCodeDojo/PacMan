using Automatonymous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    internal static class Actions
    {
        public static void Tick(GameState gameState, DateTime now, GameNotifications gameNotifications)
        {
            gameNotifications.Publish(GameNotification.PreTick);
            gameState.RecordLastTick(now);
        }

        public static void BeginDying(GameState gameState, GameNotifications gameNotifications)
        {
            gameState.HideGhosts();
            gameNotifications.Publish(GameNotification.Dying);
        }

        public static void GetReadyForNextLevel(GameState gameState, IGameSettings gameSettings)
        {
            gameState.IncrementLevel();
            MovePacManHome(gameState, gameSettings);
            MoveGhostsHome(gameState);
            MakeGhostsNotEdible(gameState);
            gameState.ShowGhosts();
            gameState.ReplaceCoins(gameSettings.Coins);
            gameState.ReplacePowerPills(gameSettings.PowerPills);
            gameState.HideFruit();
        }

        public static void BeginRespawning(GameNotifications gameNotifications)
        {
            gameNotifications.Publish(GameNotification.Respawning);
        }

        public static void CompleteRespawning(GameState gameState, IGameSettings gameSettings)
        {
            MakeGhostsNotEdible(gameState);
            MoveGhostsHome(gameState);
            MovePacManHome(gameState, gameSettings);
            gameState.ShowGhosts();
        }

        public static void SetupGame(GameState gameState, DateTime now, GameNotifications gameNotifications)
        {
            gameState.RecordLastTick(now);
            MoveGhostsHome(gameState);
            gameState.ShowGhosts();
            gameNotifications.Publish(GameNotification.Beginning);
        }

    private static Dictionary<int, FruitType> FruitForLevel = new Dictionary<int, FruitType> {
        [1] = FruitType.Cherry,
        [2] = FruitType.Strawberry,
        [3] = FruitType.Orange,
        [4] = FruitType.Orange,
        [5] = FruitType.Bell,
        [6] = FruitType.Bell,
        [7] = FruitType.Apple,
        [8] = FruitType.Apple,
        [9] = FruitType.Grapes,
        [10] = FruitType.Grapes,
        [11] = FruitType.Arcadian,
        [12] = FruitType.Arcadian
    };
        public static void CoinEaten(Game game, IGameSettings settings, GameState gameState, CellLocation coinLocation, GameNotifications gameNotifications)
        {
            gameState.RemoveCoin(coinLocation);
            gameState.IncreaseScore(10);
            if(settings.FruitAppearsAfterCoinsEaten.Contains(game.StartingCoins.Count - game.Coins.Count))
            {
                if(!FruitForLevel.TryGetValue(gameState.Level, out var fruitType))
                {
                    fruitType = FruitType.Key;
                }
                gameState.ShowFruit(settings.FruitVisibleForSeconds, fruitType);
            }
            gameNotifications.Publish(GameNotification.EatCoin);
        }

        public static void PowerPillEaten(IGameSettings gameSettings, GameState gameState, CellLocation powerPillLocation, GameNotifications gameNotifications)
        {
            gameState.IncreaseScore(50);
            gameNotifications.Publish(GameNotification.EatPowerPill);
            MakeGhostsEdible(gameSettings, gameState);
            gameState.RemovePowerPill(powerPillLocation);
        }

        public static void GhostEaten(GameState gameState, Ghost ghost, Game game, GameNotifications gameNotifications)
        {
            SendGhostHome(gameState, ghost);
            IncreaseScoreAfterEatingGhost(gameState, game);
            MakeGhostNotEdible(gameState, ghost);
            gameNotifications.Publish(GameNotification.EatGhost);
        }

        internal static void FruitEaten(Game game, IGameSettings settings, GameState gameState, CellLocation location, GameNotifications gameNotifications)
        {
            var scoreInc = gameState.FruitTypeToShow switch {
                FruitType.Cherry => 100,
                FruitType.Strawberry => 300,
                FruitType.Orange => 500,
                FruitType.Bell => 700,
                FruitType.Apple => 1000,
                FruitType.Grapes => 2000,
                FruitType.Arcadian => 3000,
                FruitType.Key => 5000,
                _ => throw new NotImplementedException()
            };
            
            gameState.IncreaseScore(scoreInc);
            gameState.HideFruit();
            gameNotifications.Publish(GameNotification.EatFruit);
        }

        public static void EatenByGhost(GameState gameState)
        {
            gameState.DecreaseLives();
        }

        public static void ChangeDirection(Game game, GameState gameState, Direction direction)
        {
            var nextSpace = gameState.PacMan.Location + direction;
            if (!game.Walls.Contains(nextSpace))
            {
                gameState.ChangeDirectionOfPacMan(direction);
            }
        }

        public static void ScatterGhosts(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.Scatter());
        }

        public static void GhostToChase(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.Chase());
        }

        public static void MakeGhostsNotEdible(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToNotEdible());
        }

        public static async Task MovePacMan(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine, IGameSettings settings)
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

            if (settings.Fruit == newPacManLocation && gameState.FruitVisible )
            {
                await context.Raise(gameStateMachine.FruitCollision, new FruitCollision(newPacManLocation));
            }
        }

        public static async Task MoveGhosts(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            gameState.ApplyToGhosts(ghost => ghost.Move(game, gameState));

            var ghosts = GhostsCollidedWithPacMan(gameState);
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }
        }

        private static IEnumerable<Ghost> GhostsCollidedWithPacMan(GameState gameState)
        {
            return gameState.Ghosts.Values.Where(ghost => ghost.Location == gameState.PacMan.Location);
        }

        private static void IncreaseScoreAfterEatingGhost(GameState gameState, Game game)
        {
            var numberOfInEdibleGhosts = game.Ghosts.Values.Count(g => !g.Edible);
            var increaseInScore = (int)Math.Pow(2, numberOfInEdibleGhosts) * 200;
            gameState.IncreaseScore(increaseInScore);
        }

        private static void MakeGhostNotEdible(GameState gameState, Ghost ghostToUpdate)
        {
            gameState.ApplyToGhost(ghost => ghost.SetToNotEdible(), ghostToUpdate);
        }

        private static void MovePacManHome(GameState gameState, IGameSettings gameSettings) => gameState.ReplacePacMan(gameSettings.PacMan);

        private static void SendGhostHome(GameState gameState, Ghost ghostToUpdate)
        {
            gameState.ApplyToGhost(ghost => ghost.SetToHome(), ghostToUpdate);
        }

        private static void MoveGhostsHome(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToHome());
        }

        private static void MakeGhostsEdible(IGameSettings gameSettings, GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToEdible(gameSettings.DirectionPicker));
        }
    }
}
