using UnityEngine;
using TMPro;

public class Drawer : MonoBehaviour
{
    public GameObject foodPrefab;
    public int stock = 5;
    public TextMeshPro stockText; // world-space TextMeshPro above drawer

    private void Start()
    {
        UpdateStockUI();
    }

    public GameObject TakeOne()
    {
        if (stock <= 0) return null;
        stock--;
        UpdateStockUI();
        return Instantiate(foodPrefab);
    }

    public void Refill(int amount)
    {
        stock += amount;
        UpdateStockUI();
    }

    private void UpdateStockUI()
    {
        if (stockText) stockText.text = stock.ToString();
    }
}
