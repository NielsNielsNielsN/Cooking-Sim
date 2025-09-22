using UnityEngine;
using System.Collections;

public class Customer : MonoBehaviour
{
    [HideInInspector] public OrderSystem orderSystem;
    [HideInInspector] public OrderScreenPoint orderScreenPoint; // one chosen point
    [HideInInspector] public Transform[] waitingSpots;
    [HideInInspector] public Transform pickupPoint;
    [HideInInspector] public Transform exitPoint;

    private float moveSpeed = 2f;

    private void Start()
    {
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // Step 1: Walk to chosen order screen
        orderScreenPoint.isOccupied = true;
        yield return MoveTo(orderScreenPoint.transform.position);

        // Step 2: Wait 10 seconds at order screen
        yield return new WaitForSeconds(10f);

        // Generate order
        orderSystem.GenerateOrder();

        // Free the order screen
        orderScreenPoint.isOccupied = false;

        // Step 3: Pick random waiting spot
        Transform randomSpot = waitingSpots[Random.Range(0, waitingSpots.Length)];
        yield return MoveTo(randomSpot.position);

        // Simulate order completion (replace later with "order ready" event)
        yield return new WaitForSeconds(5f);

        // Step 4: Go to pickup point
        yield return MoveTo(pickupPoint.position);

        // Step 5: Walk to exit and despawn
        yield return MoveTo(exitPoint.position);
        Destroy(gameObject);
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
