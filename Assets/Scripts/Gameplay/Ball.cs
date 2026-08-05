using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 direction;
    [SerializeField] private Transform model;
    private Vector3 modelScale;
    private Rigidbody rb;

    [SerializeField] ParticleSystem hitParticles;

    private void Awake() => Init();

    public void Init()
    {
        rb = GetComponent<Rigidbody>();
        modelScale = model.localScale;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out BallDirectionChanger dirChanger))
        {
            direction = dirChanger.GetNewDirection(direction);
        }
        if(collision.gameObject.TryGetComponent(out Paddle paddle))
        {
            paddle.HitAnimation();
            _speed += 0.3f;
        }
        HitAnimation();
        Instantiate(hitParticles, collision.contacts[0].point, Quaternion.identity);
    }

    public void HitAnimation()
    {
        var seq = DOTween.Sequence();

        seq.Append(model.DOScale(modelScale * 0.6f, 0.1f));
        seq.Append(model.DOScale(modelScale, 0.2f));

    }
}
