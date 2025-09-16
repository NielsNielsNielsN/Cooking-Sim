using UnityEngine;
using TMPro;

public class DrawerUIManager : MonoBehaviour
{
    public static DrawerUIManager Instance;

    public GameObject panel;         // UI panel with background
    public TextMeshProUGUI stockText; // Text field inside panel

    private void Awake()
    {
        Instance = this;
        if (panel) panel.SetActive(false);
    }

    public void ShowStock(Drawer drawer)
    {
        if (panel == null || stockText == null) return;

        stockText.text = $"Stock: {drawer.stock}";
        panel.SetActive(true);
    }

    public void HideStock()
    {
        if (panel) panel.SetActive(false);
    }
}
