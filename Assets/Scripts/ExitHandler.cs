using UnityEngine;
using VContainer;

public class ExitHandler : MonoBehaviour
{
    private PlayerInput _playerInput;

    [Inject]
    public void Initialize(PlayerInput playerInput)
    {
        _playerInput = playerInput;
        _playerInput.OnExit += HandleExit;
    }

    private void OnDestroy()
    {
        _playerInput.OnExit -= HandleExit;
    }

    private void HandleExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE
        Application.Quit();
#endif
    }
}