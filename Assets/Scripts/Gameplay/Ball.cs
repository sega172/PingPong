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
        rb.linearVelocity = direction * _speed;
    }

    private void FixedUpdate()
    {
        
    }
}
