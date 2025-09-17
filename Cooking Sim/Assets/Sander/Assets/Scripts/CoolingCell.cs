using UnityEngine;
using UnityEngine.UI;

public class CoolingCell : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuUI;
    public Button hotdogButton;
    public Button friesButton;
    public Button closeButton;

    [Header("Bag Prefabs")]
    public GameObject hotdogBagPrefab;
    public GameObject friesBagPrefab;

    private PlayerGrabber currentGrabber;

    private void Start()
    {
        // Start hidden
        if (menuUI != null) menuUI.SetActive(false);

        // Link buttons
        if (hotdogButton != null)
            hotdogButton.onClick.AddListener(SpawnHotdogBag);

        if (friesButton != null)
            friesButton.onClick.AddListener(SpawnFriesBag);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseMenu);
    }

    public void OpenMenu(PlayerGrabber grabber)
    {
        currentGrabber = grabber;

        if (menuUI) menuUI.SetActive(true);

        if (currentGrabber != null)
            currentGrabber.DisableScripts();

        // Unlock cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        if (menuUI) menuUI.SetActive(false);

        if (currentGrabber != null)
        {
            currentGrabber.EnableScripts();
            currentGrabber.OpenedCoolingCell = null;
        }

        currentGrabber = null;

        // Lock cursor back for player control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SpawnHotdogBag()
    {
        if (currentGrabber == null || hotdogBagPrefab == null) return;

        GameObject bag = Instantiate(hotdogBagPrefab);
        currentGrabber.Grab(bag);
        CloseMenu();
    }

    public void SpawnFriesBag()
    {
        if (currentGrabber == null || friesBagPrefab == null) return;

        GameObject bag = Instantiate(friesBagPrefab);
        currentGrabber.Grab(bag);
        CloseMenu();
    }

    private void Update()
    {
        // Optional: close menu with Escape
        if (menuUI != null && menuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}
