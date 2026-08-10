using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Button))]
public class ChangeLanguageButton : MonoBehaviour
{
    [SerializeField] string _language;
    [SerializeField] Image _frame;
    [SerializeField] Color _selectedColor;
    [SerializeField] Color _normalColor;

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
        => _frame.color = newLang == _language ? _selectedColor : _normalColor;

    private void SetMyLanguage() 
        => SettingsManager.Instance.SetLanguage(_language);
}