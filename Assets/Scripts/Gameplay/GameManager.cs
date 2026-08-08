using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<bool> OnSetControls;

    [SerializeField] private PlayerType player1Type;
    [SerializeField] private PlayerType player2Type;

    [SerializeField] private Paddle paddle1;
    [SerializeField] private Paddle paddle2;

    [SerializeField] Ball ball;

    [SerializeField] private TextMeshProUGUI backCountLabel;

    [Header("Sound")]
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip backCountClip;
    [SerializeField] AudioClip startClip;

    [SerializeField] AudioSource music;

    private void Awake()
    {
        Instance = this;
        music.Play();
        music.volume = 0;

        StartGame();
    }

    private void StartGame()
    {
        InitializePaddles();
        Restart();
    }

    void Music(bool enable, float duration)
    {
        float volume = enable ? 0.5f : 0;
        music.DOFade(volume, duration);
    }

    private void InitializePaddles()
    {

        paddle1.Init(Team.Player1, isBot: player1Type == PlayerType.Bot);
        paddle2.Init(Team.Player2, isBot: player2Type == PlayerType.Bot);
    }

    public void Restart()
    {
        ball.Disable();
        DisableInputs();
        Music(false, 1);

        float ballScale = ball.transform.localScale.x;
        ball.transform.localScale = Vector3.zero;
        ball.transform.position = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1);

        seq.Insert(1, paddle1.transform.DOMoveY(0, 1).SetEase(Ease.OutBack));
        seq.Insert(1, paddle2.transform.DOMoveY(0, 1).SetEase(Ease.OutBack));
        seq.Insert(1, BackCount());

        float backCountDuration = 3.4f;
        seq.Insert(backCountDuration-1, ball.transform.DOScale(ballScale, 1).SetEase(Ease.OutElastic));
        seq.InsertCallback(backCountDuration, () =>
        {
            EnableInputs();
            print("Inputs en");
            ball.Enable();
            
            
        });

        seq.PlayForward();
    }

    public Sequence BackCount()
    {
        Sequence seq = DOTween.Sequence();

        backCountLabel.transform.localScale = Vector3.zero;

        float step = 0.3f;

        seq.AppendCallback(() => backCountLabel.text = "3...");
        seq.AppendCallback(() => source.PlayOneShot(backCountClip));
        seq.Append(backCountLabel.transform.DOScale(1, step).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, step).SetEase(Ease.InBack));

        seq.AppendCallback(() => backCountLabel.text = "2...");
        seq.AppendCallback(() => source.PlayOneShot(backCountClip));
        seq.Append(backCountLabel.transform.DOScale(1, step).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, step).SetEase(Ease.InBack));
        
        seq.AppendCallback(() => backCountLabel.text = "1...");
        seq.AppendCallback(() => source.PlayOneShot(backCountClip));
        seq.Append(backCountLabel.transform.DOScale(1, step).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, step).SetEase(Ease.InBack));

        seq.AppendCallback(() => backCountLabel.text = "GO!");
        seq.AppendCallback(() => Music(true, 0.5f));
        seq.AppendCallback(() => source.PlayOneShot(startClip)); 
        seq.Append(backCountLabel.transform.DOScale(1, 0.6f).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, 2f).SetEase(Ease.InBack));

        return seq;
    }


    private void DisableInputs() => OnSetControls?.Invoke(false);

    private void EnableInputs() => OnSetControls?.Invoke(true);
}
