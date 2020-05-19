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

        private static void ApplyToGhosts(GameState gameState, Func<Ghost, Ghost> action)
        {
            var newPositionOfGhosts = new Dictionary<string, Ghost>();
            foreach (var ghost in gameState.Ghosts.Values)
            {
                newPositionOfGhosts[ghost.Name] = action(ghost);
            }

            gameState.Ghosts = newPositionOfGhosts;
        }

        public static void ScatterGhosts(GameState gameState)
        {
            ApplyToGhosts(gameState, ghost => ghost.Scatter());
        }

        public static void GhostToChase(GameState gameState)
        {
            ApplyToGhosts(gameState, ghost => ghost.Chase());
        }

        public static void MoveGhostsHome(GameState gameState)
        {
            ApplyToGhosts(gameState, ghost => ghost.SetToHome());
        }

        public static void MakeGhostsEdible(GameState gameState)
        {
            ApplyToGhosts(gameState, ghost => ghost.SetToEdible());
        }

        public static void MakeGhostsNotEdible(GameState gameState)
        {
            ApplyToGhosts(gameState, ghost => ghost.SetToNotEdible());
        }

        public static void MovePacManHome(GameState gameState)
        {
            gameState.PacMan = gameState.PacMan.SetToHome();
        }

        public static void SendGhostHome(GameState gameState, Ghost ghostToSendHome)
        {
            ApplyToGhosts(gameState, ghost =>
            {
                if (ghost.Name == ghostToSendHome.Name)
                {
                    ghost = ghost.SetToHome();
                }
                return ghost;
            });
        }

        public async static Task MovePacMan(IGameSettings gameSettings, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            var newPacMan = gameState.PacMan.Move();

            if (gameSettings.Portals.TryGetValue(newPacMan.Location, out var portal))
            {
                newPacMan = gameState.PacMan.WithNewX(portal.X).WithNewY(portal.Y);
                newPacMan = newPacMan.Move();
            }

            if (!gameSettings.Walls.Contains(newPacMan.Location))
            {
                gameState.PacMan = newPacMan;
            }

            var ghosts = GhostsCollidedWithPacMan(gameState);
            foreach (var ghost in ghosts)
            {
                await context.Raise(gameStateMachine.GhostCollision, new GhostCollision(ghost));
            }

            if (gameState.RemainingCoins.Contains(newPacMan.Location))
            {
                await context.Raise(gameStateMachine.CoinCollision, new CoinCollision(newPacMan.Location));
            }

            if (gameState.RemainingPowerPills.Contains(newPacMan.Location))
            {
                await context.Raise(gameStateMachine.PowerPillCollision, new PowerPillCollision(newPacMan.Location));
            }
        }

        public static async Task MoveGhosts(Game game, GameState gameState, BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine)
        {
            ApplyToGhosts(gameState, ghost => ghost.Move(game));

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
    }
}
