using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SetPause);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(SetPause);
    }

    private void SetPause()
    {
        GameManager.Instance.Pause();
    }
}
