using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Slider))]
public class MySlider : MonoBehaviour
{
    public string VolumeKey = "Music";
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(ApplyVolume);

        if (VolumeKey == "Music")
            slider.value = YG2.saves.musicVolume;
        else
            slider.value = YG2.saves.soundVolume;

    }

    private void ApplyVolume(float value)
    {
        SettingsApplier.Instance.SetVolume(VolumeKey, value);
    }


}
