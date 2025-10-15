using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

        buyHotdogButton.onClick.AddListener(() => BuyItem("hotdogs", GameManagerHelpers.Instance.hotdogCost));
        buyFriesButton.onClick.AddListener(() => BuyItem("fries", GameManagerHelpers.Instance.friesCost));
        buyCansButton.onClick.AddListener(() => BuyItem("cans", GameManagerHelpers.Instance.cansCost));
        buyBunsButton.onClick.AddListener(() => BuyItem("buns", GameManagerHelpers.Instance.bunsCost));

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
        moneyText.text = $"Money: ${GameManagerHelpers.Instance.playerMoney:F0}";
        hotdogText.text = $"Hotdogs: {GameManagerHelpers.Instance.hotdogStock}/{GameManagerHelpers.Instance.maxStock}";
        friesText.text = $"Fries: {GameManagerHelpers.Instance.friesStock}/{GameManagerHelpers.Instance.maxStock}";
        cansText.text = $"Cans: {GameManagerHelpers.Instance.cansStock}/{GameManagerHelpers.Instance.maxStock}";
        bunsText.text = $"Buns: {GameManagerHelpers.Instance.bunsStock}/{GameManagerHelpers.Instance.maxStock}";
    }

    private void BuyItem(string item, float cost)
    {
        if (!GameManagerHelpers.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough money!");
            return;
        }
        GameManagerHelpers.Instance.SpendMoney(cost);
        GameManagerHelpers.Instance.AddStock(item, 1);

        UpdateUI();
    }

    public void OpenMenuLaptop(PlayerGrabber grabber)
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
