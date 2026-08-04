using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Paddle : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 direction;
    [SerializeField] Transform model;
    private Vector3 modelScale;
    private Rigidbody rb;
    private bool moving;
    private bool isInit;
    private bool isBot;
    private InputAction action;


    private void FixedUpdate()
    {
        if (isInit == false)
            return;

        TryMove();
    }

    private void OnDestroy()
    {
        if (isInit == false) return;

        if (!isBot)
        {
            action.performed -= StartMoving;
            action.canceled -= StopMoving;
        }
    }

    public void Init(Team team, bool isBot, InputAction inputAction)
    {
        rb = GetComponent<Rigidbody>();
        action = inputAction;
        this.isBot = isBot;
        if (!isBot)
        {
            action.performed += StartMoving;
            action.canceled += StopMoving;
        }
        modelScale = model.transform.localScale;
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

    public void HitAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(model.DOScale(modelScale * 0.95f, 0.05f));
        seq.Append(model.DOScale(modelScale, 0.2f).SetEase(Ease.OutBack));

    }
}
