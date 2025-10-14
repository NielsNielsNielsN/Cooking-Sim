using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class PlayerGrabber : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 2.5f;
    public Transform holdParent;
    public Vector3 holdLocalPosition = new Vector3(0.4f, -0.3f, 0.9f);
    public TextMeshProUGUI interactionText;

    public MonoBehaviour[] scriptsToDisable;

    public GameObject heldObject;
    private FoodItem heldFood;
    private FoodBag heldBag;
    private Tray hoveredTray;
    private Tray heldTray;

    private Drawer hoveredDrawer;
    private TrayDispenser hoveredTrayDispenser;
    private BowlDispenser hoveredBowlDispenser;
    private CuttingBoard hoveredCuttingBoard;
    private CoolingCell hoveredCell;
    private CookStation hoveredStation;
    private FoodItem hoveredFoodItem;
    private FoodBag hoveredBag;
    private TrashBin hoveredTrashBin;
    private Counter hoveredCounter;
    private Interactable lastInteractable;
    private Knife hoveredKnife;
    private Knife heldKnife;

    public GameObject knifePrefab; // assign the knife prefab in inspector

    public CoolingCell OpenedCoolingCell { get; set; }

    private void Start()
    {
        if (scriptsToDisable == null || scriptsToDisable.Length == 0)
        {
            PlayerLook look = GetComponentInChildren<PlayerLook>();
            if (look != null)
                scriptsToDisable = new MonoBehaviour[] { look };
        }
    }

    private void Update()
    {
        if (playerCamera == null) return;

        HandleHover();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickUp();
            else
                TryPlace();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DropHeld();
        }
    }

    private void HandleHover()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            hoveredDrawer = hit.collider.GetComponent<Drawer>();
            hoveredCell = hit.collider.GetComponent<CoolingCell>();
            hoveredStation = hit.collider.GetComponent<CookStation>();
            hoveredFoodItem = hit.collider.GetComponent<FoodItem>();
            hoveredBag = hit.collider.GetComponent<FoodBag>();
            hoveredTrashBin = hit.collider.GetComponent<TrashBin>();
            hoveredTray = hit.collider.GetComponent<Tray>();
            hoveredCounter = hit.collider.GetComponent<Counter>();
            hoveredTrayDispenser = hit.collider.GetComponent<TrayDispenser>();
            hoveredBowlDispenser = hit.collider.GetComponent<BowlDispenser>();
            hoveredCuttingBoard = hit.collider.GetComponent<CuttingBoard>();
            hoveredKnife = hit.collider.GetComponent<Knife>();

            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != lastInteractable)
            {
                lastInteractable = interactable;
            }

            if (interactionText)
                interactionText.text = interactable ? interactable.promptMessage : "";

            if (hoveredDrawer != null && DrawerUIManager.Instance != null)
                DrawerUIManager.Instance.ShowStock(hoveredDrawer);
            else if (DrawerUIManager.Instance != null)
                DrawerUIManager.Instance.HideStock();
        }
        else
        {
            hoveredDrawer = null;
            hoveredCell = null;
            hoveredStation = null;
            hoveredFoodItem = null;
            hoveredBag = null;
            hoveredTrashBin = null;
            hoveredTray = null;
            hoveredCounter = null;
            hoveredTrayDispenser = null;
            hoveredBowlDispenser = null;
            hoveredCuttingBoard = null;
            hoveredKnife = null;

            if (interactionText) interactionText.text = "";

            if (DrawerUIManager.Instance != null) DrawerUIManager.Instance.HideStock();
        }
    }

    private void TryPickUp()
    {
        // Grab from drawer
        if (hoveredDrawer != null)
        {
            GameObject instance = hoveredDrawer.TakeOne();
            if (instance != null) { Grab(instance); }
            return;
        }

        // Open CoolingCell UI
        if (hoveredCell != null)
        {
            hoveredCell.OpenMenu(this);
            return;
        }

        // Grab raw food on ground
        if (hoveredFoodItem != null)
        {
            Grab(hoveredFoodItem.gameObject);
            return;
        }

        // Grab a bag on ground
        if (hoveredBag != null)
        {
            Grab(hoveredBag.gameObject);
            return;
        }

        // Remove from cooking station
        if (hoveredStation != null)
        {
            FoodItem removed = hoveredStation.RemoveFood();
            if (removed != null) Grab(removed.gameObject);
            return;
        }

        // Grab a tray that's lying in the world
        if (hoveredTray != null && heldObject == null)
        {
            Grab(hoveredTray.gameObject);
            return;
        }

        // Take tray from counter
        if (hoveredCounter != null && heldObject == null)
        {
            Tray takenTray = hoveredCounter.TakeTray();
            if (takenTray != null) Grab(takenTray.gameObject);
            return;
        }

        // Take a tray from dispenser
        if (hoveredTrayDispenser != null)
        {
            GameObject tray = hoveredTrayDispenser.TakeTray();
            if (tray != null) Grab(tray);
            return;
        }

        // Take a bowl from bowl dispenser
        if (hoveredBowlDispenser != null)
        {
            GameObject bowl = hoveredBowlDispenser.TakeBowl();
            if (bowl != null) Grab(bowl);
            return;
        }

        // Pick up a knife object in the world
        if (hoveredKnife != null)
        {
            Grab(hoveredKnife.gameObject);
            return;
        }

        // Cutting board interactions when hand is empty (pick up knife visual or pick up item)
        if (hoveredCuttingBoard != null)
        {
            // If board has knife visual and hand empty -> pick up knife
            if (hoveredCuttingBoard.HasKnife() && heldObject == null)
            {
                hoveredCuttingBoard.SetKnife(false);
                GameObject knifeObject = Instantiate(knifePrefab, holdParent != null ? holdParent.position : playerCamera.transform.position, holdParent != null ? holdParent.rotation : playerCamera.transform.rotation);
                Grab(knifeObject);
                return;
            }

            // If board has an item and hand empty -> pick up the item
            if (hoveredCuttingBoard.currentItem != null && heldObject == null)
            {
                GameObject taken = hoveredCuttingBoard.TakeItem();
                if (taken != null) { Grab(taken); }
                return;
            }
        }
    }

    private void TryPlace()
    {
        // Trash bin
        if (hoveredTrashBin != null)
        {
            hoveredTrashBin.Interact(this);
            return;
        }

        // Place into cooking station
        if (hoveredStation != null && heldFood != null && hoveredStation.CanAccept(heldFood.foodType))
        {
            hoveredStation.PlaceFood(heldFood);
            ClearHeld();
            return;
        }

        // Refill drawer
        if (hoveredDrawer != null && heldBag != null)
        {
            if (hoveredDrawer.drawerFoodType == heldBag.bagType)
            {
                hoveredDrawer.Refill(heldBag);
                Destroy(heldBag.gameObject);
                ClearHeld();
            }
            return;
        }

        // Place tray on counter
        if (heldTray != null && hoveredCounter != null)
        {
            hoveredCounter.AcceptTray(heldTray);
            ClearHeld();
            return;
        }

        // Place food into bun or bowl
        var hoveredBun = hoveredTray == null ? hoveredFoodItem?.GetComponent<HotdogBun>() : null;
        var hoveredBowl = hoveredTray == null ? hoveredFoodItem?.GetComponent<FriesBowl>() : null;

        if (hoveredBun != null && heldFood != null && heldFood.foodType == FoodType.Hotdog)
        {
            if (hoveredBun.AddFood(heldFood))
            {
                ClearHeld();
                return;
            }
        }

        if (hoveredBowl != null && heldFood != null && heldFood.foodType == FoodType.Fries)
        {
            if (hoveredBowl.AddFood(heldFood))
            {
                ClearHeld();
                return;
            }
        }

        // Place on tray
        if (hoveredTray != null && heldObject != null)
        {
            var bun = heldObject.GetComponent<HotdogBun>();
            var bowl = heldObject.GetComponent<FriesBowl>();
            var foodItem = heldObject.GetComponent<FoodItem>();
            bool placed = false;
            if (bun != null) placed = hoveredTray.AddBun(bun);
            else if (bowl != null) placed = hoveredTray.AddBowl(bowl);
            else if (foodItem != null) placed = hoveredTray.AddFood(foodItem);
            if (placed) { ClearHeld(); return; }
        }

        // Cutting board interactions when holding something
        if (hoveredCuttingBoard != null)
        {
            // If holding a knife and board has an item -> cut
            if (heldKnife != null && hoveredCuttingBoard.currentItem != null)
            {
                heldKnife.TryCut(hoveredCuttingBoard);
                return;
            }

            // If holding a knife and the knife visual is NOT active -> place knife back
            if (heldKnife != null && !hoveredCuttingBoard.HasKnife())
            {
                hoveredCuttingBoard.SetKnife(true);
                Destroy(heldObject);
                ClearHeld();
                return;
            }

            // If hand empty and board has knife visual, pickup knife - handled in TryPickUp (kept for safety if placed here)
            if (heldObject == null && hoveredCuttingBoard.HasKnife())
            {
                hoveredCuttingBoard.SetKnife(false);
                GameObject knifeObject2 = Instantiate(knifePrefab, holdParent != null ? holdParent.position : playerCamera.transform.position, holdParent != null ? holdParent.rotation : playerCamera.transform.rotation);
                Grab(knifeObject2);
                return;
            }

            // Place food on the board (when holding food and there is no current item)
            if (heldObject != null && heldFood != null && hoveredCuttingBoard.currentItem == null)
            {
                if (hoveredCuttingBoard.PlaceItem(heldObject))
                {
                    ClearHeld();
                    return;
                }
            }
        }
    }

    public void Grab(GameObject obj)
    {
        heldObject = obj;
        heldFood = obj.GetComponent<FoodItem>();
        heldBag = obj.GetComponent<FoodBag>();
        heldTray = obj.GetComponent<Tray>();
        heldKnife = obj.GetComponent<Knife>();

        var rb = heldObject.GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = true; rb.detectCollisions = false; }

        if (holdParent != null)
        {
            heldObject.transform.SetParent(holdParent, false);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            heldObject.transform.SetParent(playerCamera.transform, false);
            heldObject.transform.localPosition = holdLocalPosition;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    private void DropHeld()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        var rb = heldObject.GetComponent<Rigidbody>();
        if (rb) { rb.isKinematic = false; rb.detectCollisions = true; }

        ClearHeld();
    }

    private void ClearHeld()
    {
        heldObject = null;
        heldFood = null;
        heldBag = null;
        heldTray = null;
        heldKnife = null;
    }

    public void DisableScripts()
    {
        if (scriptsToDisable == null) return;
        foreach (var script in scriptsToDisable)
        {
            if (script) script.enabled = false;
        }
    }

    public void EnableScripts()
    {
        if (scriptsToDisable == null) return;
        foreach (var script in scriptsToDisable)
        {
            if (script) script.enabled = true;
        }
    }

    public void ThrowHeld()
    {
        if (heldObject != null)
        {
            Destroy(heldObject);
            heldObject = null;
            heldFood = null;
            heldBag = null;
            heldTray = null;
            heldKnife = null;
        }
    }
}
