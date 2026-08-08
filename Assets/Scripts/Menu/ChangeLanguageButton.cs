using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Button))]
public class ChangeLanguageButton : MonoBehaviour
{
    [SerializeField] string language;

    [SerializeField] Image frame;
    [SerializeField] Color selectedColor;
    [SerializeField] Color normalColor;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetMyLanguage);
        YG2.onSwitchLang += UpdateColor;
        UpdateColor(YG2.lang);
    }

    private void OnDisable() => Unsubscribe();


    private void OnDestroy() => Unsubscribe();

    private void Unsubscribe()
    {
        button.onClick.RemoveListener(SetMyLanguage);
        YG2.onSwitchLang -= UpdateColor;
    }

    private void UpdateColor(string newLang)
    {
        Color color = newLang == language ? selectedColor : normalColor;
        frame.color = color;
    }

    private void SetMyLanguage() => SettingsApplier.Instance.SetLanguage(language);
}