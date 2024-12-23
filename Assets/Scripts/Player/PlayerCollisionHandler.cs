using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

[RequireComponent(typeof(PlayerCollider))] 
public class PlayerCollisionHandler : MonoBehaviour, ICollisionHandler
{
    private PlayerDeathHandler _deathHandler;
    private TrackManager _trackManager;
    private ScoreManager _scoreManager;
    private LeaderboardManager _leaderboardManager;

    private int _gameNumber = 0;

    [Inject]
    public void Initialize(PlayerDeathHandler deathHandler, TrackManager trackManager, ScoreManager scoreManager, LeaderboardManager leaderboardManager)
    {
        _deathHandler = deathHandler;
        _trackManager = trackManager;
        _scoreManager = scoreManager;
        _leaderboardManager = leaderboardManager;

        _gameNumber = _leaderboardManager.GetNextGameNumber();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    public void HandleCollision(Collider other)
    {
        if (other.TryGetComponent<ICollectable>(out var collectable))
        {
            collectable.Collect();
        }

        if (other.TryGetComponent<Obstacle>(out var obstacle))
        {
            _deathHandler.TriggerDeathAnimation();

            // Save the game result
            _leaderboardManager.AddGameResult(_gameNumber, _scoreManager.TotalScore);

            _scoreManager.ResetScore();

            _trackManager.End();

            SceneLoader.LoadSceneAsync(SceneIndex.LosePopup, LoadSceneMode.Additive).Forget();
        }
    }
}
