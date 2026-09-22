using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour, IMovable
{
    public event Action OnReflect;

    [SerializeField] private float _smoothTime = 0.08f;
    [SerializeField] private float _maxSpeed = 25f;
    [SerializeField] private float _yMin = -2.81f;
    [SerializeField] private float _yMax = 2.81f;
    [SerializeField] private List<Reflector> _reflectors;
    [SerializeField] private Transform _model;
    [SerializeField] private AudioClip _hitSound;

    private Rigidbody _rb;
    private float _targetY;
    private float _currentVelocity;
    private bool _moving;
    private bool _isInit;
    private Sequence _hitAnimation;

    public float SmoothTime => _smoothTime;
    public float MaxSpeed => _maxSpeed;
    public float TargetY => _targetY;

    private void FixedUpdate()
    {
        if (_isInit == false)
            return;

        TryMove();
    }

    private void OnDestroy()
    {
        if (_isInit == false)
            return;

        GameManager.OnSetControls -= GameManager_OnSetControls;

        if (_reflectors != null)
        {
            foreach (var reflector in _reflectors)
                if (reflector != null) reflector.OnReflect -= OnHit;
        }

        _hitAnimation?.Kill();
    }

    public void Init(Team team, bool isBot)
    {
        _rb = GetComponent<Rigidbody>();
        _targetY = transform.position.y;
        _currentVelocity = 0f;

        foreach (var reflector in _reflectors)
            reflector.OnReflect += OnHit;

        GameManager.OnSetControls += GameManager_OnSetControls;

        var seq = DOTween.Sequence();
        seq.Append(_model.DOLocalMoveX(-0.1f, 0.05f).From(0));
        seq.Append(_model.DOLocalMoveX(0, 1.3f).SetEase(Ease.OutElastic));
        seq.SetAutoKill(false);
        seq.Pause();
        _hitAnimation = seq;

        _isInit = true;
    }

    public void SetTargetY(float targetY)
    {
        _targetY = Mathf.Clamp(targetY, _yMin, _yMax);
    }

    public void SetDirection(float direction)
    {
        float currentY = _rb != null ? _rb.position.y : transform.position.y;
        SetTargetY(currentY + direction);
    }

    public void OnHit()
    {
        _hitAnimation?.Restart();
        OnReflect?.Invoke();
    }

    private void GameManager_OnSetControls(bool enable)
    {
        _moving = enable;
        if (enable)
        {
            _targetY = transform.position.y;
            _currentVelocity = 0f;
        }
    }

    private void TryMove()
    {
        if (_moving == false)
            return;

        float newY = Mathf.SmoothDamp(_rb.position.y, _targetY, ref _currentVelocity, _smoothTime, _maxSpeed, Time.fixedDeltaTime);
        newY = Mathf.Clamp(newY, _yMin, _yMax);
        _rb.position = new Vector3(_rb.position.x, newY, _rb.position.z);
    }
}