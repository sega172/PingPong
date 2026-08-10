using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour, IMovable
{
    [Header("Физика")]
    [SerializeField] private float _speed;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _deceleration = 15f;
    [SerializeField] private float _bounceDamping = 1f;
    [SerializeField] private float _yMin, _yMax;
    [SerializeField] private List<Reflector> _reflectors;
    private float _targetDirection;
    private float _currentVelocity;
    private Rigidbody _rb;

    [Header("Визуал")]
    [SerializeField] Transform _model;
    [SerializeField] AudioClip _hitSound;

    // состояние
    private bool _moving;
    private bool _isInit;

    private void FixedUpdate()
    {
        if (_isInit == false)
            return;

        TryMove();

        float t = Mathf.Abs(_currentVelocity) / _speed;
    }

    public void Init(Team team, bool isBot)
    {
        _rb = GetComponent<Rigidbody>();
        _currentVelocity = 0f;
        _targetDirection = 0f;

        foreach (var reflector in _reflectors)
            reflector.OnReflect += OnHit;

        GameManager.OnSetControls += GameManager_OnSetControls;

        _isInit = true;
    }

    private void OnDestroy()
    {
        if (_isInit == false) return;

        GameManager.OnSetControls -= GameManager_OnSetControls;
    }

    private void GameManager_OnSetControls(bool enable) 
        => _moving = enable;

    public void SetDirection(float direction) 
        => _targetDirection = Mathf.Clamp(direction, -1f, 1f);

    private void TryMove()
    {
        if (_moving)
        {
            _currentVelocity += _targetDirection * _acceleration * Time.fixedDeltaTime;
            _currentVelocity = Mathf.Clamp(_currentVelocity, -_speed, _speed);
        }
        else
        {
            if (Mathf.Abs(_currentVelocity) > 0.01f)
            {
                _currentVelocity -= Mathf.Sign(_currentVelocity) * _deceleration * Time.fixedDeltaTime;

                if (Mathf.Abs(_currentVelocity) < 0.01f)
                    _currentVelocity = 0f;
            }
        }

        _rb.position += Vector3.up * _currentVelocity * Time.fixedDeltaTime;

        if(_rb.position.y > _yMax || _rb.position.y < _yMin)
        {
            _rb.position = new Vector3(_rb.position.x, Mathf.Clamp(_rb.position.y, _yMin, _yMax), 0);
            _currentVelocity *= -_bounceDamping;
        }

    }

    public void OnHit()
    {
        var seq = DOTween.Sequence();

        seq.Append(_model.DOLocalMoveX(-0.1f, 0.05f));
        seq.Append(_model.DOLocalMoveX(0, 1.3f).SetEase(Ease.OutElastic));
    }
}