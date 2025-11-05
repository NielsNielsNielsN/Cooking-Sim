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
    public float milkshakeSellPrice = 20f;

    [SerializeField] private bool hasLost;
    [SerializeField] private Canvas loseCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        hotdogStock = Mathf.Clamp(hotdogStock, 0, maxStock);
        friesStock = Mathf.Clamp(friesStock, 0, maxStock);
        cansStock = Mathf.Clamp(cansStock, 0, maxStock);
        bunsStock = Mathf.Clamp(bunsStock, 0, maxStock);
    }

    public bool CanAfford(float cost) => playerMoney >= cost;
    public void SpendMoney(float amount) => playerMoney = Mathf.Max(0f, playerMoney - amount);
    public void AddMoney(float amount) => playerMoney += amount;

    public void AddStock(string item, int amount = 1)
    {
        if (string.IsNullOrEmpty(item)) return;
        switch (item.ToLower())
        {
            case "hotdog": case "hotdogs": hotdogStock = Mathf.Min(maxStock, hotdogStock + amount); break;
            case "fries": friesStock = Mathf.Min(maxStock, friesStock + amount); break;
            case "cans": case "can": cansStock = Mathf.Min(maxStock, cansStock + amount); break;
            case "bun": case "buns": bunsStock = Mathf.Min(maxStock, bunsStock + amount); break;
            default: Debug.LogWarning($"GameManager.AddStock: unknown item '{item}'"); break;
        }
    }

    private void Update()
    {
        if (playerMoney < 1f && !hasLost)
        {
            hasLost = true;
            Debug.Log("You Lose");
            if (loseCanvas != null) loseCanvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SellTrayContents(Tray tray)
    {
        if (tray == null) return;

        int hotdogs = tray.counts[FoodType.Hotdog];
        int fries = tray.counts[FoodType.Fries];
        int cans = tray.counts[FoodType.Can];
        int buns = tray.counts[FoodType.Hotdog];

        float earned = 0f;

        if (hotdogs > 0 && hotdogStock >= hotdogs) { hotdogStock -= hotdogs; earned += hotdogs * hotdogSellPrice; }
        if (fries > 0 && friesStock >= fries) { friesStock -= fries; earned += fries * friesSellPrice; }
        if (cans > 0 && cansStock >= cans) { cansStock -= cans; earned += cans * cansSellPrice; }
        if (buns > 0 && bunsStock >= buns) { bunsStock -= buns; earned += buns * milkshakeSellPrice; }

        if (earned > 0f)
        {
            AddMoney(earned);
            Debug.Log($"[GameManager] SOLD TRAY → +${earned:F2} | Money: ${playerMoney:F2}");
        }
    }
}