using UnityEngine;
using YG;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private SettingsManager _settingsManager;
    [SerializeField] private Ball _ball;
    private void Awake()
    {
        if(Config.Initialized == false)
            InitializeGame();

        _ball.Init();
        _ball.Enable();

        print("Awake");
    }

    public void InitializeGame()
    {
        Config.FirstTime = YG2.isFirstGameSession;

        if (string.IsNullOrEmpty(YG2.saves.lang))
            YG2.saves.lang = YG2.lang;

        _settingsManager.Init();

        YG2.InterstitialAdvShow();

        if (Config.FirstTime)
            FirtTimeInitialization();
        else
            NormalInitialization();

        Config.Initialized = true;
    }

    private void FirtTimeInitialization()
    {
        _settingsManager.SetLanguage(YG2.saves.lang);
        _settingsManager.SetVolume(SoundType.Music, 0.75f);
        _settingsManager.SetVolume(SoundType.VFX, 0.75f);

        Config.FirstTime = false;
    }

    private void NormalInitialization()
    {
        _settingsManager.SetLanguage(YG2.saves.lang);
        _settingsManager.SetVolume(SoundType.Music, YG2.saves.musicVolume);
        _settingsManager.SetVolume(SoundType.VFX, YG2.saves.soundVolume);
    }
}