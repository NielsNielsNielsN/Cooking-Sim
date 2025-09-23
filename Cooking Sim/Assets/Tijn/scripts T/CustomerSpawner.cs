using UnityEngine;
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

    private float spawnDelay = 1f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnDelay)
        {
            timer = 0f;
            TrySpawnCustomer();
        }
    }

    private void TrySpawnCustomer()
    {
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

        // Assign paths
        c.pathToOrderScreen = pathToOrderScreen;
        c.pathToPickup = pathToPickup;
        c.pathToExit = pathToExit;

        screen.isOccupied = true;
    }
}
