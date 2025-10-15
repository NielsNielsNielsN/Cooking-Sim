using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class OrderSystem : MonoBehaviour
{
    private string[] menuItems = { "Hotdog", "Buns", "Fries", "Drinks" };

    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    [Header("UI References")]
    [SerializeField] private GameObject orderCardPrefab;  // The UI card prefab
    [SerializeField] private RectTransform ordersContainer;   // Parent object in UI for all cards
    [SerializeField] private float cardSpacing = 250f;    // Horizontal spacing between cards

    public int maxOrders = 5;
    public int activeOrders = 0;

    private int customerNumber = 1;

    private List<GameObject> activeOrderCards = new List<GameObject>();
    private Queue<Customer> customerQueue = new Queue<Customer>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CompleteNextOrder();
        }
    }

    public string GenerateOrder()
    {
        if (activeOrders >= maxOrders)
            return null;

        int itemCount = Random.Range(minItems, maxItems + 1);
        List<string> order = new List<string>();

        for (int i = 0; i < itemCount; i++)
        {
            string randomItem = menuItems[Random.Range(0, menuItems.Length)];
            order.Add(randomItem);
        }

        string orderString = $"Customer {customerNumber}:\n - " + string.Join("\n - ", order);

        // Spawn a new order card
        GameObject newCard = Instantiate(orderCardPrefab, ordersContainer);
        TextMeshProUGUI orderText = newCard.GetComponentInChildren<TextMeshProUGUI>();
        orderText.text = orderString;

        // Position cards horizontally next to each other
        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition = new Vector2(activeOrderCards.Count * cardSpacing, 0f);

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
        if (activeOrderCards.Count == 0 || customerQueue.Count == 0)
            return;

        // Remove the first order from the list
        GameObject firstCard = activeOrderCards[0];
        Destroy(firstCard);
        activeOrderCards.RemoveAt(0);
        activeOrders--;

        // Shift remaining cards to fill the empty space
        for (int i = 0; i < activeOrderCards.Count; i++)
        {
            RectTransform cardRect = activeOrderCards[i].GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2(i * cardSpacing, 0f);
        }

        // Tell the first customer to complete
        Customer c = customerQueue.Dequeue();
        c.CompleteOrder();
    }
}
