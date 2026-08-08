using YG;

public static class GameSession
{
    public static bool FirstTime = true;
    public static YG2.Device Device {  get; set; }   
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