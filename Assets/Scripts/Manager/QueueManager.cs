using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public List<Transform> queuePoints;
    private List<CustomerMovement> customers = new List<CustomerMovement>();

    public void AddToQueue(CustomerMovement newCustomer)
    {
        customers.Add(newCustomer);
        RefreshQueue();
    }

    public void ServeFirst()
    {
        if (customers.Count > 0)
        {
            customers[0].ExitShop(); 
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
            }
        }
    }
}
