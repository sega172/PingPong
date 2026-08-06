using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private Team team;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip explosionSound;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Ball ball))
        {
            Team winner = team == Team.Player1 ? Team.Player2 : Team.Player1;
            ScoreManager.AddPoint(winner);
            ball.HitParticles(ball.transform.position);
            ball.Disable();

            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(explosionSound);

            GameManager.Instance.Restart();
        }
    }
}
