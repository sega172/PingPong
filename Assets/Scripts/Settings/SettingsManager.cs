using System;
using UnityEngine;
using UnityEngine.Audio;
using YG;

public class SettingsManager : MonoBehaviour
{
    public static event Action OnSettingsSaved;
    public static SettingsManager Instance;

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private float _saveInterval = 2f;

    private bool _isDirty = false;
    private float _elapsed = 0;

    private void Update()
    {
        if (_isDirty == false) return;

        _elapsed += Time.deltaTime;

        bool shouldSave = _elapsed > _saveInterval;
        if (shouldSave)
            Save();
    }

    public void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLanguage(string value)
    {
        YG2.saves.lang = value;
        YG2.SwitchLanguage(value);
        TriggerSave();
    }

    public void SetVolume(SoundType soundType, float value)
    {
        if (soundType == SoundType.Music)
            SetMusicVolume(value);
        else
            SetVfxVolume(value);

        TriggerSave();
    }

    private void SetMusicVolume(float value)
    {
        _mixer.SetFloat("Music", ConvertLinearToDecibels(value));
        YG2.saves.musicVolume = value;
    }

    private void SetVfxVolume(float value)
    {
        _mixer.SetFloat("Sounds", ConvertLinearToDecibels(value));
        YG2.saves.soundVolume = value;
    }

    private static float ConvertLinearToDecibels(float linear)
    {
        if (linear <= 0.0001f)
            return -80f;

        return Mathf.Log10(linear) * 20f;
    }

    private void TriggerSave()
    {
        _isDirty = true;
        _elapsed = 0;
    }

    private void Save()
    {
        _elapsed = 0;
        _isDirty = false;
        YG2.SaveProgress();

        OnSettingsSaved?.Invoke();
    }
}