using System;

public static class ScoreManager
{
    public static event Action<int, int> ScorePvpChanged;
    public static event Action<int> ScorePveChanged;

    private static GameMode _gameMode;

    public static int Score1 { get; private set; }
    public static int Score2 { get; private set; }
    public static int ScoreVsBot { get; private set; }

    public static void Initialize(GameMode gameMode)
    {
        Reset();
        _gameMode = gameMode;
    }

    public static void AddPoint(Team team)
    {
        if(_gameMode == GameMode.PvP)
            AddPointPvp(team);
        else
            AddPointPve(team);
    }

    private static void AddPointPvp(Team team)
    {
        if (team == Team.Player1)
            Score1++;
        else //Player2
            Score2++;

        ScorePvpChanged?.Invoke(Score1, Score2);
    }

    private static void AddPointPve(Team team)
    {
        ScoreVsBot += team == Team.Player1 ? 1 : -1;
        ScorePveChanged?.Invoke(ScoreVsBot);
    }

    public static void Reset()
    {
        Score1 = 0;
        Score2 = 0;
        ScoreVsBot = 0;
    }
}
