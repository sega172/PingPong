using TMPro;
using UnityEngine;
using YG;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BestScoreSign : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = YG2.saves.record.ToString();
    }
}
