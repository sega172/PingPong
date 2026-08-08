using System;

public static class ScoreManager
{
    public static event Action<int> OnScoreChanged;

    public static int Score { get; private set; }

    public static void Initialize()
    {
        ResetScore();
    }

    public static void AddPoint(Team team)
    {
        Score += team == Team.Player1 ? 1 : -1;
        OnScoreChanged?.Invoke(Score);
    }

    public static void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }
}