using Automatonymous;

namespace NPacMan.Game
{
    partial class Game
    {
        class GameStateMachine :
            AutomatonymousStateMachine<GameState>
        {
            public GameStateMachine(Game game)
            {
                InstanceState(x => x.Status);

                Initially(
                    When(Tick)
                        .Then(context => game._gameNotifications.Publish(GameAction.Beginning))
                        .Then(context => game.MoveGhostsHome())
                        .Then(context => game.ShowGhosts())
                        .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(game._settings.InitialScatterTimeInSeconds))
                        .TransitionTo(InitialScatter));

                WhenEnter(InitialScatter,
                          binder => binder.Then(context => game.ScatterGhosts()));

                During(InitialScatter,
                     When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                        .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(game._settings.ChaseTimeInSeconds))
                         .Then(context => game.GhostToChase())
                         .TransitionTo(Alive));

                During(Alive,
                     When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                        .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(game._settings.InitialScatterTimeInSeconds))
                         .Then(context => game.ScatterGhosts())
                         .TransitionTo(InitialScatter));

                During(InitialScatter, Alive,
                    When(Tick)
                        .Then(context => game.MoveGhosts(context.Data.Now))
                        .Then(context => game.MovePacMan(context.Data.Now)),
                    When(CoinEaten)
                        .Then(context => context.Instance.Score += 10)
                        .Then(context => game._gameNotifications.Publish(GameAction.EatCoin)),
                    When(PacManCaughtByGhost)
                        .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                        .TransitionTo(Dying));

                WhenEnter(Dying,
                          binder => binder
                                    .Then(context => context.Instance.Lives -= 1)
                                    .Then(context => game._gameNotifications.Publish(GameAction.Dying)));

                During(Dying,
                    When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                        .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                        .IfElse(context => context.Instance.Lives > 0,
                            binder => binder.TransitionTo(Respawning),
                            binder => binder.Finalize()));

                WhenEnter(Respawning,
                          binder => binder
                                    .Then(context => game._gameNotifications.Publish(GameAction.Respawning))
                                    .Then(context => game.HideGhosts()));

                During(Respawning,
                    When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                        .Then(context => context.Instance.TimeToChangeState = context.Data.Now.AddSeconds(4))
                        .Then(context => game._gameNotifications.Publish(GameAction.Reborn))
                        .Then(context => game.MoveGhostsHome())
                        .Then(context => game.MovePacManHome())
                        .Then(context => game.ShowGhosts())
                        .TransitionTo(Alive));
            }

            public State Alive { get; private set; } = null!;
            public State InitialScatter { get; private set; } = null!;
            public State Dying { get; private set; } = null!;
            public State Respawning { get; private set; } = null!;
            public Event<Tick> Tick { get; private set; } = null!;
            public Event<Tick> PacManCaughtByGhost { get; private set; } = null!;
            public Event CoinEaten { get; private set; } = null!;
        }

    }
}
