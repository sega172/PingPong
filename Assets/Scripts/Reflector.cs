using System;
using UnityEngine;

public class Reflector : MonoBehaviour
{
    public event Action OnHit;
    public bool addSpeed = false;
    [SerializeField] private DirectionChangeMode changeXMode;
    [SerializeField] private DirectionChangeMode changeYMode;

    public Vector3 Reflect(Vector3 oldDirection)
    {
        float x = ApplyDirectionChange(oldDirection.x, changeXMode);
        float y = ApplyDirectionChange(oldDirection.y, changeYMode);
        OnHit?.Invoke();
        return new Vector3(x, y);
    }

    private float ApplyDirectionChange(float value, DirectionChangeMode change)
    {
        if (change == DirectionChangeMode.None)
            return value;

        bool shouldFlip = (value > 0 && change == DirectionChangeMode.Negative) ||
                          (value < 0 && change == DirectionChangeMode.Positive);

        return shouldFlip ? -value : value;
    }

    public enum DirectionChangeMode
    {
        None,
        Positive, 
        Negative  
    }
}
