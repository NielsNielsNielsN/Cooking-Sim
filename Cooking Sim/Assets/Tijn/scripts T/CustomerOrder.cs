using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrderSystem : MonoBehaviour
{
    private string[] menuItems = { "Hotdog", "Milkshake", "Fries", "Popcorn", "Chips", "Drinks" };

    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    [Header("UI")]
    [SerializeField] private GameObject ordersMenu;      // Panel that pops up with Tab
    [SerializeField] private TextMeshProUGUI ordersText; // Text inside the panel

    public int maxOrders = 5;
    public int activeOrders = 0;

    private int customerNumber = 1;

    private List<string> activeOrderList = new List<string>();
    private Queue<Customer> customerQueue = new Queue<Customer>();

    private void Update()
    {
        // Toggle UI menu with Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ordersMenu.SetActive(!ordersMenu.activeSelf);
        }

        // For testing: complete next order with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            CompleteNextOrder();
        }
    }

    public string GenerateOrder()
    {
        int itemCount = Random.Range(minItems, maxItems + 1);
        List<string> order = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string randomItem = menuItems[Random.Range(0, menuItems.Length)];
            order.Add(randomItem);
        }

        string orderString = $"Customer {customerNumber}:\n - " + string.Join("\n - ", order);

        activeOrderList.Add(orderString);
        activeOrders++;
        RefreshOrdersUI();

        customerNumber++;
        return orderString;
    }

    public void RegisterCustomerOrder(string order, Customer customer)
    {
        customerQueue.Enqueue(customer);
    }

    private void CompleteNextOrder()
    {
        if (activeOrderList.Count == 0 || customerQueue.Count == 0) return;

        string firstOrder = activeOrderList[0];
        activeOrderList.RemoveAt(0);
        activeOrders--;

        Customer c = customerQueue.Dequeue();
        c.CompleteOrder();

        RefreshOrdersUI();
    }

    private void RefreshOrdersUI()
    {
        ordersText.text = string.Join("\n\n", activeOrderList);
    }
}
