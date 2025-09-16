using UnityEngine;

public class Drawer : MonoBehaviour
{
    public GameObject foodPrefab;
    public int stock = 5;

    public GameObject TakeOne()
    {
        if (stock <= 0) return null;
        stock--;
        return Instantiate(foodPrefab);
    }

    public void Refill(int amount)
    {
        stock += amount;
    }
}
