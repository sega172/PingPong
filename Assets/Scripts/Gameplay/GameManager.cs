using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    public static event Action<bool> OnSetControls;

    [SerializeField] private Paddle _paddle1;
    [SerializeField] private Paddle _paddle2;
    [SerializeField] private Ball _ball;
    [SerializeField] private TextMeshProUGUI _backCountLabel;

    [Header("Sound")]
    [SerializeField] AudioSource _musicSource;
    [SerializeField] AudioSource _vfxSource;
    [SerializeField] AudioClip _backCountClip;
    [SerializeField] AudioClip _startClip;

    //
    [SerializeField] private HeartsPanel heartsPanel;
    [SerializeField] GetLifePanel _getLife;
    [SerializeField] GameOverPanel _gameOverPanel;
    [SerializeField] PausePanel _pausePanel;

    public Transform UpWallPoint;
    public Transform DownWallPoint;

    [SerializeField] List<Goal> _goals;

    public static GameManager Instance { get; private set; }
    public static PlayerHealth PlayerHealth { get; private set; }

    private void Awake()
    {
        Instance = this;
        _musicSource.Play();
        _musicSource.volume = 0;

        PlayerHealth = new PlayerHealth(initialHealth: 3);

        if (_goals != null)
            foreach (Goal goal in _goals)
                goal.OnGoal += OnGoal;

        YG2.onRewardAdv += OnReward;
        YG2.onErrorRewardedAdv += GameOver;

        StartGame();
    }

    private void OnDestroy()
    {
        if (_goals != null)
            foreach (Goal goal in _goals)
                goal.OnGoal -= OnGoal;
        YG2.onRewardAdv -= OnReward;

        YG2.onErrorRewardedAdv -= GameOver;
        //YG2.onCloseRewardedAdv -= GameOver;
    }

    private void OnGoal(Team team)
    {
        _ball.GoalParticles();
        StopGame();

        Team winner = team == Team.Player ? Team.Bot : Team.Player;
        if (winner == Team.Player)
            ScoreManager.AddPoint();
        else if (winner == Team.Bot)
            PlayerHealth.RemoveHealth(1);

        if (PlayerHealth.Health < 1)
        {
            GetPrepareAnimation().PlayForward();
            OfferAds();
        }
        else
        {
            PrepareAndStart();
        }
    }

    public void OfferAds()
    {
        _getLife.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        _gameOverPanel.gameObject.SetActive(true);
    }

    private void StartGame()
    {
        InitializePaddles();
        PrepareAndStart();
    }

    public void StopGame()
    {
        _ball.Disable();
        DisableInputs();
        FadeMusic(false, 1);

        _ball.transform.localScale = Vector3.zero;
        _ball.transform.position = Vector3.zero;
    }

    public void Pause()
    {
        _pausePanel.gameObject.SetActive(true);
    }

    public void PrepareAndStart()
    {
        StopGame();

        if (PlayerHealth.Health < 1)
        {
            GameOver();
            return;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.Append(GetPrepareAnimation());
        sequence.Append(GetStartAnimation(startGameCallback: StartGame1));
        sequence.PlayForward();
    }

    private Sequence GetStartAnimation(Action startGameCallback)
    {
        Sequence seq = DOTween.Sequence();
        seq.Insert(1, GetBackCountSequence());
        float backCountDuration = 3.4f;
        seq.Insert(backCountDuration - 1, _ball.transform.DOScale(_ball.normalScale, 1).SetEase(Ease.OutElastic));
        seq.InsertCallback(backCountDuration, StartGame1);
        return seq;
    }

    private void StartGame1()
    {
        EnableInputs();
        _ball.Enable();
    }

    private Sequence GetPrepareAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(heartsPanel.SetHearts(PlayerHealth.Health));
        seq.AppendInterval(1);
        seq.Insert(1, _paddle1.transform.DOMoveY(0, 1).SetEase(Ease.OutBack));
        seq.Insert(1, _paddle2.transform.DOMoveY(0, 1).SetEase(Ease.OutBack));

        return seq;
    }

    private Sequence GetBackCountSequence()
    {
        Sequence seq = DOTween.Sequence();

        _backCountLabel.transform.localScale = Vector3.zero;

        float step = 0.3f;

        seq.AppendCallback(() => _backCountLabel.text = "3...");
        seq.AppendCallback(() => _vfxSource.PlayOneShot(_backCountClip));
        seq.Append(_backCountLabel.transform.DOScale(1, step).SetEase(Ease.OutBack));
        seq.Append(_backCountLabel.transform.DOScale(0, step).SetEase(Ease.InBack));

        seq.AppendCallback(() => _backCountLabel.text = "2...");
        seq.AppendCallback(() => _vfxSource.PlayOneShot(_backCountClip));
        seq.Append(_backCountLabel.transform.DOScale(1, step).SetEase(Ease.OutBack));
        seq.Append(_backCountLabel.transform.DOScale(0, step).SetEase(Ease.InBack));

        seq.AppendCallback(() => _backCountLabel.text = "1...");
        seq.AppendCallback(() => _vfxSource.PlayOneShot(_backCountClip));
        seq.Append(_backCountLabel.transform.DOScale(1, step).SetEase(Ease.OutBack));
        seq.Append(_backCountLabel.transform.DOScale(0, step).SetEase(Ease.InBack));

        seq.AppendCallback(() => _backCountLabel.text = "GO!");
        seq.AppendCallback(() => FadeMusic(true, 0.5f));
        seq.AppendCallback(() => _vfxSource.PlayOneShot(_startClip));
        seq.Append(_backCountLabel.transform.DOScale(1, 0.6f).SetEase(Ease.OutBack));
        seq.Append(_backCountLabel.transform.DOScale(0, 2f).SetEase(Ease.InBack));

        return seq;
    }

    private void FadeMusic(bool enable, float duration)
    {
        float volume = enable ? 0.5f : 0;
        _musicSource.DOFade(volume, duration);
    }

    private void InitializePaddles()
    {
        _paddle1.Init(Team.Player, isBot: false);
        _paddle2.Init(Team.Bot, isBot: true);
    }

    private void DisableInputs() => OnSetControls?.Invoke(false);

    private void EnableInputs() => OnSetControls?.Invoke(true);

    private void OnReward(string id)
    {
        PlayerHealth.AddHealth(1);
        PrepareAndStart();
    }
}