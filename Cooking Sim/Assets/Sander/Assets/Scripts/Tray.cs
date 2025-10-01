using UnityEngine;
using System.Collections.Generic;

public class Tray : MonoBehaviour
{
    public Transform foodParent;

    public Transform[] friesPositions = new Transform[2];
    public Transform[] hotdogPositions = new Transform[2];
    public Transform[] drinkPositions = new Transform[2];

    private Dictionary<FoodType, Transform[]> foodPositions;
    private Dictionary<FoodType, int> counts = new Dictionary<FoodType, int>();

    public List<FoodItem> foods = new List<FoodItem>();

    private void Awake()
    {
        foodPositions = new Dictionary<FoodType, Transform[]>
        {
            { FoodType.Fries, friesPositions },
            { FoodType.Hotdog, hotdogPositions },
            { FoodType.Can, drinkPositions }
        };

        counts[FoodType.Fries] = 0;
        counts[FoodType.Hotdog] = 0;
        counts[FoodType.Can] = 0;
    }

    public void AddFood(FoodItem food)
    {
        int index = counts[food.foodType];
        if (index >= 2) return;

        foods.Add(food);
        food.transform.SetParent(foodParent != null ? foodParent : transform);
        food.transform.localRotation = Quaternion.identity;

        Transform targetPos = foodPositions[food.foodType][index];
        food.transform.position = targetPos.position;
        food.transform.rotation = targetPos.rotation;

        counts[food.foodType]++;
    }
}