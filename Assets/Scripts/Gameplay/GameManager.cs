using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerType player1Type;
    [SerializeField] private PlayerType player2Type;

    [SerializeField] private Paddle paddle1;
    [SerializeField] private Paddle paddle2;

    private void Awake() => StartGame();

    private void StartGame()
    {
        InitializePaddles();
    }

    private void InitializePaddles()
    {
        paddle1.Init(Team.Player1, isBot: player1Type == PlayerType.Bot ? true : false);
        paddle2.Init(Team.Player2, isBot: player1Type == PlayerType.Bot ? true : false);

        if (player1Type == PlayerType.Bot && player2Type == PlayerType.Bot)
        {

        }
        else if(player1Type == PlayerType.Bot ^ player2Type == PlayerType.Bot)
        {
            if (player1Type == PlayerType.Player)
            {
                In.Instance.actions.PlayerBoth.Vertical.performed += paddle1.StartMoving;
                In.Instance.actions.PlayerBoth.Vertical.canceled += paddle1.StopMoving;
            }
            else
            {
                In.Instance.actions.PlayerBoth.Vertical.performed += paddle2.StartMoving;
                In.Instance.actions.PlayerBoth.Vertical.canceled += paddle2.StopMoving;
            }
        }
        else
        {
            In.Instance.actions.Player1.Vertical.performed += paddle1.StartMoving;
            In.Instance.actions.Player1.Vertical.canceled += paddle1.StopMoving;

            In.Instance.actions.Player2.Vertical.performed += paddle2.StartMoving;
            In.Instance.actions.Player2.Vertical.canceled += paddle2.StopMoving;
        }



    }




}
