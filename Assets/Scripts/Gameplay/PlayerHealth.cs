using System;
using UnityEngine;

public class PlayerHealth
{
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public int Health { get; private set; }

    public PlayerHealth(int initialHealth)
    {
        Health = initialHealth;
    }

    public void AddHealth(int hp)
    {
        Health += hp;
        OnHealthChanged?.Invoke(Health);
    }

    public void RemoveHealth(int hp)
    {
        Health = Mathf.Max(Health - hp, 0);
        OnHealthChanged?.Invoke(Health);

        if (Health == 0)
            OnDeath?.Invoke();
    }
}