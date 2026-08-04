using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Paddle : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 direction;
    private Rigidbody rb;
    private bool moving;
    private bool isInit;

    //mock
    [SerializeField] Team team;
    [SerializeField] private bool bot;


    private void Awake() => Init(team, bot);

    private void FixedUpdate()
    {
        if (isInit == false)
            return;

        TryMove();
    }

    public void Init(Team team, bool isBot)
    {
        rb = GetComponent<Rigidbody>();

        isInit = true;
    }

    public void StartMoving(InputAction.CallbackContext ctx)
    {
        moving = true;
        direction = Vector3.up * ctx.ReadValue<float>();
    }

    public void StopMoving(InputAction.CallbackContext ctx)
    {
        moving = false;
        direction = Vector3.zero;
    }

    private void TryMove()
    {
        if (!moving) return;

        rb.position += direction * _speed * Time.fixedDeltaTime;
    }
}
