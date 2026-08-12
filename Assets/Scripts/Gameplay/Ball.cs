using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public float normalScale = 0.5f;

    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSpeed = 3;
    [SerializeField] private float _maxInitialSpeed;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private Transform _model;
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] ParticleSystem goalParticles;

    [Header("Звук")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private float _minPitch = 1;
    [SerializeField] private float _maxPitch = 1.5f;

    private Vector3 _modelScale;
    [SerializeField] Rigidbody _rb;

    public float SpeedPercent => _speed / (_minSpeed + _maxSpeed);
    [field: SerializeField] public bool Active { get; private set; }

    public void Init()
    {
        _modelScale = _model.localScale;
        _speed = _minSpeed;
    }

    private void FixedUpdate()
    {
        if (Active) _rb.linearVelocity = _direction * _speed * Time.timeScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Active == false) return;

        if (collision.gameObject.TryGetComponent(out Reflector reflector))
        {
            ChangeDirection(reflector.ReflectionX, reflector.ReflectionY);

            if (reflector.ShouldAddSpeed)
            {
                _speed += 0.3f;
                _speed = Mathf.Min(_speed, _maxSpeed);
            }
            HitAnimation();
            HitParticles(collision.contacts[0].point);
            HitSound();
        }
    }

    public void HitAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(_model.DOScale(_modelScale * 0.6f, 0.1f));
        seq.Append(_model.DOScale(_modelScale, 0.2f));
    }

    public void Enable()
    {
        Active = true;
        _rb.isKinematic = false;
        _speed = Mathf.Min(_speed, _maxInitialSpeed);

        int x = Random.Range(0, 2) == 0 ? -1 : 1;
        int y = Random.Range(0, 2) == 0 ? -1 : 1;
        _direction = new Vector2(x, y);
    }
    public void Disable()
    {
        Active = false;
        _rb.isKinematic = true;
    }

    public void HitParticles(Vector3 contactPoint)
    {
        var direction = (transform.position - contactPoint).normalized;
        Instantiate(hitParticles, contactPoint, Quaternion.LookRotation(direction));
    }

    public void GoalParticles() => goalParticles.Emit(30);

    public void HitSound()
    {
        _audioSource.pitch = Mathf.Lerp(_minPitch, _maxPitch, SpeedPercent);
        _audioSource.PlayOneShot(_hitClip);
    }

    private static int GetSign(ReflectDirection xDirectionSign)
    => xDirectionSign switch
    {
        ReflectDirection.Positive => 1,
        ReflectDirection.Negative => -1,
        _ => 0,
    };

    private static float ApplySign(float value, int sign)
        => sign == 0 ? value : sign * Mathf.Abs(value);

    private void ChangeDirection(ReflectDirection reflectionX, ReflectDirection reflectionY)
    {
        float x = _direction.x;
        float y = _direction.y;

        x = ApplySign(x, GetSign(reflectionX));
        y = ApplySign(y, GetSign(reflectionY));

        _direction = new Vector2(x, y);
    }
}