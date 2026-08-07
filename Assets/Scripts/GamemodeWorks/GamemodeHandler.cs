using UnityEngine;

public abstract class GamemodeHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Initialize(GameSession.GameMode);
        GameSession.OnGamemodeChanged += OnGamemodeChanged;
    }

    private void OnDestroy() => GameSession.OnGamemodeChanged -= OnGamemodeChanged;

    protected abstract void Initialize(GameMode gameMode);

    protected abstract void OnGamemodeChanged(GameMode gameMode);
}
