using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 direction;
    private Rigidbody rb;

    private void Awake() => Init();

    public void Init()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Collide" + collision.gameObject.name);
        if(collision.gameObject.TryGetComponent(out BallDirectionChanger dirChanger))
            direction = dirChanger.GetNewDirection(direction);
    }
}
