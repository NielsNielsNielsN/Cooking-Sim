using UnityEngine;
using System.Collections.Generic;

public class FriesBowl : MonoBehaviour
{
    public Transform foodParent;
    public Transform friesPosition;
    private FoodItem containedFries;

    public bool AddFood(FoodItem food)
    {
        if (containedFries != null || food.foodType != FoodType.Fries) return false;

        containedFries = food;
        food.transform.SetParent(foodParent != null ? foodParent : transform);
        food.transform.position = friesPosition.position;
        food.transform.rotation = friesPosition.rotation;
        return true;
    }
}
