using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TrackManager _trackManager;
    [SerializeField] private ThemeData _themeData;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<TrackManager>();

        builder.RegisterComponentInHierarchy<EnergyUIManager>();

        builder.Register<ScoreManager>(Lifetime.Singleton);

        builder.Register<EnergySpawner>(Lifetime.Singleton)
          .WithParameter(_themeData);

        RegisterPlayer(builder);
        RegisterStates(builder);

        RegisterSaveAndLeaderboard(builder);

        builder.RegisterComponentInHierarchy<ExitHandler>();
    }

    private void RegisterPlayer(IContainerBuilder builder)
    {
        builder.RegisterInstance(new PlayerControls());

        builder.Register<PlayerInput>(Lifetime.Scoped);

        builder.RegisterComponentInHierarchy<PlayerMovement>();

        builder.RegisterComponentInHierarchy<PlayerDeathHandler>()
            .WithParameter(_animator);

        builder.RegisterComponentInHierarchy<PlayerCollisionHandler>();
    }

    private void RegisterStates(IContainerBuilder builder)
    {
        var stateMachine = new GameStateMachine();
        var gameplayState = new GameplayState(_trackManager);

        builder.RegisterInstance(stateMachine).AsSelf();
        builder.RegisterInstance(gameplayState).As<IGameState>();

        stateMachine.SetState(gameplayState).Forget();

        Debug.Log(stateMachine.CurrentState.ToString());
    }

    private void RegisterSaveAndLeaderboard(IContainerBuilder builder)
    {
        IDataService data = new JsonDataService();

        var leaderboardManager = new LeaderboardManager(data);
        builder.RegisterInstance(leaderboardManager);
    }
}
