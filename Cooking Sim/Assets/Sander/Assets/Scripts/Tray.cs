using UnityEngine;
using System.Collections.Generic;

public class Tray : MonoBehaviour
{
    public Transform foodParent;

    public Transform[] friesPositions = new Transform[2];
    public Transform[] hotdogPositions = new Transform[2];
    public Transform[] canPositions = new Transform[2];
    public Transform[] milkshakePositions = new Transform[2];

    private Dictionary<FoodType, Transform[]> foodPositions;
    public Dictionary<FoodType, int> counts = new Dictionary<FoodType, int>();

    public List<GameObject> placedObjects = new List<GameObject>();

    private void Awake()
    {
        foodPositions = new Dictionary<FoodType, Transform[]>
        {
            { FoodType.Fries, friesPositions },
            { FoodType.Hotdog, hotdogPositions },
            { FoodType.Can, canPositions },
            { FoodType.Milkshake, milkshakePositions }
        };

        counts[FoodType.Fries] = 0;
        counts[FoodType.Hotdog] = 0;
        counts[FoodType.Can] = 0;
        counts[FoodType.Milkshake] = 0;
    }

    public bool AddFood(FoodItem food)
    {
        FoodType type = food.foodType;
        if (!foodPositions.ContainsKey(type)) return false;

        int index = counts[type];
        if (index >= foodPositions[type].Length) return false;

        Transform target = foodPositions[type][index];
        PlaceObject(food.gameObject, target);
        counts[type]++;
        return true;
    }

    public bool AddBun(HotdogBun bun)
    {
        int index = counts[FoodType.Hotdog];
        if (index >= hotdogPositions.Length) return false;

        Transform target = hotdogPositions[index];
        PlaceObject(bun.gameObject, target);
        counts[FoodType.Hotdog]++;
        return true;
    }

    public bool AddBowl(FriesBowl bowl)
    {
        int index = counts[FoodType.Fries];
        if (index >= friesPositions.Length) return false;

        Transform target = friesPositions[index];
        PlaceObject(bowl.gameObject, target);
        counts[FoodType.Fries]++;
        return true;
    }

    private void PlaceObject(GameObject obj, Transform target)
    {
        placedObjects.Add(obj);
        obj.transform.SetParent(foodParent != null ? foodParent : transform);
        obj.transform.position = target.position;
        obj.transform.rotation = target.rotation;
    }

    public void ClearTray()
    {
        foreach (GameObject obj in placedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        placedObjects.Clear();
    }

}
