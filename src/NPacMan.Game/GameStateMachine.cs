using Automatonymous;
using System;

namespace NPacMan.Game
{
    internal class GameStateMachine :
        AutomatonymousStateMachine<GameState>
    {
        public GameStateMachine(Actions actions, IGameSettings settings, Game game)
        {
            InstanceState(x => x.Status);

            DuringAny(
                When(Tick)
                    .Then(context => actions.Tick(context.Instance, context.Data.Now)));

            Initially(
                When(Tick)
                    .Then(context => actions.Tick(context.Instance, context.Data.Now))
                    .TransitionTo(AttractMode));

            WhenEnter(AttractMode,
                binder => binder
                    .Then(context => actions.SetupGame(context.Instance)));

            During(AttractMode,
                When(PressStart)
                    .Then(context => actions.Start())
                    .TransitionTo(Scatter));

            WhenEnter(Scatter,
                       binder => binder
                       .Then(context => context.Instance.ChangeStateIn(settings.InitialScatterTimeInSeconds))
                       .Then(context => actions.ScatterGhosts(context.Instance)));

            During(Scatter,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(GhostChase));

            During(ChangingLevel,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(Scatter));

            WhenLeave(ChangingLevel, binder => binder.Then(context => actions.GetReadyForNextLevel(context.Instance)));
            
            WhenEnter(GhostChase,
                       binder => binder
                            .Then(context => context.Instance.ChangeStateIn(settings.ChaseTimeInSeconds))
                            .Then(context => actions.GhostToChase(context.Instance)));

            During(GhostChase,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .TransitionTo(Scatter));

            During(Frightened,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => actions.MakeGhostsNotEdible(context.Instance))
                    .TransitionTo(Scatter));

            During(Scatter, GhostChase, Frightened,
                When (PlayersWishesToChangeDirection)
                    .Then(context => actions.ChangeDirection(game, context.Instance, context.Data.NewDirection)),
                When(Tick)
                    .ThenAsync(async context => await actions.MoveGhosts(game, context.Instance, context, this))
                    .ThenAsync(async context => await actions.MovePacMan(game, context.Instance, context, this)),
                When(CoinCollision)
                    .Then(context => actions.CoinEaten(game, context.Instance, context.Data.Location))
                    .If(context => context.Instance.IsLevelComplete(), 
                            binder => binder.TransitionTo(ChangingLevel)),
                When(FruitCollision)
                    .Then(context => actions.FruitEaten(game, context.Instance, context.Data.Location)),
                When(PowerPillCollision)
                    .Then(context => actions.PowerPillEaten(context.Instance, context.Data.Location))
                    .IfElse(context => context.Instance.IsLevelComplete(), 
                            binder => binder.TransitionTo(ChangingLevel),
                        binder =>
                            binder.Then(context => context.Instance.ChangeStateIn(settings.FrightenedTimeInSeconds))
                                .TransitionTo(Frightened)),
                When(GhostCollision)
                    .If(x => x.Data.Ghost.Edible,
                        binder => binder.Then(context => actions.GhostEaten(context.Instance, context.Data.Ghost, game))
                                .TransitionTo(EatingGhost))
                    .If(x => x.Data.Ghost.Status == GhostStatus.Alive,
                        binder => binder.Then(context => actions.EatenByGhost(context.Instance))
                                    .TransitionTo(Dying))); 


            WhenEnter(ChangingLevel,
                       binder => binder
                                .Then(context => context.Instance.HideGhosts())
                                .Then(context => context.Instance.ChangeStateIn(4)));


            WhenEnter(Dying,
                       binder => binder
                                .Then(context => actions.BeginDying(context.Instance))
                                .Then(context => context.Instance.ChangeStateIn(4)));

            During(Dying,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .IfElse(context => context.Instance.Lives > 0,
                        binder => binder.TransitionTo(Respawning),
                        binder => binder.TransitionTo(AttractMode)));

            WhenEnter(Respawning,
                       binder => binder
                                .Then(context => context.Instance.ChangeStateIn(4))
                                .Then(context => actions.BeginRespawning()));

            WhenEnter(EatingGhost,
                       binder => binder
                                .Then(context => context.Instance.ResumeStateIn(1)));

            During(EatingGhost,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToResumeState)
                    .Then(context => actions.SendGhostHome1(context.Instance))
                    .TransitionTo(Frightened),
                When(GhostCollision)
                    .IfElse(x => x.Data.Ghost.Edible,
                    binder => binder.Then(context => actions.GhostEaten(context.Instance, context.Data.Ghost, game))
                                .TransitionTo(EatingGhost),
                    binder => binder.Then(context => actions.EatenByGhost(context.Instance))
                                    .TransitionTo(Dying))); 

            During(Respawning,
                When(Tick, context => context.Data.Now >= context.Instance.TimeToChangeState)
                    .Then(context => actions.CompleteRespawning(context.Instance))
                    .TransitionTo(Scatter));

            During(AttractMode, Ignore(Tick));

            During(EatingGhost,
                    Ignore(PlayersWishesToChangeDirection),
                    Ignore(CoinCollision),
                    Ignore(PowerPillCollision));

            During(new [] {Dying, Respawning, AttractMode, ChangingLevel},
                    Ignore(PlayersWishesToChangeDirection),
                    Ignore(CoinCollision),
                    Ignore(PowerPillCollision),
                    Ignore(GhostCollision));
        }

        public State GhostChase { get; private set; } = null!;
        public State Scatter { get; private set; } = null!;
        public State Frightened { get; private set; } = null!;
        public State Dying { get; private set; } = null!;
        public State Respawning { get; private set; } = null!;
        public State Dead { get; private set; } = null!;
        public State ChangingLevel { get; private set; } = null!;
        public State AttractMode { get; private set; } = null!;
        public State EatingGhost { get; private set; } = null!;
        public Event<Tick> Tick { get; private set; } = null!;
        public Event<GhostCollision> GhostCollision { get; private set; } = null!;
        public Event<CoinCollision> CoinCollision { get; private set; } = null!;
        public Event<PowerPillCollision> PowerPillCollision { get; private set; } = null!;
        public Event<PlayersWishesToChangeDirection> PlayersWishesToChangeDirection { get; private set; } = null!;
        public Event<FruitCollision> FruitCollision { get; private set; } = null!;
        public Event<PressStart> PressStart { get; private set; } = null!;
    }
}