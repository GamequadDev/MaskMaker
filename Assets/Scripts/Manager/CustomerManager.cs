using UnityEngine;
using System.Collections;


public class CustomerManager : MonoBehaviour
{
    [Header("Ustawienia Spawnera")]
    public GameObject customerPrefab;
    public Transform spawnPoint;      
    public float spawnInterval = 5f; 
    public int maxQueueSize = 3;

    [Header("Referencje")]
    public QueueManager queueManager;

    void Start()
    {
        maxQueueSize = queueManager.queuePoints.Count;
        StartCoroutine(SpawnCustomers());
    }

    IEnumerator SpawnCustomers()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

           if (queueManager != null && queueManager.GetCurrentCustomerCount() < maxQueueSize)
            {
                SpawnNewCustomer();
            }
            else
            {
                Debug.Log("Kolejka pełna, czekam na wolne miejsce...");
            }
        }
    }
    void SpawnNewCustomer()
    {
        Debug.Log("SpawnNewCustomer called!");
        GameObject newCustomerObj = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        if (newCustomerObj != null)
        {
            Debug.Log($"Customer instantiated: {newCustomerObj.name}");
            
            Customer customerScript = newCustomerObj.GetComponent<Customer>();
            if (customerScript != null)
            {
                customerScript.customerManager = this;
                Debug.Log("Customer script found and assigned");
            }
            else
            {
                Debug.LogError("Customer script NOT FOUND!");
            }

            queueManager.AddToQueue(newCustomerObj);
        }
        else
        {
            Debug.LogError("Failed to instantiate customer!");
        }
    }
}
