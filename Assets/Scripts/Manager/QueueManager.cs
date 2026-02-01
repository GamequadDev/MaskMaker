using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public List<Transform> queuePoints;
    public Transform exitPoint;
    private List<CustomerMovement> customers = new List<CustomerMovement>();

    public void AddToQueue(GameObject newCustomerObj)
    {
        Debug.Log($"AddToQueue called with: {newCustomerObj.name}");
        
        CustomerMovement newCustomer = newCustomerObj.GetComponentInChildren<CustomerMovement>();
        
        if (newCustomer != null)
        {
            Debug.Log($"CustomerMovement found! Adding to queue. Current count: {customers.Count}");
            customers.Add(newCustomer);
            RefreshQueue();
            Debug.Log($"After adding - Queue count: {customers.Count}");
        }
        else
        {
            Debug.LogError($"CustomerMovement NOT FOUND on {newCustomerObj.name}!");
        }
    }

    public void ServeFirst()
    {
        if (customers.Count > 0)
        {
            customers[0].ExitShop(exitPoint); 
            customers.RemoveAt(0);
            
            RefreshQueue();
        }
    }

    private void RefreshQueue()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (i < queuePoints.Count)
            {
                customers[i].MoveTo(queuePoints[i].position);

                Customer customerScript = customers[i].GetComponentInParent<Customer>();
                if (customerScript != null)
                {
                    customerScript.queuePosition = i;
                }
            }
        }
    }

    public int GetCurrentCustomerCount()
    {
    return customers.Count;
    }

    public Customer GetFirstCustomer()
    {
        Debug.Log($"GetFirstCustomer called. Customers count: {customers.Count}");
        
        if (customers.Count > 0 && customers[0] != null)
        {
            Customer customer = customers[0].GetComponentInParent<Customer>();
            Debug.Log($"First customer found: {customer != null}");
            return customer;
        }
        
        Debug.Log("No customers in queue");
        return null;
    }
}
