using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomerOrder : MonoBehaviour
{
    // Only food types customers can order (exclude Buns)
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
    private Queue<Customer> customerQueue = new Queue<Customer>();

    public Tray tray;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TryCompleteNextOrder();
        }
    }

    public string GenerateOrder()
    {
        if (activeOrders >= maxOrders)
            return null;

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

        // Spawn UI card
        GameObject newCard = Instantiate(orderCardPrefab, ordersContainer);
        TextMeshProUGUI orderText = newCard.GetComponentInChildren<TextMeshProUGUI>();
        orderText.text = orderString.TrimEnd();

        // Position cards horizontally
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

    private void TryCompleteNextOrder()
    {
        if (activeOrderCards.Count == 0 || customerQueue.Count == 0)
            return;

        if (IsTrayOrderCorrect())
        {
            CompleteNextOrder();
            tray.ClearTray();
        }
    }

    private void CompleteNextOrder()
    {
        GameObject firstCard = activeOrderCards[0];
        Destroy(firstCard);
        activeOrderCards.RemoveAt(0);
        activeOrders--;

        // Shift UI cards left
        for (int i = 0; i < activeOrderCards.Count; i++)
        {
            RectTransform cardRect = activeOrderCards[i].GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2(i * cardSpacing, 0f);
        }

        // Mark customer as complete
        Customer c = customerQueue.Dequeue();
        c.CompleteOrder();
    }

    private bool IsTrayOrderCorrect()
    {
        if (tray == null) return false;

        bool hasCookedHotdog = false;
        bool hasOpenBun = false;
        bool hasCookedFriesInBowl = false;
        bool drinksPresent = false;

        foreach (GameObject obj in tray.placedObjects)
        {
            if (obj == null) continue;

            // Check for Fries inside a bowl
            FriesBowl bowl = obj.GetComponent<FriesBowl>();
            if (bowl != null && bowl.friesPosition != null)
            {
                FoodItem fries = bowl.GetComponentInChildren<FoodItem>();
                if (fries != null && fries.foodType == FoodType.Fries && fries.state == CookState.Cooked)
                {
                    hasCookedFriesInBowl = true;
                }
                continue; // skip to next object
            }

            FoodItem food = obj.GetComponent<FoodItem>();
            if (food == null) continue;

            switch (food.foodType)
            {
                case FoodType.Hotdog:
                    if (food.state == CookState.Cooked)
                        hasCookedHotdog = true;
                    break;

                case FoodType.OpenBun:
                    hasOpenBun = true;
                    break;

                case FoodType.Can:
                case FoodType.Tigercan:
                case FoodType.Milkshake:
                    drinksPresent = true;
                    break;
            }
        }

        // Correct order: cooked hotdog inside open bun, cooked fries in a bowl, and drinks present
        bool hotdogCorrect = hasCookedHotdog && hasOpenBun;
        return hotdogCorrect && hasCookedFriesInBowl && drinksPresent;
    }
}
