
public class ScoreManager
{
    private int _totalScore;

    public int TotalScore => _totalScore;

    public void AddPoints(int points)
    {
        _totalScore += points;
    }

    public void ResetScore()
    {
        _totalScore = 0;
    }
}
