using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerType player1Type;
    [SerializeField] private PlayerType player2Type;

    [SerializeField] private Paddle paddle1;
    [SerializeField] private Paddle paddle2;

    [SerializeField] Ball ball;

    private InputAction player1;
    private InputAction player2;
    private InputAction playerBoth;

    [SerializeField] private TextMeshProUGUI backCountLabel;

    private void Awake()
    {
        Instance = this;

        player1 = InputSystem.actions.FindAction("Player1");
        player2 = InputSystem.actions.FindAction("Player2");
        playerBoth = InputSystem.actions.FindAction("PlayerBoth");

        StartGame();
    }

    private void StartGame()
    {
        InitializePaddles();
        Restart();
    }

    private void InitializePaddles()
    {
        InputAction paddle1Action = null;
        InputAction paddle2Action = null;

        (paddle1Action, paddle2Action) = (player1Type, player2Type) switch
        {
            (PlayerType.Player, PlayerType.Player) => (player1, player2),

            (PlayerType.Player, PlayerType.Bot) => (playerBoth, null),
            (PlayerType.Bot, PlayerType.Player) => (null, playerBoth),

            (PlayerType.Bot, PlayerType.Bot) => (null, null),

            _ => (null, null)
        };

        paddle1.Init(Team.Player1, isBot: player1Type == PlayerType.Bot, paddle1Action);
        paddle2.Init(Team.Player2, isBot: player2Type == PlayerType.Bot, paddle2Action);
    }

    public void Restart()
    {
        ball.Disable();
        DisableInputs();

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
            ball.Enable();
        });

        seq.PlayForward();
    }

    public Sequence BackCount()
    {
        Sequence seq = DOTween.Sequence();

        backCountLabel.transform.localScale = Vector3.zero;

        seq.AppendCallback(() => backCountLabel.text = "3...");
        seq.Append(backCountLabel.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, 0.4f).SetEase(Ease.InBack));

        seq.AppendCallback(() => backCountLabel.text = "2...");
        seq.Append(backCountLabel.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, 0.4f).SetEase(Ease.InBack));

        seq.AppendCallback(() => backCountLabel.text = "1...");
        seq.Append(backCountLabel.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, 0.4f).SetEase(Ease.InBack));

        seq.AppendCallback(() => backCountLabel.text = "GO!");
        seq.Append(backCountLabel.transform.DOScale(1, 0.6f).SetEase(Ease.OutBack));
        seq.Append(backCountLabel.transform.DOScale(0, 2f).SetEase(Ease.InBack));

        return seq;
    }


    private void DisableInputs()
    {
        player1.Disable();
        player2.Disable();
        playerBoth.Disable();
    }

    private void EnableInputs()
    {
        player1.Enable();
        player2.Enable();
        playerBoth.Enable();
    }


}
