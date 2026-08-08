using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StatusPanelTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatusPanel statusPanel;

    [Header("UI Controls")]
    [SerializeField] private TMP_InputField heartsInputField;
    [SerializeField] private Button setHeartsButton;
    [SerializeField] private Button showButton;
    [SerializeField] private Button hideButton;
    [SerializeField] private Button resetButton;

    [Header("Debug Info")]
    [SerializeField] private TextMeshProUGUI currentHeartsText;

    private void Start()
    {
        if (setHeartsButton != null)
            setHeartsButton.onClick.AddListener(OnSetHeartsClicked);

        if (showButton != null)
            showButton.onClick.AddListener(OnShowClicked);

        if (hideButton != null)
            hideButton.onClick.AddListener(OnHideClicked);

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);

        UpdateCurrentHeartsInfo();
    }

    private void OnSetHeartsClicked()
    {
        if (statusPanel == null)
        {
            Debug.LogError("StatusPanel не назначен!");
            return;
        }

        if (heartsInputField == null)
        {
            Debug.LogError("HeartsInputField не назначен!");
            return;
        }

        if (int.TryParse(heartsInputField.text, out int amount))
        {
            amount = Mathf.Clamp(amount, 0, 10);
            Debug.Log($"SetHearts: amount={amount}");

            Sequence seq = statusPanel.SetHearts(amount);
            seq.Play();

            UpdateCurrentHeartsInfo();
        }
        else
        {
            Debug.LogWarning($"Некорректное значение: {heartsInputField.text}");
        }
    }

    private void OnShowClicked()
    {
        if (statusPanel == null) return;
        Debug.Log("ShowStatus");
        statusPanel.ShowStatusSeq().PlayForward();
    }

    private void OnHideClicked()
    {
        if (statusPanel == null) return;
        Debug.Log("HideStatus");
        statusPanel.HideStatusSeq().PlayForward();
    }

    private void OnResetClicked()
    {
        if (statusPanel == null) return;
        Debug.Log("Reset - устанавливаем 3 сердца");

        Sequence hideSeq = statusPanel.HideStatusSeq();
        hideSeq.OnComplete(() =>
        {
            Sequence setSeq = statusPanel.SetHearts(3);
            setSeq.OnComplete(() =>
            {
                statusPanel.ShowStatusSeq().Play();
            });
            setSeq.Play();
        });
        hideSeq.Play();
    }

    private void UpdateCurrentHeartsInfo()
    {
        if (currentHeartsText != null && statusPanel != null)
        {
            currentHeartsText.text = $"Текущее количество: {GetHeartsCount()}";
        }
    }

    private int GetHeartsCount()
    {
        var field = typeof(StatusPanel).GetField("heartModels",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            var list = field.GetValue(statusPanel) as System.Collections.IList;
            return list != null ? list.Count : 0;
        }

        return 0;
    }

    private void OnDestroy()
    {
        if (setHeartsButton != null)
            setHeartsButton.onClick.RemoveListener(OnSetHeartsClicked);

        if (showButton != null)
            showButton.onClick.RemoveListener(OnShowClicked);

        if (hideButton != null)
            hideButton.onClick.RemoveListener(OnHideClicked);

        if (resetButton != null)
            resetButton.onClick.RemoveListener(OnResetClicked);
    }
}