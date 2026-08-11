using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private float _startY;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private RectTransform _panel;
    [SerializeField] private List<RectTransform> _buttons;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;

    [SerializeField] private TextMeshProUGUI _scoreLabel;
    [SerializeField] private TextMeshProUGUI _bestScoreLabel;

    [SerializeField] private RectTransform _newBestSign;

    private Sequence _sequence;
    private int _score;
    private int _bestScore;
    private bool _newBest = false;

    private void OnEnable()
    {
        _group.alpha = 0;
        _group.interactable = false;
        _panel.anchoredPosition = new Vector2(0, _startY);
        _newBestSign.localScale = Vector3.zero;

        foreach (var button in _buttons)
            button.localScale = Vector3.zero;

        _restartButton.onClick.AddListener(OnRestart);
        _menuButton.onClick.AddListener(OnMenu);

        //scores
        _score = ScoreManager.Score;
        _bestScore = YG2.saves.record;
        if(_score > _bestScore)
        {
            _bestScore = _score;
            YG2.saves.record = _bestScore;
            YG2.SaveProgress();
            _newBest = true;
        }

        _scoreLabel.text = _score.ToString();
        _bestScoreLabel.text = _bestScore.ToString();

        OpenAnimation();
    }

    private void OnDisable()
    {
        _restartButton.onClick.RemoveAllListeners();
        _menuButton.onClick.RemoveAllListeners();
    }

    private void OnRestart()
    {
        YG2.InterstitialAdvShow();
        CloseAnimation(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    private void OnMenu()
    {
        YG2.InterstitialAdvShow();
        CloseAnimation(() => SceneManager.LoadScene("Menu"));
    }

    private void OpenAnimation()
    {
        if (_sequence != null)
            _sequence.Kill(complete: true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOAnchorPosY(0, 1).SetEase(Ease.OutBack));
        _sequence.Join(_group.DOFade(1, 1).SetEase(Ease.Linear));

        if (_newBest)
            _sequence.Append(_newBestSign.DOScale(1, 1).SetEase(Ease.OutElastic));

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
