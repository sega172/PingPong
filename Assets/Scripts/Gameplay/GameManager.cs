using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerType player1Type;
    [SerializeField] private PlayerType player2Type;

    [SerializeField] private Paddle paddle1;
    [SerializeField] private Paddle paddle2;

    private InputAction player1;
    private InputAction player2;
    private InputAction playerBoth;

    private void Awake()
    {
        player1 = InputSystem.actions.FindAction("Player1");
        player2 = InputSystem.actions.FindAction("Player2");
        playerBoth = InputSystem.actions.FindAction("PlayerBoth");

        StartGame();
    }

    private void StartGame()
    {
        InitializePaddles();
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
}
