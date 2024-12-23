using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameNumber;
    [SerializeField] private TextMeshProUGUI _totalScore;

    public void SetData(int gameNumber, int points)
    {
        _gameNumber.text = $"Game {gameNumber}";
        _totalScore.text = $"Points: {points}";
    }
}
