using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    public SoundType soundType;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(ApplyVolume);
        YG2.onGetSDKData += SyncSliderValue;
        SyncSliderValue();
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(ApplyVolume);
        YG2.onGetSDKData -= SyncSliderValue;
    }

    private void SyncSliderValue()
    {
        if (_slider != null)
            _slider.SetValueWithoutNotify(soundType == SoundType.Music ? YG2.saves.musicVolume : YG2.saves.soundVolume);
    }

    private void ApplyVolume(float value) => SettingsManager.Instance.SetVolume(soundType, value);
}