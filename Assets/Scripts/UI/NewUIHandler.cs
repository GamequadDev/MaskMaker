using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUIHandler : MonoBehaviour
{
    public GameObject UIPanel;
    private bool canInteract = false;
    public TextMeshProUGUI infoText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Gracz wszedł w zasięg obiektu!");
            canInteract = true;
        }
    }

    private void canInteractWithUI()
    {
        switch(UIPanel.name)
        {
            case "ChoosePanel":
                if(ProgressManager.instance.canChoosePanel)
                {
                    canInteract = true;
                }
                else
                {
                    infoText.text = "You can't choose mask yet. Please get the order first.";
                }
                break;
            case "PaintingPanel":
                if(ProgressManager.instance.canPaintPanel)
                {
                    canInteract = true;
                }
                else
                {
                    infoText.text = "You can't paint mask yet. Please finish previous tasks.";
                }
                break;
            case "DecorativePanel":
                if(ProgressManager.instance.canDecoratePanel)
                {
                    canInteract = true;
                }
                else
                {
                    infoText.text = "You can't decorate mask yet. Please finish previous tasks.";
                }
                break;
            case "FurnacePanel":
                if(ProgressManager.instance.canBakePanel)
                {
                    canInteract = true;
                }
                else
                {
                    infoText.text = "You can't bake mask yet. Please finish previous tasks.";
                }
                break;
        }
    }

    // Wywołuje się, gdy gracz wychodzi ze strefy
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Gracz opuścił zasięg obiektu.");
            canInteract = false;
            if (UIPanel != null) UIPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
             if (UIPanel != null)
            {
                canInteractWithUI();
                UIPanel.SetActive(true);
                if(UIPanel.name == "FurnacePanel")
                {
                    UIPanel.GetComponent<FurnaceEvent>().StartMinigame();
                }
                BlockUI.instance.ShowCursorAndPauseGame();
            }
        }
    }
}