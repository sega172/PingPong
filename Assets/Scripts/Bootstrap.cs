using UnityEngine;
using YG;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private SettingsManager _settingsManager;

    private void Awake() => InitializeGame();

    public void InitializeGame()
    {
        Config.FirstTime = YG2.isFirstGameSession;
        _settingsManager.Init();

        YG2.InterstitialAdvShow();

        if (Config.FirstTime)
            FirtTimeInitialization();
        else
            NormalInitialization();
    }

    private void FirtTimeInitialization()
    {
        _settingsManager.SetLanguage(YG2.lang/*.language*/);
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