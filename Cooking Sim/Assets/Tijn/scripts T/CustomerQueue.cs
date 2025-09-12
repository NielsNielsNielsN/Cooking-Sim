using UnityEngine;
using System.Collections.Generic;

public class CustomerQueue : MonoBehaviour
{
    [SerializeField] Transform[] waitingSpots;   // queue spots
    [SerializeField] Transform counterSpot;      // counter position
    [SerializeField] Transform exitSpot;         // where customers leave

    private Queue<CustomerAI> queue = new Queue<CustomerAI>();

    public void JoinQueue(CustomerAI customer)
    {
        queue.Enqueue(customer);
        UpdateQueuePositions();
    }

    public void ServeNextCustomer()
    {
        if (queue.Count > 0)
        {
            CustomerAI next = queue.Dequeue();
            next.GoToCounter(counterSpot, exitSpot);
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        int i = 0;
        foreach (CustomerAI customer in queue)
        {
            if (i < waitingSpots.Length)
            {
                customer.GoToWaitingSpot(waitingSpots[i]);
            }
            i++;
        }
    }
}
