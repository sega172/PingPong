using System;
using YG;

public static class GameSession
{
    public static event Action<GameMode> OnGamemodeChanged;

    public static GameMode GameMode {  get; private set; }
    public static YG2.Device Device {  get; set; }

    public static bool FirstTime = true;

    public static void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
        OnGamemodeChanged?.Invoke(gameMode);
    }
}

namespace YG
{
    public partial class SavesYG
    {
        public int record = 0;
        public float musicVolume = 1f;
        public float soundVolume = 1f;
        public string lang = "ru";
    }
}