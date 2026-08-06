using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _deceleration = 15f;
    [SerializeField] Transform model;

    [SerializeField] private float yMin, yMax;
    [SerializeField] AudioSource source;
    [SerializeField] AudioSource engineSource;
    [SerializeField] AudioClip hitSound;

    [SerializeField] private float _bounceDamping = 1f;

    [SerializeField] private List<Reflector> reflectors;

    private Vector3 modelScale;
    private Rigidbody rb;
    private bool moving;
    private bool isInit;
    private bool isBot;
    private InputAction action;
    private float currentVelocity;
    private float targetDirection;

    private void FixedUpdate()
    {
        if (isInit == false)
            return;

        TryMove();

        float t = Mathf.Abs(currentVelocity) / _speed;
        engineSource.volume = Mathf.Lerp(0, 0.5f, t);
        engineSource.pitch = Mathf.Lerp(1f, 2f, t);
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
        currentVelocity = 0f;
        targetDirection = 0f;
        foreach (var reflector in reflectors)
            reflector.OnHit += OnHit;

    }

    public void SetDirection(float direction)
    {
        moving = Mathf.Abs(direction) > 0.01f;
        targetDirection = Mathf.Clamp(direction, -1f, 1f);
    }

    public void StartMoving(InputAction.CallbackContext ctx)
    {
        moving = true;
        targetDirection = ctx.ReadValue<float>(); // -1 или 1
    }

    public void StopMoving(InputAction.CallbackContext ctx)
    {
        moving = false;
        targetDirection = 0f;
    }

    private void TryMove()
    {
        if (moving)
        {
            currentVelocity += targetDirection * _acceleration * Time.fixedDeltaTime;
            currentVelocity = Mathf.Clamp(currentVelocity, -_speed, _speed);
        }
        else
        {
            if (Mathf.Abs(currentVelocity) > 0.01f)
            {
                currentVelocity -= Mathf.Sign(currentVelocity) * _deceleration * Time.fixedDeltaTime;

                if (Mathf.Abs(currentVelocity) < 0.01f)
                    currentVelocity = 0f;
            }
        }

        rb.position += Vector3.up * currentVelocity * Time.fixedDeltaTime;
        if(rb.position.y > yMax || rb.position.y < yMin)
        {
            rb.position = new Vector3(rb.position.x, Mathf.Clamp(rb.position.y, yMin, yMax), 0);
            currentVelocity *= -_bounceDamping;
        }

    }

    public void OnHit()
    {
        var seq = DOTween.Sequence();

        //seq.Append(model.DOScale(modelScale * 0.90f, 0.05f));
        seq.Append(model.DOLocalMoveX(-0.1f, 0.05f));

        //seq.Append(model.DOScale(modelScale, 0.3f).SetEase(Ease.OutBack));
        //seq.Append(model.DOLocalMoveX(0, 0.4f).SetEase(Ease.OutBack));
        seq.Append(model.DOLocalMoveX(0, 1.3f).SetEase(Ease.OutElastic));

        source.pitch = Mathf.Lerp(1, 2, Ball.Instance.SpeedPercent);
        source.PlayOneShot(hitSound);
    }
}