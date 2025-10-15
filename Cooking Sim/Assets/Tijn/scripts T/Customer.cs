using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    [HideInInspector] public CustomerOrder orderSystem;
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

    private List<string> myOrderItems = new List<string>(); // Store items for GameManager

    // Event the spawner can subscribe to (called right after placing the order)
    public Action OnOrderComplete;

    private void Start()
    {
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // Step 1: Walk to order screen
        if (orderScreenPoint != null) orderScreenPoint.isOccupied = true;
        yield return FollowPath(pathToOrderScreen);
        if (orderScreenPoint != null) yield return MoveTo(orderScreenPoint.transform.position);

        // Step 2: Wait 10–30 seconds before ordering
        float waitTime = UnityEngine.Random.Range(10f, 30f);
        yield return new WaitForSeconds(waitTime);

        // Step 3: Place order
        myOrder = orderSystem.GenerateOrder();
        orderSystem.RegisterCustomerOrder(myOrder, this);

        // Parse order items for profit later
        myOrderItems = ParseOrderItems(myOrder);

        // mark order screen free immediately after placing order
        if (orderScreenPoint != null) orderScreenPoint.isOccupied = false;

        // Notify spawner (or other listeners) that the order is placed and they can assign a waiting spot
        OnOrderComplete?.Invoke();

        // If spawner assigned a waiting spot via GoToWaitingSpot, use it.
        // Otherwise find one ourselves.
        if (myWaitingSpot == null)
        {
            myWaitingSpot = GetFreeWaitingSpot();
        }

        if (myWaitingSpot != null)
        {
            yield return MoveTo(myWaitingSpot.position);
        }

        // Step 5: Wait until order is completed (served)
        while (!orderCompleted)
        {
            yield return null;
        }

        // Step 6: Walk to pickup
        yield return FollowPath(pathToPickup);
        if (pickupPoint != null) yield return MoveTo(pickupPoint.position);

        // Step 7: Walk to exit
        yield return FollowPath(pathToExit);
        if (exitPoint != null) yield return MoveTo(exitPoint.position);

        Destroy(gameObject);
    }

    // Called by OrderSystem when this customer's order is ready / being completed
    public void CompleteOrder()
    {
        orderCompleted = true;

        // Earn money for all items sold in this order and reduce stock
        foreach (string item in myOrderItems)
        {
            GameManager.Instance.SellItem(item);
        }

        // Free the waiting spot so others can use it
        if (myWaitingSpot != null)
        {
            WaitingSpot ws = myWaitingSpot.GetComponent<WaitingSpot>();
            if (ws != null) ws.isOccupied = false;
            myWaitingSpot = null;
        }
    }

    // Called by spawner (or other systems) to assign a waiting spot externally
    public void GoToWaitingSpot(Transform spot)
    {
        if (spot == null) return;
        myWaitingSpot = spot;
    }

    private Transform GetFreeWaitingSpot()
    {
        if (waitingSpots == null) return null;

        foreach (var spot in waitingSpots)
        {
            WaitingSpot ws = spot.GetComponent<WaitingSpot>();
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
        if (waypoints == null) yield break;
        foreach (var point in waypoints)
        {
            if (point == null) continue;
            yield return MoveTo(point.position);
        }
    }

    // Helper function to extract items from the order text
    private List<string> ParseOrderItems(string orderText)
    {
        List<string> items = new List<string>();
        if (string.IsNullOrEmpty(orderText)) return items;

        string[] lines = orderText.Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith(" - "))
            {
                string item = line.Substring(3).Trim();
                if (!string.IsNullOrEmpty(item))
                    items.Add(item);
            }
        }

        return items;
    }
}
