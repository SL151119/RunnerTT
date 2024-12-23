using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyState : IGameState
{
    private readonly Button _playButton;
    private readonly LeaderboardManager _leaderboardManager;
    private readonly Transform _leaderboardContainer;

    public LobbyState(Button playButton, LeaderboardManager leaderboardManager, Transform leaderboardContainer)
    {
        _playButton = playButton;
        _leaderboardManager = leaderboardManager;
        _leaderboardContainer = leaderboardContainer;
    }

    public UniTask EnterState()
    {
        _playButton.onClick?.AddListener(OnPlayButtonClicked);

        UpdateLeaderboardUI();

        return UniTask.CompletedTask;
    }

    public UniTask ExitState()
    {
        _playButton.onClick?.RemoveListener(OnPlayButtonClicked);

        return UniTask.CompletedTask;
    }

    private void UpdateLeaderboardUI()
    {
        var leaderboard = _leaderboardManager.GetSortedLeaderboardByDescending();

        //Disable all unnecessary fields
        foreach (Transform child in _leaderboardContainer)
        {
            child.gameObject.SetActive(false);
        }

        // Update child objects with leaderboard data
        for (int i = 0; i < leaderboard.Count && i < _leaderboardContainer.childCount; i++)
        {
            var ui = _leaderboardContainer.GetChild(i).GetComponent<LeaderboardUI>();
            ui.SetData(leaderboard[i].GameNumber, leaderboard[i].Points);

            _leaderboardContainer.GetChild(i).gameObject.SetActive(true);
        }
    }

    private async void OnPlayButtonClicked()
    {
        await SceneLoader.LoadSceneAsync(SceneIndex.Gameplay, LoadSceneMode.Single);
    }
}
