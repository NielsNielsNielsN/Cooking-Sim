using UnityEngine;
using TMPro;

public class Drawer : MonoBehaviour
{
    public FoodType drawerFoodType;
    public int maxStock;
    public int currentStock;

    public GameObject foodPrefab;
    public GameObject[] physicalItems;

    private void Start()
    {
        ShufflePhysicalItems();
        UpdateVisuals();
    }

    public GameObject TakeOne()
    {
        if (currentStock <= 0 || foodPrefab == null) return null;

        currentStock--;
        UpdateVisuals();
        UpdateUI();

        return Instantiate(foodPrefab);
    }

    public void Refill(FoodBag bag)
    {
        if (bag.bagType != drawerFoodType) return;

        currentStock += bag.refillAmount;
        if (currentStock > maxStock) currentStock = maxStock;

        ShufflePhysicalItems();
        UpdateVisuals();
        UpdateUI();
    }

    private void UpdateVisuals()
    {
        if (physicalItems == null) return;

        for (int i = 0; i < physicalItems.Length; i++)
        {
            if (physicalItems[i] != null)
                physicalItems[i].SetActive(i < currentStock);
        }
    }

    private void ShufflePhysicalItems()
    {
        if (physicalItems == null || physicalItems.Length == 0) return;

        for (int i = 0; i < physicalItems.Length; i++)
        {
            int rnd = Random.Range(i, physicalItems.Length);
            GameObject temp = physicalItems[i];
            physicalItems[i] = physicalItems[rnd];
            physicalItems[rnd] = temp;
        }
    }

    private void UpdateUI()
    {
        if (DrawerUIManager.Instance != null)
            DrawerUIManager.Instance.ShowStock(this);
    }
}
