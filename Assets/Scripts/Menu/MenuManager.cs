using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject choosePanel;

    private void Awake() => Initialize();

    public void Initialize()
    {
        EnableMainScreen();



    }


    public void EnableMainScreen()
    {
        mainPanel.SetActive(true);
        choosePanel.SetActive(false);
    }

    public void EnableChooseScreen()
    {
        mainPanel.SetActive(false);
        choosePanel.SetActive(true);
    }


}
