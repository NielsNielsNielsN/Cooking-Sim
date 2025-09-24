using UnityEngine;
using TMPro;

public class DrawerUIManager : MonoBehaviour
{
    public static DrawerUIManager Instance;

    public GameObject stockPanel;
    public TextMeshProUGUI stockText;

    private void Awake()
    {
        Instance = this;

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
        stockPanel.SetActive(false);
    }
}
