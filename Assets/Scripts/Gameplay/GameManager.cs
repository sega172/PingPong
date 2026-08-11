using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

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

    public static GameManager Instance { get; private set; }
    public static PlayerHealth PlayerHealth { get; private set; }

    private void Awake()
    {
        Instance = this;
        _musicSource.Play();
        _musicSource.volume = 0;
        
        PlayerHealth = new PlayerHealth(initialHealth: 3);

        StartGame();
    }

    private void StartGame()
    {
        InitializePaddles();
        Restart();
    }

    public void Restart()
    {
        _ball.Disable();
        DisableInputs();
        FadeMusic(false, 1);
                
        _ball.transform.localScale = Vector3.zero;
        _ball.transform.position = Vector3.zero;

        GetRestartAnimation().PlayForward();
    }

    private Sequence GetRestartAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(heartsPanel.SetHearts(PlayerHealth.Health));
        
        seq.AppendInterval(1);

        seq.Insert(1, _paddle1.transform.DOMoveY(0, 1).SetEase(Ease.OutBack));
        seq.Insert(1, _paddle2.transform.DOMoveY(0, 1).SetEase(Ease.OutBack));
        seq.Insert(1, GetBackCountSequence());

        float backCountDuration = 3.4f;
        seq.Insert(backCountDuration - 1, _ball.transform.DOScale(_ball.normalScale, 1).SetEase(Ease.OutElastic));
        seq.InsertCallback(backCountDuration, () =>
        {
            EnableInputs();
            _ball.Enable();
        });
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
}