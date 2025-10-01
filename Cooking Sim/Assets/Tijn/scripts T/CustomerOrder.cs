using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrderSystem : MonoBehaviour
{
    private string[] menuItems = { "Hotdog", "Milkshake", "Fries", "Popcorn", "Chips", "Drinks" };

    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    [Header("UI")]
    [SerializeField] private GameObject ordersMenu;     // The UI panel that toggles with Tab
    [SerializeField] private Transform ordersContainer; // Parent for all order cards
    [SerializeField] private GameObject orderCardPrefab; // Prefab for a single order card

    public int maxOrders = 5;
    public int activeOrders = 0;

    private int customerNumber = 1;

    private List<GameObject> activeOrderCards = new List<GameObject>();
    private Queue<Customer> customerQueue = new Queue<Customer>();

    private void Update()
    {
        // Toggle orders menu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ordersMenu.SetActive(!ordersMenu.activeSelf);
        }

        // Complete next order with P (for testing)
        if (Input.GetKeyDown(KeyCode.P))
        {
            CompleteNextOrder();
        }
    }

    public string GenerateOrder()
    {
        // Limit to max active orders
        if (activeOrders >= maxOrders) return null;

        int itemCount = Random.Range(minItems, maxItems + 1);
        List<string> order = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string randomItem = menuItems[Random.Range(0, menuItems.Length)];
            order.Add(randomItem);
        }

        string orderString = $"Customer {customerNumber}:\n - " + string.Join("\n - ", order);

        // Create UI card
        GameObject newCard = Instantiate(orderCardPrefab, ordersContainer);
        TextMeshProUGUI textComponent = newCard.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = orderString;

        activeOrderCards.Add(newCard);
        activeOrders++;

        customerNumber++;
        return orderString;
    }

    public void RegisterCustomerOrder(string order, Customer customer)
    {
        customerQueue.Enqueue(customer);
    }

    private void CompleteNextOrder()
    {
        if (activeOrderCards.Count == 0 || customerQueue.Count == 0) return;

        // Remove first order card
        GameObject firstCard = activeOrderCards[0];
        Destroy(firstCard);
        activeOrderCards.RemoveAt(0);
        activeOrders--;

        // Notify customer
        Customer c = customerQueue.Dequeue();
        c.CompleteOrder();
    }
}
