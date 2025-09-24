using UnityEngine;
using System.Collections.Generic;

public class Tray : MonoBehaviour
{
    public Transform foodParent;
    public List<FoodItem> foods = new List<FoodItem>();

    public void AddFood(FoodItem food)
    {
        foods.Add(food);
        food.transform.SetParent(foodParent != null ? foodParent : transform);
        food.transform.localPosition = Vector3.up * (foods.Count * 0.05f);
        food.transform.localRotation = Quaternion.identity;
    }
}
