using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    public SoundType soundType;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(ApplyVolume);
    }

    private void OnDestroy() => _slider.onValueChanged.RemoveAllListeners();

    private void ApplyVolume(float value) => SettingsManager.Instance.SetVolume(soundType, value);
}