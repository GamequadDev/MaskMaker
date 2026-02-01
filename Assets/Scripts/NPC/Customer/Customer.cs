using UnityEngine;
using System.Collections;
using TMPro;

public enum CustomerState
{
    WaitingInQueue,
    OfferingOrder,
    WaitingForMask,
    Leaving
}

public class Customer : MonoBehaviour
{
    private int _queuePosition = -1;
    public float waitTime = 10f; // Czas oczekiwania przy kasie
    private bool isServed = false;
    public TextMeshProUGUI timerText;
    
    [Header("State")]
    public CustomerState currentState = CustomerState.WaitingInQueue;
    private Coroutine waitCoroutine;

    public int queuePosition
    {
        get => _queuePosition;
        set
        {
            if (_queuePosition != value)
            {
                _queuePosition = value;
                // Jeśli klient trafił na 1. miejsce (indeks 0) i nie był jeszcze obsłużony
                if (_queuePosition == 0 && !isServed)
                {
                    currentState = CustomerState.WaitingInQueue;
                    waitCoroutine = StartCoroutine(WaitAndExit());
                }
            }
        }
    }

    public CustomerManager customerManager;
    public CustomerMovement customerMovement;

    private void Start()
    {
        if (customerMovement == null)
            customerMovement = GetComponentInChildren<CustomerMovement>();
    }

    private IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(0.2f);

        while (customerMovement != null && !customerMovement.HasReachedDestination())
        {
            yield return null;
        }

        float timer = waitTime;
        while (timer > 0 && currentState == CustomerState.WaitingInQueue)
        {
            if (timerText != null)
            {
                timerText.text = timer.ToString("F1");
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        
        // Tylko jeśli nadal czekamy w kolejce (nie przyjęto zamówienia)
        if (currentState == CustomerState.WaitingInQueue)
        {
            Debug.Log("Customer timeout - leaving shop");
            Leave();
        }
    }

    // Wywołane przez PlayerInteraction gdy gracz naciśnie E
    public void OfferOrder()
    {
        if (currentState != CustomerState.WaitingInQueue)
        {
            Debug.LogWarning("Customer is not waiting in queue!");
            return;
        }

        currentState = CustomerState.OfferingOrder;
        Debug.Log("Customer offering order to player");
        
        // Wywołaj OrderManager aby pokazał panel zamówienia
        if (OrderManager.instance != null)
        {
            OrderManager.instance.OpenOfferPanel(this);
        }
    }

    // Wywołane przez OrderManager gdy gracz zaakceptuje zamówienie
    public void OnOrderAccepted()
    {
        currentState = CustomerState.WaitingForMask;
        Debug.Log("Customer: Order accepted! Waiting for mask...");
        
        // Zatrzymaj timer - klient czeka bez limitu czasu
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
        
        // Wyczyść timer text
        if (timerText != null)
        {
            timerText.text = "...";
        }
    }

    // Wywołane przez OrderManager gdy gracz odrzuci zamówienie
    public void OnOrderRejected()
    {
        Debug.Log("Customer: Order rejected! Leaving...");
        Leave();
    }

    // Wywołane przez SellMask (poprzez OrderManager) gdy maska zostanie sprzedana
    public void OnMaskSold()
    {
        Debug.Log("Customer: Mask received! Leaving satisfied...");
        Leave();
    }

    private void Leave()
    {
        currentState = CustomerState.Leaving;
        isServed = true;
        
        if (customerManager != null && customerManager.queueManager != null)
        {
            // Wywołuje logikę wyjścia i usunięcia z kolejki w QueueManager
            customerManager.queueManager.ServeFirst();
        }
    }
}
