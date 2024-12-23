using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LeadeboardData
{
    public int GameNumber;
    public int Points;
}

public class LeaderboardManager
{
    private const string SavePath = "/leaderboard.json";
    private readonly IDataService _dataService;
    private List<LeadeboardData> _leaderboard = new List<LeadeboardData>();

    public LeaderboardManager(IDataService dataService)
    {
        _dataService = dataService;
        LoadLeaderboard();
    }

    public void AddGameResult(int gameNumber, int points)
    {
        var gameData = new LeadeboardData { GameNumber = gameNumber, Points = points };
        _leaderboard.Add(gameData);

        /* I commented out this line because I decided to take an already sorted leaderboard when entering LobbyState */
        //_leaderboard = GetSortedLeaderboardByDescending(); 

        // Keep only top 10 and delete game with lowest score
        if (_leaderboard.Count > 10)
        {
            var gameWithLowestScore = _leaderboard.OrderBy(data => data.Points).First(); // Find the game with the lowest score

            _leaderboard.Remove(gameWithLowestScore);
        }

        SaveLeaderboard();
    }

    public int GetNextGameNumber()
    {
        if (_leaderboard.Count == 0)
        {
            return 1; // First game
        }

        // Get the highest game number and increment it
        return _leaderboard.Max(data => data.GameNumber) + 1;
    }

    public List<LeadeboardData> GetSortedLeaderboardByDescending() => _leaderboard.OrderByDescending(data => data.Points).ToList();

    public List<LeadeboardData> GetLeaderboard() => _leaderboard;

    private void SaveLeaderboard()
    {
        _dataService.SaveData(SavePath, _leaderboard);
    }

    private void LoadLeaderboard()
    {
        try
        {
            _leaderboard = _dataService.LoadData<List<LeadeboardData>>(SavePath) ?? new List<LeadeboardData>();
        }
        catch
        {
            _leaderboard = new List<LeadeboardData>();
        }
    }

    public void DeleteLeaderboardData()
    {
        _dataService.DeleteData<List<LeadeboardData>>(SavePath);
        _leaderboard.Clear();
    }
}
