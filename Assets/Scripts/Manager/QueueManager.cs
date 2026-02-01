using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public List<Transform> queuePoints;
    public Transform exitPoint;
    private List<CustomerMovement> customers = new List<CustomerMovement>();

    public void AddToQueue(GameObject newCustomerObj)
    {
        CustomerMovement newCustomer = newCustomerObj.GetComponentInChildren<CustomerMovement>();
        if (newCustomer != null)
        {
            customers.Add(newCustomer);
            RefreshQueue();
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
}
