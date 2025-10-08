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

    private List<string> myOrderItems = new List<string>(); // store items for GameManager

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

        // Step 2: Wait 10–30 seconds before ordering
        float waitTime = Random.Range(10f, 30f);
        yield return new WaitForSeconds(waitTime);

        // Step 3: Place order
        myOrder = orderSystem.GenerateOrder();
        orderSystem.RegisterCustomerOrder(myOrder, this);

        // Parse order items for profit later
        myOrderItems = ParseOrderItems(myOrder);

        orderScreenPoint.isOccupied = false;

        // Step 4: Go to a free waiting spot
        myWaitingSpot = GetFreeWaitingSpot();
        if (myWaitingSpot != null)
        {
            yield return MoveTo(myWaitingSpot.position);
        }

        // Step 5: Wait until order is completed
        while (!orderCompleted)
        {
            yield return null;
        }

        // Step 6: Walk to pickup
        yield return FollowPath(pathToPickup);
        yield return MoveTo(pickupPoint.position);

        // Step 7: Walk to exit
        yield return FollowPath(pathToExit);
        yield return MoveTo(exitPoint.position);

        Destroy(gameObject);
    }

    public void CompleteOrder()
    {
        orderCompleted = true;

        // Earn money for all items sold in this order
        foreach (string item in myOrderItems)
        {
            GameManager.Instance.SellItem(item);
        }

        if (myWaitingSpot != null)
        {
            WaitingSpots ws = myWaitingSpot.GetComponent<WaitingSpots>();
            if (ws != null) ws.isOccupied = false;
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

    // Helper function to extract items from the order text
    private List<string> ParseOrderItems(string orderText)
    {
        List<string> items = new List<string>();
        string[] lines = orderText.Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith(" - "))
            {
                string item = line.Substring(3).Trim();
                items.Add(item);
            }
        }

        return items;
    }
}
