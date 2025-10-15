using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Prefabs")]
    [SerializeField] private GameObject[] customerPrefabs; // ← multiple customer types

    [SerializeField] private CustomerOrder orderSystem;
    [SerializeField] private OrderScreenPoint[] orderScreenPoints;
    [SerializeField] private Transform[] waitingSpots;
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Paths")]
    [SerializeField] private List<Transform> pathToOrderScreen;
    [SerializeField] private List<Transform> pathToPickup;
    [SerializeField] private List<Transform> pathToExit;

    private void Start()
    {
        StartCoroutine(SpawnInitialCustomers());
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnInitialCustomers()
    {
        TrySpawnCustomer();
        yield return new WaitForSeconds(1f);
        TrySpawnCustomer();
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(20f, 50f);
            yield return new WaitForSeconds(waitTime);
            TrySpawnCustomer();
        }
    }

    private void TrySpawnCustomer()
    {
        if (orderSystem.activeOrders >= orderSystem.maxOrders)
            return;

        OrderScreenPoint freeScreen = null;
        foreach (var screen in orderScreenPoints)
        {
            if (!screen.isOccupied)
            {
                freeScreen = screen;
                break;
            }
        }

        if (freeScreen != null)
        {
            SpawnCustomer(freeScreen);
        }
        else
        {
            Debug.Log("All order screens are occupied — waiting for a free one.");
        }
    }

    private void SpawnCustomer(OrderScreenPoint screen)
    {
        if (screen == null || customerPrefabs.Length == 0) return;

        // 🌀 Randomly choose one of the three (or however many) prefabs
        GameObject chosenPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];

        GameObject newCustomer = Instantiate(chosenPrefab, transform.position, Quaternion.identity);
        Customer c = newCustomer.GetComponent<Customer>();

        c.orderSystem = orderSystem;
        c.orderScreenPoint = screen;
        c.waitingSpots = waitingSpots;
        c.pickupPoint = pickupPoint;
        c.exitPoint = exitPoint;

        c.pathToOrderScreen = pathToOrderScreen;
        c.pathToPickup = pathToPickup;
        c.pathToExit = pathToExit;

        screen.isOccupied = true;

        c.OnOrderComplete += () => SendToWaitingSpot(c);
    }

    private void SendToWaitingSpot(Customer c)
    {
        if (c == null || waitingSpots.Length == 0) return;

        foreach (Transform spot in waitingSpots)
        {
            WaitingSpot ws = spot.GetComponent<WaitingSpot>();
            if (ws != null && !ws.isOccupied)
            {
                ws.isOccupied = true;
                c.GoToWaitingSpot(spot);
                return;
            }
        }

        Debug.Log("No free waiting spots available!");
    }
}
