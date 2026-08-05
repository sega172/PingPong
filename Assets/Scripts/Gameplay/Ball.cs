using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxInitialSpeed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Transform model;
    private Vector3 modelScale;
    private Rigidbody rb;

    [SerializeField] ParticleSystem hitParticles;

    public bool Active;

    private void Awake() => Init();

    public void Init()
    {
        rb = GetComponent<Rigidbody>();
        modelScale = model.localScale;
        Enable();      
    }

    private void FixedUpdate()
    {
        if(Active)
            rb.linearVelocity = direction * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(Active == false) return;

        if(collision.gameObject.TryGetComponent(out BallDirectionChanger dirChanger))
        {
            direction = dirChanger.GetNewDirection(direction);
        }
        if(collision.gameObject.TryGetComponent(out Paddle paddle))
        {
            paddle.HitAnimation();
            _speed += 0.3f;
            _speed = Mathf.Min(_speed, _maxSpeed);
        }
        HitAnimation();
        HitParticles(collision.contacts[0].point);
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
    }
    public void Disable()
    {
        Active = false;
        rb.isKinematic = true;
    }

    public void HitParticles(Vector3 position)
    {
        Instantiate(hitParticles, position, Quaternion.identity);
    }
}
