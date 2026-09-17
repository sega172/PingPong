using TMPro;
using UnityEngine;
using YG;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BestScoreSign : MonoBehaviour
{
    private TextMeshProUGUI _label;

    private void Awake()
    {
        _label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        YG2.onGetSDKData += UpdateScore;
        UpdateScore();
    }

    private void OnDisable()
    {
        YG2.onGetSDKData -= UpdateScore;
    }

    private void UpdateScore()
    {
        if (_label != null)
            _label.text = YG2.saves.record.ToString();
    }
}
