using UnityEngine;
using UnityEngine.UI;

public class CoolingCell : MonoBehaviour
{
    public GameObject hotdogPrefab;
    public GameObject friesPrefab;

    public GameObject menuUI;   // Assign a small UI Canvas prefab
    public Button hotdogButton;
    public Button friesButton;

    private PlayerGrabber currentPlayer;

    private void Start()
    {
        if (menuUI) menuUI.SetActive(false);

        if (hotdogButton)
            hotdogButton.onClick.AddListener(() => SpawnFood(FoodType.Hotdog));
        if (friesButton)
            friesButton.onClick.AddListener(() => SpawnFood(FoodType.Fries));
    }

    public void OpenMenu(PlayerGrabber player)
    {
        if (menuUI == null) return;
        currentPlayer = player;
        menuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SpawnFood(FoodType type)
    {
        GameObject prefab = null;
        if (type == FoodType.Hotdog) prefab = hotdogPrefab;
        if (type == FoodType.Fries) prefab = friesPrefab;

        if (prefab != null && currentPlayer != null)
        {
            GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            currentPlayer.Grab(instance);
        }

        CloseMenu();
    }

    private void CloseMenu()
    {
        if (menuUI) menuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentPlayer = null;
    }
}
