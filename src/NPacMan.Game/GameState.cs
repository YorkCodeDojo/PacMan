using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NPacMan.Game
{
    internal class GameState : IReadOnlyGameState
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
            RemainingCoins = new List<CellLocation>(settings.Coins);
            RemainingPowerPills = new List<CellLocation>(settings.PowerPills);
            PacMan = settings.PacMan;
            Ghosts = settings.Ghosts.ToDictionary(x => x.Name, x => x);
        }

        public string Status { get; set; } = null!;

        public DateTime? TimeToChangeState { get; private set; }

        public int Lives { get; private set; }

        public bool GhostsVisible { get; private set; }

        public int Score { get; private set; }

        public DateTime LastTick { get; private set; }

        public int TickCounter => _tickCounter;

        private int _tickCounter;

        public IReadOnlyCollection<CellLocation> RemainingCoins { get; private set; }

        public IReadOnlyCollection<CellLocation> RemainingPowerPills { get; private set; }

        public IReadOnlyDictionary<string, Ghost> Ghosts { get; private set; }

        public PacMan PacMan { get; private set; }

        internal void RemoveCoin(CellLocation location)
        {
            // Note - this is not the same as gameState.RemainingCoins = gameState.RemainingCoins.Remove(location)
            // We have to allow for the UI to be iterating over the list whilst we are removing elements from it.
            RemainingCoins = RemainingCoins.Where(c => c != location).ToList();
        }

        internal void RemovePowerPill(CellLocation location)
        {
            // Note - this is not the same as gameState.RemainingPowerPills = gameState.RemainingPowerPills.Remove(location)
            // We have to allow for the UI to be iterating over the list whilst we are removing elements from it.
            RemainingPowerPills = RemainingPowerPills.Where(p => p != location).ToList();
        }

        internal void MovePacManTo(CellLocation newPacManLocation)
        {
            PacManEventSource.Log.PacManMoved(TickCounter, PacMan.Location.X, PacMan.Location.Y, newPacManLocation.X, newPacManLocation.Y);

            PacMan = PacMan.WithNewLocation(newPacManLocation);
        }

        internal void IncreaseScore(int amount)
        {
            Score += amount;
            PacManEventSource.Log.PacManStateChanged(TickCounter, Lives, Score, PacMan.Location.X, PacMan.Location.Y, PacMan.Direction.ToText());
        }

        internal void DecreaseLives()
        {
            Lives--;
            PacManEventSource.Log.PacManStateChanged(TickCounter, Lives, Score, PacMan.Location.X, PacMan.Location.Y, PacMan.Direction.ToText());
        }

        internal void ShowGhosts()
        {
            GhostsVisible = true;
        }

        internal void HideGhosts()
        {
            GhostsVisible = false;
        }

        internal void RecordLastTick(DateTime now)
        {
            Interlocked.Increment(ref _tickCounter);

            LastTick = now;
        }

        internal void ChangeStateIn(int timeInSeconds)
        {
            TimeToChangeState = LastTick.AddSeconds(timeInSeconds);
        }

        internal void ApplyToGhosts(Func<Ghost, Ghost> action)
        {
            var newPositionOfGhosts = new Dictionary<string, Ghost>(Ghosts.Count);
            foreach (var ghost in Ghosts.Values)
            {
                newPositionOfGhosts[ghost.Name] = action(ghost);
            }

            Ghosts = newPositionOfGhosts;
        }

        internal void ApplyToGhost(Func<Ghost, Ghost> action, Ghost ghostToUpdate)
        {
            ApplyToGhosts(ghost =>
            {
                if (ghost.Name == ghostToUpdate.Name)
                {
                    ghost = action(ghost);
                }
                return ghost;
            });
        }

        internal void MovePacManHome()
        {
            PacManEventSource.Log.PacManMoved(TickCounter, PacMan.Location.X, PacMan.Location.Y, PacMan.Home.X, PacMan.Home.Y);

            PacMan = PacMan.SetToHome();
        }

        internal void ChangeDirectionOfPacMan(Direction direction)
        {
            if (direction != PacMan.Direction)
            {
                PacMan = PacMan.WithNewDirection(direction);

                PacManEventSource.Log.PacManStateChanged(TickCounter, Lives, Score, PacMan.Location.X, PacMan.Location.Y, PacMan.Direction.ToText());
            }
        }
    }
}