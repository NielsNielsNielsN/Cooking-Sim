using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class PlayerGrabber : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance;
    public Transform holdParent;
    public Vector3 holdLocalPosition;
    public TextMeshProUGUI interactionText;

    public MonoBehaviour[] scriptsToDisable;

    public GameObject heldObject;
    private FoodItem heldFood;
    private FoodBag heldBag;
    private Tray hoveredTray;
    private Tray heldTray;

    private Drawer hoveredDrawer;
    private TrayDispenser hoveredTrayDispenser;
    private CoolingCell hoveredCell;
    private CookStation hoveredStation;
    private FoodItem hoveredFoodItem;
    private FoodBag hoveredBag;
    private TrashBin hoveredTrashBin;
    private Counter hoveredCounter;
    private Interactable lastInteractable;

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


            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != lastInteractable) lastInteractable = interactable;

            if (interactionText) interactionText.text = interactable ? interactable.promptMessage : "";

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


            lastInteractable = null;
            if (interactionText) interactionText.text = "";

            if (DrawerUIManager.Instance != null) DrawerUIManager.Instance.HideStock();
        }
    }

    private void TryPickUp()
    {
        if (hoveredDrawer != null)
        {
            GameObject instance = hoveredDrawer.TakeOne();
            if (instance != null) Grab(instance);
            return;
        }

        if (hoveredCell != null)
        {
            hoveredCell.OpenMenu(this);
            return;
        }

        if (hoveredFoodItem != null)
        {
            Grab(hoveredFoodItem.gameObject);
            return;
        }

        if (hoveredBag != null)
        {
            Grab(hoveredBag.gameObject);
            return;
        }

        if (hoveredStation != null)
        {
            FoodItem removed = hoveredStation.RemoveFood();
            if (removed != null) Grab(removed.gameObject);
            return;
        }

        if (hoveredTray != null)
        {
            if (heldObject == null)
            {
                Grab(hoveredTray.gameObject);
            }
            return;
        }

        if (hoveredCounter != null)
        {
            if (heldObject == null)
            {
                Tray takenTray = hoveredCounter.TakeTray();
                if (takenTray != null) Grab(takenTray.gameObject);
            }
            return;
        }

        if (hoveredTrayDispenser != null)
        {
            GameObject tray = hoveredTrayDispenser.TakeTray();
            if (tray != null) Grab(tray);
            return;
        }

    }

    private void TryPlace()
    {
        if (hoveredTrashBin != null)
        {
            hoveredTrashBin.Interact(this);
            return;
        }

        if (hoveredStation != null && heldFood != null && hoveredStation.CanAccept(heldFood.foodType))
        {
            hoveredStation.PlaceFood(heldFood);
            ClearHeld();
            return;
        }

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

        if (heldTray != null && hoveredCounter != null)
        {
            hoveredCounter.AcceptTray(heldTray);
            ClearHeld();
            return;
        }

        if (hoveredTray != null && heldFood != null)
        {
            if (hoveredTray.AddFood(heldFood))
            {
                ClearHeld();
            }
            return;
        }
    }

    public void Grab(GameObject obj)
    {
        heldObject = obj;
        heldFood = obj.GetComponent<FoodItem>();
        heldBag = obj.GetComponent<FoodBag>();
        heldTray = obj.GetComponent<Tray>();

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
        }
    }
}
