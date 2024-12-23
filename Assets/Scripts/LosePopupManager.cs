using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePopupManager : MonoBehaviour
{
    [SerializeField] private Button _returnToLobbyButton;

    private void OnEnable()
    {
        _returnToLobbyButton.onClick?.AddListener(OnReturnToLobby);
    }

    private void OnDisable()
    {
        _returnToLobbyButton.onClick?.RemoveListener(OnReturnToLobby);
    }

    private async void OnReturnToLobby()
    {
        Debug.Log("Returning to Lobby...");

        await SceneLoader.UnloadSceneAsync(SceneIndex.LosePopup);

        await SceneLoader.LoadSceneAsync(SceneIndex.Lobby, LoadSceneMode.Single);
    }
}
