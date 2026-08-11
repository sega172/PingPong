using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class GetLifePanel : MonoBehaviour
{
    [SerializeField] private float _startY;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private RectTransform _panel;
    [SerializeField] private List<RectTransform> _buttons;

    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    private Sequence _sequence;

    private void OnEnable()
    {
        _group.alpha = 0;
        _group.interactable = false;
        _panel.anchoredPosition = new Vector2(0, _startY);

        foreach (var button in _buttons)
            button.localScale = Vector3.zero;

        _yesButton.onClick.AddListener(OnYes);
        _noButton.onClick.AddListener(OnNo);

        OpenAnimation();
    }

    private void OnDisable()
    {
        _yesButton.onClick.RemoveAllListeners();
        _noButton.onClick.RemoveAllListeners();
    }

    private void OnYes()
    {
        CloseAnimation(() => YG2.RewardedAdvShow("hp"));
    }

    private void OnNo()
    {
        CloseAnimation(() => GameManager.Instance.GameOver());
    }

    private void OpenAnimation()
    {
        if (_sequence != null)
            _sequence.Kill(complete: true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOAnchorPosY(0, 1).SetEase(Ease.OutBack));
        _sequence.Join(_group.DOFade(1, 1).SetEase(Ease.Linear));

        foreach (var button in _buttons)
            _sequence.Insert(1, button.DOScale(1, 0.5f).SetEase(Ease.OutBack));

        _sequence.AppendCallback(() => _group.interactable = true);

        _sequence.PlayForward();
    }

    private void CloseAnimation(Action callback)
    {
        _group.interactable = false;

        if (_sequence != null)
            _sequence.Kill(complete: true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOAnchorPosY(_startY, 1).SetEase(Ease.InBack));
        _sequence.Join(_group.DOFade(0, 1));
        _sequence.onComplete += () => 
        {
            callback?.Invoke();
            gameObject.SetActive(false);
        };

        _sequence.PlayForward();
    }
}