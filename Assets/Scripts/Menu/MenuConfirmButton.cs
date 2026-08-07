using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuConfirmButton : MonoBehaviour
{
    private Button button;

    public void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(EnterGame);
    }

    private void OnDisable() => button.onClick.RemoveListener(EnterGame);

    private void OnDestroy() => button.onClick.RemoveListener(EnterGame);

    private void EnterGame()
    {
        print($"Заход в игру. Режим: {GameSession.GameMode}");
    }
}
