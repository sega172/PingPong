using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChangeGamemodeButton : MonoBehaviour
{
    [SerializeField] GameMode gameMode;
    private Button button;
    
    public void OnEnable()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetGamemode);
    }

    private void OnDisable() => button.onClick.RemoveListener(SetGamemode);

    private void OnDestroy() => button.onClick.RemoveListener(SetGamemode);

    private void SetGamemode()
    {
        GameSession.SetGameMode(gameMode);
    }
}
