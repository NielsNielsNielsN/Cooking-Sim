using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrderSystem : MonoBehaviour
{
    private string[] menuItems = { "Hotdog", "Milkshake", "Fries", "Popcorn", "Chips", "Drinks" };

    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    [SerializeField] private TextMeshProUGUI orderScreenText; // in-world screen text

    private int customerNumber = 1; // keep track of how many customers ordered

    private void Update()
    {
        // Press O to generate a new order
        if (Input.GetKeyDown(KeyCode.O))
        {
            GenerateOrder();
        }
    }

    public void GenerateOrder()
    {
        int itemCount = Random.Range(minItems, maxItems + 1);
        List<string> order = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string randomItem = menuItems[Random.Range(0, menuItems.Length)];
            order.Add(randomItem);
        }

        string orderString = $"Customer {customerNumber}:\n - " + string.Join("\n - ", order);

        Debug.Log(orderString);

        // Add this order to the existing text (stacking orders)
        orderScreenText.text += (orderScreenText.text.Length > 0 ? "\n\n" : "") + orderString;

        customerNumber++;
    }
}
