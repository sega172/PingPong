using System;
using UnityEngine;
using UnityEngine.Audio;
using YG;

public class SettingsApplier : MonoBehaviour
{
    public static SettingsApplier Instance;

    public static event Action OnSettingsChanged;
    public static event Action OnSettingsSaved;

    public AudioMixer mixer;

    public bool saved = true;
    public float saveSettingsCuldown = 2f;
    public float elapsed = 0;

    private void Update()
    {
        if (saved) return;

        elapsed += Time.deltaTime;

        if (elapsed > saveSettingsCuldown)
        {
            elapsed = 0;
            saved = true;
            Save();
            OnSettingsSaved?.Invoke();
        }
    }

    public void Init()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetVolume(string key, float value)
    {
        if(key== "Music")
            SetMusicVolume(value);
        else
            SetSoundsVolume(value);
        saved = false;
    }

    private void SetMusicVolume(float value)
    {
        mixer.SetFloat("Music", ConvertLinearToDecibels(value));
        YG2.saves.musicVolume = value;
    }

    private void SetSoundsVolume(float value)
    {
        mixer.SetFloat("Sounds", ConvertLinearToDecibels(value));
        YG2.saves.soundVolume = value;
    }

    public void SetLanguage(string value)
    {
        YG2.saves.lang = value;
        YG2.SwitchLanguage(value);
        saved = false;
    }

    private static float ConvertLinearToDecibels(float linear)
    {
        if (linear <= 0.0001f)
            return -80f;

        return Mathf.Log10(linear) * 20f;
    }

    private static void Save() => YG2.SaveProgress();
}
