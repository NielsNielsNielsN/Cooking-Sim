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
        if (menuUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}
