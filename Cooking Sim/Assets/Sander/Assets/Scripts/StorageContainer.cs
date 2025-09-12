using UnityEngine;

public class StorageContainer : MonoBehaviour, IInteractable, IPlacementZone
{
    public FoodType foodType;
    public int maxStock = 10;
    public GameObject hotdogPrefab;
    public GameObject friesPrefab;
    private int currentStock = 10;

    public GameObject Pickup(Transform holder)
    {
        return null; // Not pickup-able itself
    }

    public bool TryDispense(out GameObject food)
    {
        if (currentStock > 0)
        {
            currentStock--;
            food = Instantiate(GetFoodPrefab(foodType), transform.position + Vector3.up, Quaternion.identity);
            return true;
        }
        food = null;
        return false;
    }

    public bool CanPlace(GameObject item)
    {
        RefillBag bag = item.GetComponent<RefillBag>();
        return bag != null && ((bag.bagType == FoodType.RefillBagHotdog && foodType == FoodType.Hotdog) ||
                              (bag.bagType == FoodType.RefillBagFries && foodType == FoodType.Fries));
    }

    public void Place(GameObject item)
    {
        RefillBag bag = item.GetComponent<RefillBag>();
        Refill(bag.refillAmount);
        Destroy(item);
    }

    public void Refill(int amount)
    {
        currentStock = Mathf.Min(currentStock + amount, maxStock);
    }

    private GameObject GetFoodPrefab(FoodType type)
    {
        return type == FoodType.Hotdog ? hotdogPrefab : friesPrefab;
    }
}