using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public event Action<Team> OnGoal;

    [SerializeField] private Team _team;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _explosionSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Ball ball))
            
            RegisterGoal(ball);
    }

    private void RegisterGoal(Ball ball)
    {
        PlayGoalSound();
        OnGoal?.Invoke(_team);
    }

    private void PlayGoalSound()
    {
        _source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        _source.PlayOneShot(_explosionSound);
    }
}