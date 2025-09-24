using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrderSystem : MonoBehaviour
{
    private string[] menuItems = { "Hotdog", "Milkshake", "Fries", "Popcorn", "Chips", "Drinks" };

    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    [SerializeField] private TextMeshProUGUI orderScreenText;

    private int customerNumber = 1;

    public int activeOrders = 0;           // NEW
    public int maxOrders = 5;              // NEW

    public void GenerateOrder()
    {
        if (activeOrders >= maxOrders) return; // safety

        int itemCount = Random.Range(minItems, maxItems + 1);
        List<string> order = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string randomItem = menuItems[Random.Range(0, menuItems.Length)];
            order.Add(randomItem);
        }

        string orderString = $"Customer {customerNumber}:\n - " + string.Join("\n - ", order);
        Debug.Log(orderString);

        orderScreenText.text += (orderScreenText.text.Length > 0 ? "\n\n" : "") + orderString;

        customerNumber++;
        activeOrders++; // increase active orders
    }

    public void CompleteOrder()
    {
        if (activeOrders > 0)
            activeOrders--; // decrease when customer leaves pickup
    }
}
