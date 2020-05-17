using System;
using System.Collections.Generic;

namespace NPacMan.Game
{
    internal class GameState
    {
        public GameState(IGameSettings settings)
        {
            Lives = settings.InitialLives;
            Status = settings.InitialGameStatus switch
            {
                GameStatus.Initial => nameof(GameStateMachine.Initial),
                GameStatus.Alive => nameof(GameStateMachine.GhostChase),
                GameStatus.Dying => nameof(GameStateMachine.Dying),
                GameStatus.Respawning => nameof(GameStateMachine.Respawning),
                GameStatus.Dead => nameof(GameStateMachine.Dead),
                _ => throw new NotImplementedException($"No map for InitialGameStatus '{settings.InitialGameStatus}")
            };
            Score = 0;
            GhostsVisible = true;
            TimeToChangeState = null;
            RemainingCoins = new List<CellLocation> ( settings.Coins );
            RemainingPowerPills = new List<CellLocation> ( settings.PowerPills );
        }

        public string Status { get; set; } = null!;

        public DateTime? TimeToChangeState { get; set; }

        public int Lives { get; set; }

        public bool GhostsVisible { get; set; }

        public int Score { get; set; }

        public DateTime LastTick { get; set; }

        public List<CellLocation> RemainingCoins { get; set; }

        public List<CellLocation> RemainingPowerPills { get; set; }
    }
}