using UnityEngine;
using YG;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] SettingsApplier settingsApplier;

    private void Awake() => InitializeGame();


    public void InitializeGame()
    {
        GameSession.Device = YG2.envir.device;
        GameSession.FirstTime = YG2.isFirstGameSession;

        settingsApplier.Init();

        if (GameSession.FirstTime)
            FirtTimeInitialization();
        else
            NormalInitialization();
    }

    private void FirtTimeInitialization()
    {
        settingsApplier.SetLanguage(YG2.lang/*.language*/);
        settingsApplier.SetVolume("Music", 0.75f);
        settingsApplier.SetVolume("Sounds", 0.75f);

        GameSession.FirstTime = false;
    }

    private void NormalInitialization()
    {
        settingsApplier.SetLanguage(YG2.saves.lang);
        settingsApplier.SetVolume("Music", YG2.saves.musicVolume);
        settingsApplier.SetVolume("Sounds", YG2.saves.soundVolume);
    }
}
