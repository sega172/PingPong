using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour, IMovable
{
    [Header("Физика")]
    public float _speed;
    public float _acceleration = 10f;
    public float _deceleration = 15f;
    public float _bounceDamping = 1f;
    public float _yMin, _yMax;
    public float _targetDirection;
    public float _currentVelocity;
    public Rigidbody _rb;
    public Transform upBound, downBound;
    public bool _moving;

    [SerializeField] private List<Reflector> _reflectors;
    [SerializeField] private Transform _model;
    [SerializeField] private AudioClip _hitSound;

    private bool _isInit;
    private Sequence _hitAnimation;

    public float Speed => _speed;

    private void FixedUpdate()
    {
        if (_isInit == false)
            return;

        TryMove();
    }

    private void OnDestroy()
    {
        if (_isInit == false) return;

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
        _currentVelocity = 0f;
        _targetDirection = 0f;

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

    public void SetDirection(float direction)
        => _targetDirection = Mathf.Clamp(direction, -1f, 1f);

    public void OnHit()
    {
        _hitAnimation?.Restart();
    }

    private void GameManager_OnSetControls(bool enable)
        => _moving = enable;

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

        if (_rb.position.y > _yMax || _rb.position.y < _yMin)
        {
            _rb.position = new Vector3(_rb.position.x, Mathf.Clamp(_rb.position.y, _yMin, _yMax), 0);
            _currentVelocity *= -_bounceDamping;
        }
    }
}