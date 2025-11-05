using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Prefabs")]
    [SerializeField] private GameObject[] customerPrefabs;
    [SerializeField] private CustomerOrder orderSystem;
    [SerializeField] private OrderScreenPoint[] orderScreenPoints;
    [SerializeField] private Transform[] waitingSpots;
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Paths")]
    [SerializeField] private List<Transform> pathToOrderScreen;
    [SerializeField] private List<Transform> pathToPickup;
    [SerializeField] private List<Transform> pathToExit;

    [Header("Limits")]
    [SerializeField] private int maxTotalCustomers = 5;
    [SerializeField] private float minSpawnDelay = 20f;
    [SerializeField] private float maxSpawnDelay = 50f;

    private int currentCustomerCount = 0;

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
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            TrySpawnCustomer();
        }
    }

    private void TrySpawnCustomer()
    {
        if (currentCustomerCount >= maxTotalCustomers) return;
        if (orderSystem == null || orderSystem.activeOrders >= orderSystem.maxOrders) return;

        OrderScreenPoint freeScreen = GetFreeOrderScreen();
        if (freeScreen != null)
            SpawnCustomer(freeScreen);
    }

    private OrderScreenPoint GetFreeOrderScreen()
    {
        foreach (var screen in orderScreenPoints)
            if (screen != null && !screen.isOccupied)
                return screen;
        return null;
    }

    private void SpawnCustomer(OrderScreenPoint screen)
    {
        if (screen == null || customerPrefabs == null || customerPrefabs.Length == 0) return;

        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
        GameObject newCust = Instantiate(prefab, transform.position, Quaternion.identity);

        Customer c = newCust.GetComponent<Customer>();
        if (c != null)
        {
            c.orderSystem = orderSystem;
            c.orderScreenPoint = screen;
            c.waitingSpots = waitingSpots;
            c.pickupPoint = pickupPoint;
            c.exitPoint = exitPoint;
            c.pathToOrderScreen = pathToOrderScreen;
            c.pathToPickup = pathToPickup;
            c.pathToExit = pathToExit;

            // Hook up destroy event
            c.OnCustomerDestroy += DecrementCount;
        }

        currentCustomerCount++;
        screen.isOccupied = true;

        Debug.Log($"[Spawner] Spawned {newCust.name} | Total: {currentCustomerCount}/{maxTotalCustomers}");
    }

    private void DecrementCount()
    {
        currentCustomerCount = Mathf.Max(0, currentCustomerCount - 1);
        Debug.Log($"[Spawner] Customer left | Total: {currentCustomerCount}/{maxTotalCustomers}");
        TrySpawnCustomer(); // Try to fill the spot immediately
    }
}