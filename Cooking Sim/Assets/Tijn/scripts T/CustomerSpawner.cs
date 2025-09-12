using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCustomer), 2f, spawnInterval);
    }

    void SpawnCustomer()
    {
        Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
    }
}
