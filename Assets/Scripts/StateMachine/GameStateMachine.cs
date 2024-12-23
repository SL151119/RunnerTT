using Cysharp.Threading.Tasks;

public interface IGameState
{
    UniTask EnterState();
    UniTask ExitState();
}

public class GameStateMachine
{
    private IGameState _currentState;
    public IGameState CurrentState => _currentState;
    public async UniTask SetState(IGameState newState)
    {
        if (_currentState != null)
        {
            await _currentState.ExitState();
        }

        _currentState = newState;

        if (_currentState != null)
        {
            await _currentState.EnterState();
        }
    }
}
