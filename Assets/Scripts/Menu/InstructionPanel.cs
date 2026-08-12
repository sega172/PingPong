using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class InstructionPanel : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _panels;
    [SerializeField] private Button _button;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _offsetY = 1200f;

    private float[] _cachedPanelPositions;

    private void OnEnable()
    {
        Init();
        GetOpenSequence().PlayForward();
    }

    private void Init()
    {
        _cachedPanelPositions = new float[_panels.Count];

        for (int i = 0; i < _panels.Count; i++)
        {
            RectTransform panel = _panels[i];
            float startPanelPosition = panel.anchoredPosition.y;
            _cachedPanelPositions[i] = startPanelPosition;
            panel.anchoredPosition = new Vector2(0, startPanelPosition - _offsetY);
        }

        _button.transform.localScale = Vector3.zero;
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0;
        _button.onClick.AddListener(GotoGame);
    }

    Sequence GetOpenSequence()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_canvasGroup.DOFade(1, 1));

        for (int i = 0; i < _panels.Count; i++)
        {
            RectTransform panel = _panels[i];
            seq.Append(panel.DOAnchorPosY(_cachedPanelPositions[i], 1).SetEase(Ease.OutCubic));
        }

        seq.AppendInterval(1);
        seq.Append(_button.GetComponent<RectTransform>().DOScale(1, 1).SetEase(Ease.OutBack));
        seq.AppendCallback(() => _canvasGroup.interactable = true);

        return seq;
    }

    private void GotoGame()
    {
        YG2.saves.learnCompleted = true;
        SceneManager.LoadScene("Game");
    }
}