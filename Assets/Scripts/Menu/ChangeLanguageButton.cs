using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChangeLanguageButton : MonoBehaviour
{
    [SerializeField] string language;

    private Button button;

    private void OnEnable()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(SetMyLanguage);
    }

    private void OnDisable() => button.onClick.RemoveListener(SetMyLanguage);

    private void OnDestroy() => button.onClick.RemoveListener(SetMyLanguage);

    private void SetMyLanguage()
    {
        LanguageChanger.SetLanguage(language);
    }
}