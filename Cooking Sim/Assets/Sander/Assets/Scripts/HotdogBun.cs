using UnityEngine;
using System.Collections.Generic;

public class HotdogBun : MonoBehaviour
{
    public Transform foodParent;
    public Transform hotdogPosition;
    private FoodItem containedHotdog;

    public bool AddFood(FoodItem food)
    {
        if (containedHotdog != null || food.foodType != FoodType.Hotdog) return false;

        containedHotdog = food;
        food.transform.SetParent(foodParent != null ? foodParent : transform);
        food.transform.position = hotdogPosition.position;
        food.transform.rotation = hotdogPosition.rotation;
        return true;
    }
}
