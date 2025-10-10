using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Money")]
    public float playerMoney = 100f;

    [Header("Stock")]
    public int maxStock = 20;
    public int hotdogStock = 5;
    public int friesStock = 5;
    public int cansStock = 5;
    public int milkshakeStock = 5;
    public int restockAmount = 5; // how many units are added per restock

    [Header("Costs (for refilling)")]
    public float hotdogCost = 20f;
    public float friesCost = 15f;
    public float cansCost = 10f;
    public float milkshakeCost = 25f;

    [Header("Sell prices (when customer buys)")]
    public float hotdogSellPrice = 15f;
    public float friesSellPrice = 10f;
    public float cansSellPrice = 8f;
    public float milkshakeSellPrice = 20f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // optional: ensure stocks are within limits
        hotdogStock = Mathf.Clamp(hotdogStock, 0, maxStock);
        friesStock = Mathf.Clamp(friesStock, 0, maxStock);
        cansStock = Mathf.Clamp(cansStock, 0, maxStock);
        milkshakeStock = Mathf.Clamp(milkshakeStock, 0, maxStock);
    }

    // ----- Money helpers -----
    public bool CanAfford(float cost) => playerMoney >= cost;

    public void SpendMoney(float amount)
    {
        playerMoney = Mathf.Max(0f, playerMoney - amount);
    }

    public void AddMoney(float amount)
    {
        playerMoney += amount;
    }

    // ----- Stock helpers -----
    // Adds restockAmount to the named item (caps at maxStock)
    public void AddStock(string item)
    {
        if (string.IsNullOrEmpty(item)) return;
        switch (item.ToLower())
        {
            case "hotdog":
            case "hotdogs":
                hotdogStock = Mathf.Min(maxStock, hotdogStock + restockAmount);
                break;
            case "fries":
                friesStock = Mathf.Min(maxStock, friesStock + restockAmount);
                break;
            case "cans":
            case "can":
                cansStock = Mathf.Min(maxStock, cansStock + restockAmount);
                break;
            case "milkshake":
            case "milkshakes":
                milkshakeStock = Mathf.Min(maxStock, milkshakeStock + restockAmount);
                break;
            default:
                Debug.LogWarning($"GameManager.AddStock: unknown item '{item}'");
                break;
        }
    }

    // Convenience used by UI to buy stock (spend money then add stock)
    public bool BuyStock(string item, float cost)
    {
        if (!CanAfford(cost)) return false;
        SpendMoney(cost);
        AddStock(item);
        return true;
    }

    // ----- Selling: called when customer order completes -----
    // Reduces stock and gives the player money for the sold item
    public void SellItem(string item)
    {
        if (string.IsNullOrEmpty(item)) return;

        float earned = 0f;
        switch (item.ToLower())
        {
            case "hotdog":
            case "hotdogs":
                if (hotdogStock <= 0) { Debug.Log("No hotdog stock to sell."); return; }
                hotdogStock = Mathf.Max(0, hotdogStock - 1);
                earned = hotdogSellPrice;
                break;

            case "fries":
                if (friesStock <= 0) { Debug.Log("No fries stock to sell."); return; }
                friesStock = Mathf.Max(0, friesStock - 1);
                earned = friesSellPrice;
                break;

            case "cans":
            case "can":
                if (cansStock <= 0) { Debug.Log("No cans stock to sell."); return; }
                cansStock = Mathf.Max(0, cansStock - 1);
                earned = cansSellPrice;
                break;

            case "milkshake":
            case "milkshakes":
                if (milkshakeStock <= 0) { Debug.Log("No milkshake stock to sell."); return; }
                milkshakeStock = Mathf.Max(0, milkshakeStock - 1);
                earned = milkshakeSellPrice;
                break;

            default:
                Debug.LogWarning($"GameManager.SellItem: unknown item '{item}'");
                return;
        }

        if (earned > 0f)
        {
            AddMoney(earned);
            Debug.Log($"Sold {item} for ${earned:F2}. Money now: ${playerMoney:F2}");
        }
    }
}
