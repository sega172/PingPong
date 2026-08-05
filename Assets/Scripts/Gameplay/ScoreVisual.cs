using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreVisual : MonoBehaviour
{
    TextMeshProUGUI label;
    private void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        ScoreChanged(0, 0);
        ScoreManager.ScorePvpChanged += ScoreChanged;
    }

    private void ScoreChanged(int score1, int score2)
    {
        label.text = $"<color=red>{score1}</color>\t<color=yellow>{score2}</color>";
    }

    private void OnDestroy()
    {
        ScoreManager.ScorePvpChanged -= ScoreChanged;
    }
}
