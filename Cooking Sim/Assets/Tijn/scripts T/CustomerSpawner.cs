using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private OrderSystem orderSystem;
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
        // Spawn 2 customers with a delay
        StartCoroutine(SpawnInitialCustomers());

        // Start the random spawn loop
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
        // Limit to max 5 active orders
        if (orderSystem.activeOrders >= orderSystem.maxOrders) return;

        foreach (var screen in orderScreenPoints)
        {
            if (!screen.isOccupied)
            {
                SpawnCustomer(screen);
                return;
            }
        }
    }

    private void SpawnCustomer(OrderScreenPoint screen)
    {
        GameObject newCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);
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
    }
}
