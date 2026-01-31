using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class HandlerPlayerUI : MonoBehaviour
{

    [Tooltip("UI")]
    public GameObject ui;
    private bool canInteract = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Gracz wszedł w zasięg obiektu!");
            canInteract = true;
        }
    }

    // Wywołuje się, gdy gracz wychodzi ze strefy
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Gracz opuścił zasięg obiektu.");
            canInteract = false;
            if (ui != null) ui.SetActive(false);
        }
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
             if (ui != null)
            {
                ui.SetActive(true);
            }
        }
    }
}
