using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour, IMovable
{
    [SerializeField] private float _speed;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _deceleration = 15f;
    [SerializeField] Transform model;

    [SerializeField] private float yMin, yMax;
    [SerializeField] AudioSource engineSource;
    [SerializeField] AudioClip hitSound;

    [SerializeField] private float _bounceDamping = 1f;

    [SerializeField] private List<Reflector> reflectors;

    private Vector3 modelScale;
    private Rigidbody rb;
    private bool moving;
    private bool isInit;
    private float currentVelocity;
    private float targetDirection;

    private void FixedUpdate()
    {
        if (isInit == false)
            return;

        TryMove();

        float t = Mathf.Abs(currentVelocity) / _speed;
    }

    public void Init(Team team, bool isBot)
    {
        rb = GetComponent<Rigidbody>();
        modelScale = model.transform.localScale;
        isInit = true;
        currentVelocity = 0f;
        targetDirection = 0f;
        foreach (var reflector in reflectors)
            reflector.OnHit += OnHit;

        GameManager.OnSetControls += GameManager_OnSetControls;

    }

    private void OnDestroy()
    {
        if (isInit == false) return;

        GameManager.OnSetControls -= GameManager_OnSetControls;
    }

    private void GameManager_OnSetControls(bool enable)
    {
        moving = enable;
    }

    public void SetDirection(float direction)
    {
        targetDirection = Mathf.Clamp(direction, -1f, 1f);
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

        seq.Append(model.DOLocalMoveX(-0.1f, 0.05f));
        seq.Append(model.DOLocalMoveX(0, 1.3f).SetEase(Ease.OutElastic));
    }
}