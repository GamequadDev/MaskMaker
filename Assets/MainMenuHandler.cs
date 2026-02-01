using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameOnClick()
    {
        // Zmiana sceny na scenę gry
    }

    public void CreditsOnClick()
    {
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void BackToMainMenuOnClick()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitGameOnClick()
    {
        Application.Quit();
    }
}
