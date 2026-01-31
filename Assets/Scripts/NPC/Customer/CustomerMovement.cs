using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerMovement : MonoBehaviour
{
  private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 position)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(position);
        }
    }

    public void ExitShop(Transform endPoint)
    {
        if (endPoint != null)
        {
            MoveTo(endPoint.position);
        }
        Debug.Log("Klient obsłużony, wychodzi...");
        Destroy(gameObject, 10f); 
    }
}
