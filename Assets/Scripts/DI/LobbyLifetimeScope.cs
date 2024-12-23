using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class LobbyLifetimeScope : LifetimeScope
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Transform _leaderboardContainer;

    protected override void Configure(IContainerBuilder builder)
    {
        IDataService data = new JsonDataService();

        var leaderboardManager = new LeaderboardManager(data);
        builder.RegisterInstance(leaderboardManager);

        var stateMachine = new GameStateMachine();
        var lobbyState = new LobbyState(_playButton, leaderboardManager, _leaderboardContainer);

        builder.RegisterInstance(stateMachine).AsSelf();
        builder.RegisterInstance(lobbyState).As<IGameState>();

        stateMachine.SetState(lobbyState).Forget();

        Debug.Log(stateMachine.CurrentState.ToString());
    }
}
