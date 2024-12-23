#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LeaderboardUtility : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("JsonData/Delete leaderboard data")]
    public static void DeleteLeaderboardData()
    {
        IDataService dataService = new JsonDataService();

        var leaderboardManager = new LeaderboardManager(dataService);

        leaderboardManager.DeleteLeaderboardData();

        Debug.Log("Leaderboard data deleted.");
    }
#endif
}
