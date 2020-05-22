using Automatonymous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPacMan.Game
{
    internal static class Actions
    {
        public static void ShowGhosts(GameState gameState)
        {
            gameState.ShowGhosts();
        }

        public static void HideGhosts(GameState gameState)
        {
            gameState.HideGhosts();
        }

        private static IEnumerable<Ghost> GhostsCollidedWithPacMan(GameState gameState)
        {
            return gameState.Ghosts.Values.Where(ghost => ghost.Location == gameState.PacMan.Location);
        }

        public static void ScatterGhosts(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.Scatter());
        }

        public static void GhostToChase(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.Chase());
        }

        public static void MoveGhostsHome(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToHome());
        }

        public static void MakeGhostsEdible(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToEdible());
        }

        public static void MakeGhostsNotEdible(GameState gameState)
        {
            gameState.ApplyToGhosts(ghost => ghost.SetToNotEdible());
        }

         public static void MakeGhostNotEdible(GameState gameState,  Ghost ghostToUpdate)
        {
            gameState.ApplyToGhost(ghost => ghost.SetToNotEdible(), ghostToUpdate);
        }

        public static void MovePacManHome(GameState gameState) => gameState.MovePacManHome();

        public static void SendGhostHome(GameState gameState, Ghost ghostToUpdate)
        {
            gameState.ApplyToGhost(ghost => ghost.SetToHome(), ghostToUpdate);
        }

        public async static Task MovePacMan(IGameSettings gameSettings, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            var newPacManLocation = gameState.PacMan.Location + gameState.PacMan.Direction;

            if (gameSettings.Portals.TryGetValue(newPacManLocation, out var otherEndOfThePortal))
            {
                newPacManLocation = otherEndOfThePortal + gameState.PacMan.Direction;
            }

            if (!gameSettings.Walls.Contains(newPacManLocation))
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
        }

        public static async Task MoveGhosts(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            gameState.ApplyToGhosts(ghost => ghost.Move(game));

            var ghosts = GhostsCollidedWithPacMan(gameState);
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }
        }

        public static void RemoveCoin(GameState gameState, CellLocation location)
        {
            gameState.RemoveCoin(location);
        }

        public static void RemovePowerPill(GameState gameState, CellLocation location)
        {
            gameState.RemovePowerPill(location);
        }

        public static void ChangeDirection(IGameSettings gameSettings, GameState gameState, Direction direction)
        {
            var nextSpace = gameState.PacMan.Location + direction;
            if (!gameSettings.Walls.Contains(nextSpace))
            {
                gameState.ChangeDirectionOfPacMan(direction);
            }
        }
    }
}
