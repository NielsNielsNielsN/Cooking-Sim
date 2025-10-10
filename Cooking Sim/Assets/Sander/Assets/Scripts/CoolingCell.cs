using UnityEngine;
using UnityEngine.UI;

public class CoolingCell : MonoBehaviour
{
    public GameObject menuUI;
    public Button hotdogButton;
    public Button friesButton;
    public Button closeButton;

    public GameObject hotdogBagPrefab;
    public GameObject friesBagPrefab;
    public GameObject stockBoxPrefab; // box that spawns when buying stock

    private PlayerGrabber currentGrabber;

    private void Start()
    {
        menuUI.SetActive(false);

        hotdogButton.onClick.AddListener(SpawnHotdogBag);
        friesButton.onClick.AddListener(SpawnFriesBag);
        closeButton.onClick.AddListener(CloseMenu);
    }

    public void OpenMenu(PlayerGrabber grabber)
    {
        currentGrabber = grabber;
        menuUI.SetActive(true);

        currentGrabber.DisableScripts();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);

        if (currentGrabber != null)
        {
            currentGrabber.EnableScripts();
            currentGrabber.OpenedCoolingCell = null;
        }

        currentGrabber = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SpawnHotdogBag()
    {
        // Check if there’s enough stock
        if (currentGrabber == null || hotdogBagPrefab == null) return;
        if (GameManager.Instance.hotdogStock <= 0)
        {
            Debug.Log("❌ No more hotdogs in stock!");
            return;
        }

        // Spawn bag and reduce stock
        GameObject bag = Instantiate(hotdogBagPrefab);
        currentGrabber.Grab(bag);
        GameManager.Instance.hotdogStock--;
        CloseMenu();
    }

    public void SpawnFriesBag()
    {
        if (currentGrabber == null || friesBagPrefab == null) return;
        if (GameManager.Instance.friesStock <= 0)
        {
            Debug.Log("❌ No more fries in stock!");
            return;
        }

        GameObject bag = Instantiate(friesBagPrefab);
        currentGrabber.Grab(bag);
        GameManager.Instance.friesStock--;
        CloseMenu();
    }

    // Optional: when you buy stock, spawn a box prefab (visual feedback)
    public void ReceiveStockBox()
    {
        if (stockBoxPrefab != null)
        {
            Instantiate(stockBoxPrefab, transform.position + Vector3.up, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (menuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}
