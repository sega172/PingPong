using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Button))]
public class MenuPlayButton : MonoBehaviour
{
    [SerializeField] private GameObject _instruction;
    private Button _button;

    public void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(EnterGame);
    }

    private void OnDisable() => _button.onClick.RemoveListener(EnterGame);

    private void OnDestroy() => _button.onClick.RemoveListener(EnterGame);

    private void EnterGame()
    {
        if (YG2.saves.learnCompleted)
            SceneManager.LoadScene("Game");
        else
            _instruction.SetActive(true);
    }
}
