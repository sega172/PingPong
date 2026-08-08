using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreVisual : MonoBehaviour
{
    TextMeshProUGUI label;
    private void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        ScoreChanged(0);
        ScoreManager.OnScoreChanged+= ScoreChanged;
    }

    private void ScoreChanged(int newScore)
    {
        label.text = $"<color=red>{newScore}</color>";
    }

    private void OnDestroy()
    {
        ScoreManager.OnScoreChanged -= ScoreChanged;
    }
}