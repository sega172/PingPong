using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    public static Ball Instance { get; private set; }

    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSpeed = 3;
    [SerializeField] private float _maxInitialSpeed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Transform model;

    [Header("Звук")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private float minPitch = 1;
    [SerializeField] private float maxPitch = 1.5f;

    public float SpeedPercent => _speed / (_minSpeed + _maxSpeed);

    private Vector3 modelScale;
    private Rigidbody rb;

    [SerializeField] ParticleSystem hitParticles;

    public bool Active;

    private void Awake() => Init();

    public void Init()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        modelScale = model.localScale;
        _speed = _minSpeed;
        Enable();
    }

    private void FixedUpdate()
    {
        if (Active)
            rb.linearVelocity = direction * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Active == false) return;

        if (collision.gameObject.TryGetComponent(out Reflector reflector))
        {
            direction = reflector.Reflect(direction);

            if (reflector.addSpeed)
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

        seq.Append(model.DOScale(modelScale * 0.6f, 0.1f));
        seq.Append(model.DOScale(modelScale, 0.2f));

    }

    public void Enable()
    {
        Active = true;
        rb.isKinematic = false;
        _speed = Mathf.Min(_speed, _maxInitialSpeed);

        int x = Random.Range(0, 2) == 0 ? -1 : 1;
        int y = Random.Range(0, 2) == 0 ? -1 : 1;
        direction = new Vector2(x, y);
    }
    public void Disable()
    {
        Active = false;
        rb.isKinematic = true;
    }

    public void HitParticles(Vector3 contactPoint)
    {
        var direction = (transform.position - contactPoint).normalized;
        Instantiate(hitParticles, contactPoint, Quaternion.LookRotation(direction));
    }

    public void HitSound()
    {
        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, SpeedPercent);
        audioSource.PlayOneShot(hitClip);
    }
}
