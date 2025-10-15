using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Money")]
    public float playerMoney = 100f;

    [Header("Stock")]
    public int maxStock = 20;
    public int hotdogStock = 5;
    public int friesStock = 5;
    public int cansStock = 5;
    public int bunsStock = 5;

    [Header("Costs (for refilling)")]
    public float hotdogCost = 20f;
    public float friesCost = 15f;
    public float cansCost = 10f;
    public float bunsCost = 25f;

    [Header("Sell prices (when customer buys)")]
    public float hotdogSellPrice = 15f;
    public float friesSellPrice = 10f;
    public float cansSellPrice = 8f;
    public float bunsSellPrice = 20f;

    private void Awake()
    {
        // --- Singleton setup ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Clamp stock values
        hotdogStock = Mathf.Clamp(hotdogStock, 0, maxStock);
        friesStock = Mathf.Clamp(friesStock, 0, maxStock);
        cansStock = Mathf.Clamp(cansStock, 0, maxStock);
        bunsStock = Mathf.Clamp(bunsStock, 0, maxStock);
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
    public void AddStock(string item, int amount = 1)
    {
        if (string.IsNullOrEmpty(item)) return;

        switch (item.ToLower())
        {
            case "hotdog":
            case "hotdogs":
                hotdogStock = Mathf.Min(maxStock, hotdogStock + amount);
                break;

            case "fries":
                friesStock = Mathf.Min(maxStock, friesStock + amount);
                break;

            case "cans":
            case "can":
                cansStock = Mathf.Min(maxStock, cansStock + amount);
                break;

            case "bun":
            case "buns":
                bunsStock = Mathf.Min(maxStock, bunsStock + amount);
                break;

            default:
                Debug.LogWarning($"GameManager.AddStock: unknown item '{item}'");
                break;
        }
    }

    // ----- Selling -----
    public void SellItem(string item)
    {
        if (string.IsNullOrEmpty(item)) return;

        float earned = 0f;

        switch (item.ToLower())
        {
            case "hotdog":
            case "hotdogs":
                if (hotdogStock <= 0) { Debug.Log("No hotdog stock to sell."); return; }
                hotdogStock--;
                earned = hotdogSellPrice;
                break;

            case "fries":
                if (friesStock <= 0) { Debug.Log("No fries stock to sell."); return; }
                friesStock--;
                earned = friesSellPrice;
                break;

            case "cans":
            case "can":
                if (cansStock <= 0) { Debug.Log("No cans stock to sell."); return; }
                cansStock--;
                earned = cansSellPrice;
                break;

            case "bun":
            case "buns":
                if (bunsStock <= 0) { Debug.Log("No buns stock to sell."); return; }
                bunsStock--;
                earned = bunsSellPrice;
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
