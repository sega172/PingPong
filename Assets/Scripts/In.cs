using UnityEngine;

public class In : MonoBehaviour
{
    public InputSystem_Actions actions;
    public static In Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if (Instance != null) return;

        GameObject go = new GameObject(name: "InputManager");
        Instance = go.AddComponent<In>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        actions = new InputSystem_Actions();
        actions.Enable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            actions.Disable();
            actions = null;
            Instance = null;
        }
    }

    //public void ChangeInputMap(InputType inputType)
    //{
    //    foreach (var action in actions)
    //    {
    //        action.Disable();
    //    }

    //    switch (inputType)
    //    {
    //        case InputType.SinglePlayer:
    //            actions.SinglePlayer.Enable();
    //            break;
    //        case InputType.TwoPlayers:
    //            actions.TwoPlayers.Enable();
    //            break;
    //    }
    //}

    public enum InputType { SinglePlayer, TwoPlayers, }
}
