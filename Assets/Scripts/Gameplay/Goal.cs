using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private Team team;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Ball ball))
        {
            Team winner = team == Team.Player1 ? Team.Player2 : Team.Player1;
            ScoreManager.AddPoint(winner);
            ball.HitParticles(ball.transform.position);
            ball.Disable();
            GameManager.Instance.Restart();
        }
    }
}
