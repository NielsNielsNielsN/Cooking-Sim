using UnityEngine;
using TMPro;

public class Drawer : MonoBehaviour
{
    public FoodType drawerFoodType;
    public int maxStock;
    public int currentStock;

    public GameObject foodPrefab;

    public GameObject TakeOne()
    {
        if (currentStock <= 0 || foodPrefab == null) return null;

        currentStock--;
        UpdateUI();
        return Instantiate(foodPrefab);
    }

    public void Refill(FoodBag bag)
    {
        if (bag.bagType != drawerFoodType)
        {
            return;
        }

        currentStock += bag.refillAmount;
        if (currentStock > maxStock) currentStock = maxStock;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (DrawerUIManager.Instance != null)
        {
            DrawerUIManager.Instance.ShowStock(this);
        }
    }
}
