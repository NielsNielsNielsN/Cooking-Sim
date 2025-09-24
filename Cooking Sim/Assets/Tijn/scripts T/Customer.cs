using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    [HideInInspector] public OrderSystem orderSystem;
    [HideInInspector] public OrderScreenPoint orderScreenPoint;
    [HideInInspector] public Transform[] waitingSpots;
    [HideInInspector] public Transform pickupPoint;
    [HideInInspector] public Transform exitPoint;

    [HideInInspector] public List<Transform> pathToOrderScreen;
    [HideInInspector] public List<Transform> pathToPickup;
    [HideInInspector] public List<Transform> pathToExit;

    private float moveSpeed = 2f;

    private void Start()
    {
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // Step 1: Walk through path to order screen
        orderScreenPoint.isOccupied = true;
        yield return FollowPath(pathToOrderScreen);
        yield return MoveTo(orderScreenPoint.transform.position);

        // Step 2: Wait 10 seconds at order screen
        yield return new WaitForSeconds(10f);

        // Generate order
        orderSystem.GenerateOrder();
        orderScreenPoint.isOccupied = false;

        // Step 3: Find an open waiting spot
        Transform freeSpot = FindFreeWaitingSpot();
        if (freeSpot != null)
        {
            WaitingSpot ws = freeSpot.GetComponent<WaitingSpot>();
            ws.isOccupied = true;
            yield return MoveTo(freeSpot.position);

            // Fake wait for order ready
            yield return new WaitForSeconds(5f);
            ws.isOccupied = false;
        }

        // Step 4: Walk through path to pickup
        yield return FollowPath(pathToPickup);
        yield return MoveTo(pickupPoint.position);

        // Step 5: Walk through path to exit
        yield return FollowPath(pathToExit);
        yield return MoveTo(exitPoint.position);

        Destroy(gameObject);
    }

    private Transform FindFreeWaitingSpot()
    {
        foreach (Transform spot in waitingSpots)
        {
            WaitingSpot ws = spot.GetComponent<WaitingSpot>();
            if (!ws.isOccupied) return spot;
        }
        return null;
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator FollowPath(List<Transform> waypoints)
    {
        foreach (var point in waypoints)
        {
            yield return MoveTo(point.position);
        }
    }
}
