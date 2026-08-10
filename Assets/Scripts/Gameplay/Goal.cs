using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private Team _team;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _explosionSound;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Ball ball))
            RegisterGoal(ball);
    }

    private void RegisterGoal(Ball ball)
    {
        Team winner = _team == Team.Player ? Team.Bot : Team.Player;

        if (winner == Team.Player)
            ScoreManager.AddPoint();
        else if (winner == Team.Bot)
            print("Todo: добавить потерю сердечка");

        ball.HitParticles(ball.transform.position);
        ball.Disable();

        _source.pitch = Random.Range(0.9f, 1.1f);
        _source.PlayOneShot(_explosionSound);

        GameManager.Instance.Restart();
    }
}