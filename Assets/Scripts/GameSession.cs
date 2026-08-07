using System;

public static class GameSession
{
    public static event Action<GameMode> OnGamemodeChanged;

    public static GameMode GameMode {  get; private set; }

    public static void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
        OnGamemodeChanged?.Invoke(gameMode);
    }
}