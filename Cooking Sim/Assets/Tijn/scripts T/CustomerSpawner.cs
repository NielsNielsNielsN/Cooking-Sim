using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private OrderSystem orderSystem;
    [SerializeField] private OrderScreenPoint[] orderScreenPoints;
    [SerializeField] private Transform[] waitingSpots;
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private Transform exitPoint;

    private float spawnDelay = 1f; // check every second
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
        // Find a free order screen
        foreach (var screen in orderScreenPoints)
        {
            if (!screen.isOccupied)
            {
                SpawnCustomer(screen);
                return; // only spawn one at a time
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

        screen.isOccupied = true;
    }
}
