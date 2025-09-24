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
    private string myOrder;
    private Transform myWaitingSpot;

    private bool orderCompleted = false;

    private void Start()
    {
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // Step 1: Walk to order screen
        orderScreenPoint.isOccupied = true;
        yield return FollowPath(pathToOrderScreen);
        yield return MoveTo(orderScreenPoint.transform.position);

        // Step 2: Place order
        myOrder = orderSystem.GenerateOrder();
        orderSystem.RegisterCustomerOrder(myOrder, this);
        orderScreenPoint.isOccupied = false;

        // Step 3: Go to a free waiting spot
        myWaitingSpot = GetFreeWaitingSpot();
        if (myWaitingSpot != null)
        {
            yield return MoveTo(myWaitingSpot.position);
        }

        // Step 4: Wait until order is completed
        while (!orderCompleted)
        {
            yield return null;
        }

        // Step 5: Walk to pickup
        yield return FollowPath(pathToPickup);
        yield return MoveTo(pickupPoint.position);

        // Step 6: Walk to exit
        yield return FollowPath(pathToExit);
        yield return MoveTo(exitPoint.position);

        Destroy(gameObject);
    }

    public void CompleteOrder()
    {
        orderCompleted = true;
        if (myWaitingSpot != null)
        {
            WaitingSpots ws = myWaitingSpot.GetComponent<WaitingSpots>();
            if (ws != null) ws.isOccupied = false; // free the spot
        }
    }

    private Transform GetFreeWaitingSpot()
    {
        foreach (var spot in waitingSpots)
        {
            WaitingSpots ws = spot.GetComponent<WaitingSpots>();
            if (ws != null && !ws.isOccupied)
            {
                ws.isOccupied = true;
                return spot;
            }
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
