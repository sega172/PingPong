using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private float _startY;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private RectTransform _panel;
    [SerializeField] private List<RectTransform> _buttons;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _menuButton;

    private Sequence _sequence;

    private void OnEnable()
    {
        _group.alpha = 0;
        _group.interactable = false;
        _panel.anchoredPosition = new Vector2(0, _startY);

        foreach (var button in _buttons)
            button.localScale = Vector3.zero;

        _playButton.onClick.AddListener(OnPlay);
        _menuButton.onClick.AddListener(OnMenu);

        Time.timeScale = 0;
        OpenAnimation();
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveAllListeners();
        _menuButton.onClick.RemoveAllListeners();
    }

    private void OnPlay()
    {
        YG2.InterstitialAdvShow();
        CloseAnimation(() => Time.timeScale = 1);
    }

    private void OnMenu()
    {
        if(ScoreManager.Score > YG2.saves.record)
        {
            YG2.saves.record = ScoreManager.Score;
            YG2.SaveProgress();
        }

        CloseAnimation(() => SceneManager.LoadScene("Menu"));
    }

    private void OpenAnimation()
    {
        if (_sequence != null)
            _sequence.Kill(complete: true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack));
        _sequence.Join(_group.DOFade(1, 0.5f).SetEase(Ease.Linear));

        foreach (var button in _buttons)
            _sequence.Insert(1, button.DOScale(0.5f, 0.5f).SetEase(Ease.OutBack));

        _sequence.AppendCallback(() => _group.interactable = true);

        _sequence.SetUpdate(true);
        _sequence.PlayForward();
    }

    private void CloseAnimation(Action callback)
    {
        _group.interactable = false;

        if (_sequence != null)
            _sequence.Kill(complete: true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOAnchorPosY(_startY, 0.5f).SetEase(Ease.InBack));
        _sequence.Join(_group.DOFade(0, 0.5f));
        _sequence.onComplete += () =>
        {
            callback?.Invoke();
            gameObject.SetActive(false);
        };
        _sequence.SetUpdate(true);
        _sequence.PlayForward();
    }
}
