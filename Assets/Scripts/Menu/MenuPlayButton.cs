using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuPlayButton : MonoBehaviour
{
    private Button _button;

    public void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(EnterGame);
    }

    private void OnDisable() => _button.onClick.RemoveListener(EnterGame);

    private void OnDestroy() => _button.onClick.RemoveListener(EnterGame);

    private void EnterGame() => SceneManager.LoadScene("Game");
}
