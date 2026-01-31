using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    private int _queuePosition = -1;
    public float waitTime = 10f; // Czas oczekiwania przy kasie
    private bool isServed = false;

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
                    StartCoroutine(WaitAndExit());
                }
            }
        }
    }

    public CustomerManager customerManager;
    public CustomerMovement customerMovement;

    private IEnumerator WaitAndExit()
    {
        yield return new WaitForSeconds(waitTime);
        
        isServed = true;
        if (customerManager != null && customerManager.queueManager != null)
        {
            // Wywołuje logikę wyjścia i usunięcia z kolejki w QueueManager
            customerManager.queueManager.ServeFirst();
        }
    }
}
