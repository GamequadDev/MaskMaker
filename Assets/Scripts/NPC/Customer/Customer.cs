using UnityEngine;
using System.Collections;
using TMPro;

public class Customer : MonoBehaviour
{
    private int _queuePosition = -1;
    public float waitTime = 10f; // Czas oczekiwania przy kasie
    private bool isServed = false;
    public TextMeshProUGUI timerText;

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
        while (timer > 0)
        {
            if (timerText != null)
            {
                timerText.text = timer.ToString("F1");
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        
        isServed = true;
        if (customerManager != null && customerManager.queueManager != null)
        {
            // Wywołuje logikę wyjścia i usunięcia z kolejki w QueueManager
            customerManager.queueManager.ServeFirst();
        }
    }
}
