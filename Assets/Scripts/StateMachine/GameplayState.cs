using Cysharp.Threading.Tasks;
using VContainer;

public class GameplayState : IGameState
{
    private readonly TrackManager _trackManager;

    public GameplayState(TrackManager trackManager)
    {
        _trackManager = trackManager;
    }

    public UniTask EnterState()
    {
        _trackManager.StartMove();
        _trackManager.Begin();

        return UniTask.CompletedTask;
    }

    public UniTask ExitState()
    {
        return UniTask.CompletedTask;
    }
}
