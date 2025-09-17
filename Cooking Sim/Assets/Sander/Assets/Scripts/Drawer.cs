using UnityEngine;
using TMPro;

public class Drawer : MonoBehaviour
{
    public FoodType drawerFoodType;     // Hotdog or Fries
    public int maxStock = 10;
    public int currentStock = 5;

    [Header("Food Prefab")]
    public GameObject foodPrefab;       // Raw food prefab to spawn

    // Take one piece of food
    public GameObject TakeOne()
    {
        if (currentStock <= 0 || foodPrefab == null) return null;

        currentStock--;
        UpdateUI();
        return Instantiate(foodPrefab);
    }

    // Refill drawer with a bag
    public void Refill(FoodBag bag)
    {
        if (bag == null) return;

        if (bag.bagType != drawerFoodType)
        {
            Debug.Log("Cannot refill this drawer with this bag type.");
            return;
        }

        currentStock += bag.refillAmount;
        if (currentStock > maxStock) currentStock = maxStock;

        UpdateUI();
    }

    // Update the DrawerUIManager
    private void UpdateUI()
    {
        if (DrawerUIManager.Instance != null)
        {
            DrawerUIManager.Instance.ShowStock(this);
        }
    }
}
