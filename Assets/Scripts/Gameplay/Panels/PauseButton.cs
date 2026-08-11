using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SetPause);
    }    

    private void SetPause()
    {
        GameManager.Instance.Pause();
    }
}
