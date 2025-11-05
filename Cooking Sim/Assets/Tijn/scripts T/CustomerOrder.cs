using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomerOrder : MonoBehaviour
{
    private FoodType[] orderableItems = { FoodType.Hotdog, FoodType.Fries, FoodType.Can, FoodType.Tigercan, FoodType.Milkshake };
    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 6;

    [Header("UI References")]
    [SerializeField] private GameObject orderCardPrefab;
    [SerializeField] private RectTransform ordersContainer;
    [SerializeField] private float cardSpacing = 250f;

    public int maxOrders = 5;
    public int activeOrders = 0;
    private int customerNumber = 1;

    private List<GameObject> activeOrderCards = new List<GameObject>();

    // Maps order string to customer (for safety)
    private readonly Dictionary<string, Customer> _orderToCustomer = new Dictionary<string, Customer>();

    public string GenerateOrder(Customer customer)
    {
        if (activeOrders >= maxOrders) return null;

        int itemCount = Random.Range(minItems, maxItems + 1);
        Dictionary<FoodType, int> typeCounts = new Dictionary<FoodType, int>();
        List<FoodType> orderList = new List<FoodType>();

        for (int i = 0; i < itemCount; i++)
        {
            FoodType selected;
            int attempts = 0;
            do
            {
                selected = orderableItems[Random.Range(0, orderableItems.Length)];
                attempts++;
            } while (typeCounts.ContainsKey(selected) && typeCounts[selected] >= 2 && attempts < 10);

            if (!typeCounts.ContainsKey(selected))
                typeCounts[selected] = 0;

            if (typeCounts[selected] < 2)
            {
                orderList.Add(selected);
                typeCounts[selected]++;
            }
        }

        string orderString = $"Customer {customerNumber}:\n";
        foreach (FoodType ft in orderList)
        {
            orderString += $" - {ft}\n";
        }

        // Create UI Card
        GameObject newCard = Instantiate(orderCardPrefab, ordersContainer);
        TextMeshProUGUI orderText = newCard.GetComponentInChildren<TextMeshProUGUI>();
        orderText.text = orderString.TrimEnd();

        // Link button to this customer
        OrderCardButton btn = newCard.GetComponentInChildren<OrderCardButton>();
        if (btn != null)
        {
            btn.customer = customer;
            btn.orderSystem = this;
        }

        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        cardRect.anchoredPosition = new Vector2(activeOrderCards.Count * cardSpacing, 0f);
        activeOrderCards.Add(newCard);

        // Store mapping
        _orderToCustomer[orderString] = customer;

        activeOrders++;
        customerNumber++;

        RegisterCustomerOrder(orderString, customer);
        return orderString;
    }

    public void RegisterCustomerOrder(string order, Customer customer)
    {
        // Optional: keep a queue if needed elsewhere
    }

    
    public void OrderReady(Customer customer)
    {
        if (customer == null || activeOrderCards.Count == 0) return;

        Destroy(activeOrderCards[0]);
        activeOrderCards.RemoveAt(0);
        activeOrders--;

        for (int i = 0; i < activeOrderCards.Count; i++)
        {
            RectTransform r = activeOrderCards[i].GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(i * cardSpacing, 0f);
        }

        customer.ForcePickupOrder();   
    }
}