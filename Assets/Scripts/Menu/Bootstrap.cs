using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] MenuManager menuManager;

    public string debugLang = "ru";

    private void Awake() => InitializeGame();

    private void InitializeGame()
    {
        LanguageChanger.SetLanguage(debugLang);

        menuManager.Initialize();
    }
}
