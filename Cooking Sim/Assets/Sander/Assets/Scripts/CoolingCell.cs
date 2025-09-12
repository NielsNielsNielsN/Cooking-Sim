using UnityEngine;

public class CoolingCell : MonoBehaviour, IInteractable
{
    public GameObject refillBagHotdogPrefab;
    public GameObject refillBagFriesPrefab;

    public GameObject Pickup(Transform holder)
    {
        // Dispense bag based on input or context (e.g., raycast tag or player choice)
        FoodType bagType = Random.value > 0.5f ? FoodType.RefillBagHotdog : FoodType.RefillBagFries; // Placeholder logic
        GameObject bag = Instantiate(bagType == FoodType.RefillBagHotdog ? refillBagHotdogPrefab : refillBagFriesPrefab,
                                    transform.position + Vector3.up, Quaternion.identity);
        bag.transform.SetParent(holder);
        bag.transform.localPosition = Vector3.forward * 0.5f;
        return bag;
    }
}