using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class LoadingLifetimeScope : LifetimeScope
{
    [SerializeField] private Slider _loadingSlider;

    protected override void Configure(IContainerBuilder builder)
    {
        RegisterStates(builder);
    }

    private void RegisterStates(IContainerBuilder builder)
    {
        var stateMachine = new GameStateMachine();
        var loadingState = new LoadingState(SceneIndex.Lobby, _loadingSlider);

        builder.RegisterInstance(stateMachine).AsSelf();
        builder.RegisterInstance(loadingState).As<IGameState>();

        stateMachine.SetState(loadingState).Forget();

        Debug.Log(stateMachine.CurrentState.ToString());
    }
}
