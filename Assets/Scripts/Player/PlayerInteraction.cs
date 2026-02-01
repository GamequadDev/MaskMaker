using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public QueueManager queueManager;
    public SellMask sellMask; // Przypisz w Inspectorze!
    
    [Header("Player Area Detection")]
    public bool isInPlayerArea = false;
    
    [Header("UI (Optional)")]
    public GameObject interactionPrompt; // "Naciśnij E" UI element (opcjonalne)
    
    private Customer currentFirstCustomer;

    void Update()
    {
        // Sprawdź czy jest jakiś klient w kolejce
        if (queueManager != null)
        {
            currentFirstCustomer = queueManager.GetFirstCustomer();
        }

        // Pokazuj/ukrywaj prompt jeśli jest przypisany
        if (interactionPrompt != null)
        {
            bool shouldShowPrompt = isInPlayerArea && 
                                   currentFirstCustomer != null && 
                                   currentFirstCustomer.currentState == CustomerState.WaitingInQueue;
            interactionPrompt.SetActive(shouldShowPrompt);
        }

        // Obsługa klawisza E
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool maskFinished = ProgressManager.instance != null && ProgressManager.instance.isMaskFinished;
            Debug.Log($"=== E PRESSED === isInPlayerArea: {isInPlayerArea}, isMaskFinished: {maskFinished}, customer: {currentFirstCustomer != null}");
            
            if (isInPlayerArea)
            {
                TryOfferOrderToPlayer();
            }
            else
            {
                Debug.Log("E pressed but NOT in player area!");
            }
        }
    }

    void TryOfferOrderToPlayer()
    {
        // PRIORYTET 1: Jeśli maska jest gotowa, otwórz panel sprzedaży
        if (ProgressManager.instance != null && ProgressManager.instance.isMaskFinished)
        {
            Debug.Log("Mask is finished! Opening sell panel...");
            
            if (sellMask != null)
            {
                sellMask.OpenSellPanel();
            }
            else
            {
                Debug.LogError("SellMask is NOT assigned in PlayerInteraction Inspector!");
            }
            return;
        }
        
        // PRIORYTET 2: Jeśli nie ma klienta w kolejce, nie rób nic
        if (currentFirstCustomer == null)
        {
            Debug.Log("No customer in queue and mask not finished");
            return;
        }

        Debug.Log($"Customer state: {currentFirstCustomer.currentState}");

        // PRIORYTET 3: Jeśli jest klient czekający, odbierz zamówienie
        if (currentFirstCustomer.currentState != CustomerState.WaitingInQueue)
        {
            Debug.Log("Customer is not waiting for interaction");
            return;
        }

        // Wywołaj OfferOrder na pierwszym kliencie
        Debug.Log("Calling OfferOrder on customer!");
        currentFirstCustomer.OfferOrder();
    }

    // Wywołane przez trigger gdy gracz wejdzie do playerArea
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger enter with: {other.gameObject.name}, tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            isInPlayerArea = true;
            Debug.Log("Player entered player area");
        }
    }

    // Wywołane przez trigger gdy gracz wyjdzie z playerArea
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayerArea = false;
            Debug.Log("Player left player area");
        }
    }
}
