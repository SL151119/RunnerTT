using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class EnergyUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _totalScoreText;
    [SerializeField] private List<EnergyUIConfig> _energyUIConfigs;

    private Dictionary<EnergyType, EnergyUIElement> energyUIElements;
    private readonly Dictionary<EnergyType, int> _energyScores = new();

    private ScoreManager _scoreManager;

    [Inject]
    public void Initialize(ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
    }

    private void Awake()
    {
        energyUIElements = new Dictionary<EnergyType, EnergyUIElement>();

        foreach (var config in _energyUIConfigs)
        {
            energyUIElements[config.Type] = config.UIElement;
            _energyScores[config.Type] = 0;
        }

        UpdateUI();
    }

    public void AddEnergyScore(EnergyType type, int points)
    {
        if (!_energyScores.ContainsKey(type))
        {
            return;
        }

        _energyScores[type] += points;

        Debug.Log($"Added {points} points to {type}. Current score for {type}: {_energyScores[type]}.");

        _scoreManager.AddPoints(points);

        UpdateUI();
    }

    private void UpdateUI()
    {
        _totalScoreText.text = $"Total Score:\n {_scoreManager.TotalScore}";

        foreach (var type in _energyScores.Keys)
        {
            if (!energyUIElements.TryGetValue(type, out var uiElement))
            {
                continue;
            }

            uiElement.UpdateScore(_energyScores[type]);
        }
    }

    [System.Serializable]
    public class EnergyUIElement
    {
        public Image icon;
        public TextMeshProUGUI scoreText;

        public void UpdateScore(int score)
        {
            scoreText.text = $": {score}";
        }
    }

    [System.Serializable]
    public class EnergyUIConfig
    {
        public EnergyType Type;
        public EnergyUIElement UIElement;
    }
}

