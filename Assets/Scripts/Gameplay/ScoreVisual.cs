using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreVisual : MonoBehaviour
{
    private TextMeshProUGUI _label;

    private void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();
        DisplayScore(0);
        ScoreManager.OnScoreChanged += DisplayScore;
    }

    private void OnDestroy() 
        => ScoreManager.OnScoreChanged -= DisplayScore;

    private void DisplayScore(int newScore) 
        => _label.text = newScore.ToString();
}