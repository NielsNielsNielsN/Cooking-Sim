using UnityEngine;
using TMPro;

public class DrawerUIManager : MonoBehaviour
{
    public static DrawerUIManager Instance;

    [Header("UI References")]
    public GameObject stockPanel;        // Parent panel (hidden by default)
    public TextMeshProUGUI stockText;    // TMP Text for stock

    private void Awake()
    {
        Instance = this;

        // Hide panel at start
        if (stockPanel != null)
            stockPanel.SetActive(false);
    }

    public void ShowStock(Drawer drawer)
    {
        if (drawer == null || stockPanel == null || stockText == null) return;

        stockText.text = $"{drawer.currentStock}/{drawer.maxStock}";
        stockPanel.SetActive(true);
    }

    public void HideStock()
    {
        if (stockPanel != null)
            stockPanel.SetActive(false);
    }
}
