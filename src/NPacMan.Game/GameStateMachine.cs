using Automatonymous;

namespace NPacMan.Game
{
    internal class GameStateMachine :
        AutomatonymousStateMachine<GameState>
    {
        public GameStateMachine(IGameActions game, IGameSettings settings)
        {
            InstanceState(x => x.Status);

            Initially(
                When(Tick)
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.ShowGhosts(context.Instance))
                    .Then(context => game.ScatterGhosts())
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.InitialScatterTimeInSeconds))
                    .TransitionTo(Scatter));

            During(Scatter,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.ChaseTimeInSeconds))
                    .Then(context => game.GhostToChase())
                    .TransitionTo(Alive));

            During(Alive,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(settings.InitialScatterTimeInSeconds))
                    .Then(context => game.ScatterGhosts())
                    .TransitionTo(Scatter));

            During(Scatter, Alive,
                When(Tick)
                    .ThenAsync(async context => await game.MoveGhosts(context.Data.Now))
                    .Then(context => game.MovePacMan(context.Data.Now)),
                When(CoinEaten)
                    .Then(context => context.Instance.Score += 10),
                //.Then(context => game._soundSet.Chomp()),
                When(PacManCaughtByGhost)
                    .Then(context => context.Instance.Lives -= 1)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .TransitionTo(Dying));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => game.HideGhosts(context.Instance))
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    //.Then(context => game._soundSet.Death())
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.Finalize()));

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                    .Then(context => game.MoveGhostsHome())
                    .Then(context => game.MovePacManHome())
                    .Then(context => game.ShowGhosts(context.Instance))
                    .TransitionTo(Alive));

            During(Dead, Ignore(Tick));
        }


        public State Alive { get; private set; } = null!;

        public State Scatter { get; private set; } = null!;
        public State Dying { get; private set; } = null!;
        public State Respawning { get; private set; } = null!;
        public State Dead { get; private set; } = null!;
        public Event<Tick> Tick { get; private set; } = null!;
        public Event<Tick> PacManCaughtByGhost { get; private set; } = null!;
        public Event CoinEaten { get; private set; } = null!;
    }
}