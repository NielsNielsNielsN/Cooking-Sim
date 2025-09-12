using UnityEngine;
using System.Collections.Generic;

public class OrderSystem : MonoBehaviour
{
    private string[] menuItems = { "Hotdog", "Milkshake", "Fries", "Popcorn", "Chips", "Drinks" };

    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    public void GenerateOrder()
    {
        int itemCount = Random.Range(minItems, maxItems + 1);
        List<string> order = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string randomItem = menuItems[Random.Range(0, menuItems.Length)];
            order.Add(randomItem);
        }

        Debug.Log("Customer Order: " + string.Join(", ", order));
    }
}
