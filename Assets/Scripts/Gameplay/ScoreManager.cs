using System;

public static class ScoreManager
{
    public static event Action<int> OnScoreChanged;

    public static int Score { get; private set; }

    public static void Initialize()
        => ResetScore();

    public static void AddPoint()
    {
        Score++;
        OnScoreChanged?.Invoke(Score);
    }

    public static void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }
}