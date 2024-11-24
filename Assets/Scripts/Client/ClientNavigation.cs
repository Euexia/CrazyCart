/*using UnityEngine;
using UnityEngine.AI;

public class ClientNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    public System.Action OnDestinationReached;

    public void InitializeAgent()
    {
        if (!TryGetComponent(out agent))
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        agent.avoidancePriority = Random.Range(0, 100);
    }

    public void MoveTo(Vector3 position)
    {
        if (agent != null)
        {
            agent.SetDestination(position);
        }
    }

    private void Update()
    {
        if (agent != null && agent.remainingDistance <= agent.stoppingDistance)
        {
            OnDestinationReached?.Invoke();
        }
    }
}
*/