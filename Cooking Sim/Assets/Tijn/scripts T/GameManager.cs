using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Money")]
    public float money = 100f;

    [Header("Stock Settings")]
    public int maxStock = 5;
    public int hotdogStock;
    public int friesStock;
    public int cansStock;
    public int milkshakeStock;

    [Header("Item Buy Costs (for refilling stock)")]
    public float hotdogCost = 10f;
    public float friesCost = 8f;
    public float cansCost = 6f;
    public float milkshakeCost = 12f;

    [Header("Item Sell Prices (customer orders)")]
    public float hotdogSellPrice = 18f;
    public float friesSellPrice = 15f;
    public float cansSellPrice = 10f;
    public float milkshakeSellPrice = 20f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initialize all stock to max at game start
        hotdogStock = maxStock;
        friesStock = maxStock;
        cansStock = maxStock;
        milkshakeStock = maxStock;
    }

    public bool CanAfford(float cost) => money >= cost;

    public void SpendMoney(float amount)
    {
        money = Mathf.Max(0, money - amount);
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }

    public void AddStock(string item)
    {
        switch (item.ToLower())
        {
            case "hotdog":
            case "hotdogs":
                hotdogStock = Mathf.Min(maxStock, hotdogStock + 1);
                break;
            case "fries":
                friesStock = Mathf.Min(maxStock, friesStock + 1);
                break;
            case "cans":
                cansStock = Mathf.Min(maxStock, cansStock + 1);
                break;
            case "milkshake":
            case "milkshakes":
                milkshakeStock = Mathf.Min(maxStock, milkshakeStock + 1);
                break;
        }
    }

    public void SellItem(string item)
    {
        float profit = 0;

        switch (item.ToLower())
        {
            case "hotdog":
            case "hotdogs":
                profit = hotdogSellPrice;
                hotdogStock = Mathf.Max(0, hotdogStock - 1);
                break;
            case "fries":
                profit = friesSellPrice;
                friesStock = Mathf.Max(0, friesStock - 1);
                break;
            case "cans":
                profit = cansSellPrice;
                cansStock = Mathf.Max(0, cansStock - 1);
                break;
            case "milkshake":
            case "milkshakes":
                profit = milkshakeSellPrice;
                milkshakeStock = Mathf.Max(0, milkshakeStock - 1);
                break;
        }

        if (profit > 0)
        {
            money += profit;
            Debug.Log($"Sold {item} for ${profit}. Total money: ${money}");
        }
    }
}
