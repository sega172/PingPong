using UnityEngine;

public class GamemodeSign : GamemodeHandler
{
    [SerializeField] GameMode requiredGamemode;

    protected override void Initialize(GameMode gameMode) => OnGamemodeChanged(gameMode);
    protected override void OnGamemodeChanged(GameMode gameMode) => gameObject.SetActive(ShouldEnable(gameMode));

    private bool ShouldEnable(GameMode gameMode) => gameMode == requiredGamemode;
}
