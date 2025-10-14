using UnityEngine;

public class OrderUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject orderPanel; // The whole order UI panel
    public OrderSystem orderSystem; // Reference to your existing OrderSystem

    private bool isOpen = false;

    private void Start()
    {
        orderPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleOrderPanel();
        }
    }

    private void ToggleOrderPanel()
    {
        isOpen = !isOpen;
        orderPanel.SetActive(isOpen);

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
