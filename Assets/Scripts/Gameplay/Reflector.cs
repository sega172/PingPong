using System;
using UnityEngine;

public class Reflector : MonoBehaviour
{
    public event Action OnReflect;


    [field: SerializeField] public bool ShouldAddSpeed = false;
    [field: SerializeField] public ReflectDirection ReflectionX { get; private set; }
    [field: SerializeField] public ReflectDirection ReflectionY { get; private set; }
    
    public void InvokeReflect()
    {
        OnReflect?.Invoke();
    }
}

public enum ReflectDirection
{
    None,
    Positive, 
    Negative  
}
