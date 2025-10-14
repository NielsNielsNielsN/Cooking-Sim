using UnityEngine;
using UnityEngine.UI;

public class CoolingCell : MonoBehaviour
{
    public GameObject menuUI;
    public Button hotdogButton;
    public Button friesButton;
    public Button bunButton;
    public Button cansButton;
    public Button closeButton;

    public GameObject hotdogBagPrefab;
    public GameObject friesBagPrefab;
    public GameObject bunBagPrefab;
    public GameObject cansBagPrefab;
    public GameObject stockBoxPrefab; // box that spawns when buying stock

    private PlayerGrabber currentGrabber;

    private void Start()
    {
        menuUI.SetActive(false);

        hotdogButton.onClick.AddListener(SpawnHotdogBag);
        friesButton.onClick.AddListener(SpawnFriesBag);
        bunButton.onClick.AddListener(SpawnBunBag);
        cansButton.onClick.AddListener(SpawnCanBag);
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
            return;
        }

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
            return;
        }

        GameObject bag = Instantiate(friesBagPrefab);
        currentGrabber.Grab(bag);
        GameManager.Instance.friesStock--;
        CloseMenu();
    }

    public void SpawnBunBag()
    {
        // Check if there’s enough stock
        if (currentGrabber == null || bunBagPrefab == null) return;
        if (GameManager.Instance.bunsStock <= 0)
        {
            return;
        }

        GameObject bag = Instantiate(bunBagPrefab);
        currentGrabber.Grab(bag);
        GameManager.Instance.bunsStock--;
        CloseMenu();
    }

    public void SpawnCanBag()
    {
        // Check if there’s enough stock
        if (currentGrabber == null || cansBagPrefab == null) return;
        if (GameManager.Instance.cansStock <= 0)
        {
            return;
        }

        GameObject bag = Instantiate(cansBagPrefab);
        currentGrabber.Grab(bag);
        GameManager.Instance.cansStock--;
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
