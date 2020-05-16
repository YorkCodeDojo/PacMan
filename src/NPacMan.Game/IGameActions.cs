using System;
using System.Threading.Tasks;
using Automatonymous;

namespace NPacMan.Game
{
    internal interface IGameActions
    {
        void MoveGhostsHome();
        void ShowGhosts(GameState gameState);
        void ScatterGhosts();
        void GhostToChase();
        void MakeGhostsEdible();
        Task MovePacMan(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine);
        Task MoveGhosts(BehaviorContext<GameState, Tick> context, GameStateMachine gameStateMachine);
        void HideGhosts(GameState gameState);
        void MovePacManHome();
        void MakeGhostsNotEdible();
        void SendGhostHome(Ghost ghost);
    }
}