using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private Team team;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Ball ball))
        {
            ScoreManager.AddPoint(team);
            print($"—чет {ScoreManager.Score1} : {ScoreManager.Score2}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
