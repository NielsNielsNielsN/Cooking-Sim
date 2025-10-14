using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RefillMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject menuUI;
    public TMP_Text moneyText;

    [Header("Stock Displays")]
    public TMP_Text hotdogText;
    public TMP_Text friesText;
    public TMP_Text cansText;
    public TMP_Text bunsText;

    [Header("Buttons")]
    public Button buyHotdogButton;
    public Button buyFriesButton;
    public Button buyCansButton;
    public Button buyBunsButton;
    public Button closeButton;

    private bool isOpen = false;

    private void Start()
    {
        menuUI.SetActive(false);

        buyHotdogButton.onClick.AddListener(() => BuyItem("hotdogs", GameManager.Instance.hotdogCost));
        buyFriesButton.onClick.AddListener(() => BuyItem("fries", GameManager.Instance.friesCost));
        buyCansButton.onClick.AddListener(() => BuyItem("cans", GameManager.Instance.cansCost));
        buyBunsButton.onClick.AddListener(() => BuyItem("buns", GameManager.Instance.bunsCost));

        closeButton.onClick.AddListener(CloseMenu);
    }

    private void Update()
    {
        if (isOpen)
        {
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.Escape))
                CloseMenu();
        }
    }

    private void UpdateUI()
    {
        moneyText.text = $"Money: ${GameManager.Instance.playerMoney:F0}";
        hotdogText.text = $"Hotdogs: {GameManager.Instance.hotdogStock}/{GameManager.Instance.maxStock}";
        friesText.text = $"Fries: {GameManager.Instance.friesStock}/{GameManager.Instance.maxStock}";
        cansText.text = $"Cans: {GameManager.Instance.cansStock}/{GameManager.Instance.maxStock}";
        bunsText.text = $"Buns: {GameManager.Instance.bunsStock}/{GameManager.Instance.maxStock}";
    }

    private void BuyItem(string item, float cost)
    {
        if (!GameManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough money!");
            return;
        }

        GameManager.Instance.SpendMoney(cost);

        //  Add exactly ONE item instead of 5
        GameManager.Instance.AddStock(item, 1);

        UpdateUI();
    }

    public void OpenMenu()
    {
        isOpen = true;
        menuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        isOpen = false;
        menuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
