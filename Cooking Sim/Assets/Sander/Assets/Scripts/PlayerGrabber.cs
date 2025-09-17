using UnityEngine;
using TMPro;

public class PlayerGrabber : MonoBehaviour
{
    [Header("General Settings")]
    public Camera playerCamera;
    public float interactDistance = 2.5f;
    public Transform holdParent;
    public Vector3 holdLocalPosition = new Vector3(0.4f, -0.3f, 0.9f);
    public TextMeshProUGUI interactionText;

    [Header("Scripts to disable during UI")]
    public MonoBehaviour[] scriptsToDisable;

    public GameObject heldObject;
    private FoodItem heldFood;
    private FoodBag heldBag;

    private Drawer hoveredDrawer;
    private CoolingCell hoveredCell;
    private CookStation hoveredStation;
    private FoodItem hoveredFoodItem;
    private FoodBag hoveredBag;
    private TrashBin hoveredTrashBin;
    private Interactable lastInteractable;

    public CoolingCell OpenedCoolingCell { get; set; }

    private void Start()
    {
        // Auto-find PlayerLook on child if scriptsToDisable empty
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

            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != lastInteractable)
            {
                if (lastInteractable) lastInteractable.Highlight(false);
                if (interactable) interactable.Highlight(true);
                lastInteractable = interactable;
            }

            if (interactionText)
                interactionText.text = interactable ? interactable.promptMessage : "";

            // Drawer stock UI
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

            if (lastInteractable) lastInteractable.Highlight(false);
            lastInteractable = null;

            if (interactionText) interactionText.text = "";

            if (DrawerUIManager.Instance != null)
                DrawerUIManager.Instance.HideStock();
        }
    }

    private void TryPickUp()
    {
        // Grab from drawer
        if (hoveredDrawer != null)
        {
            GameObject instance = hoveredDrawer.TakeOne();
            if (instance != null) Grab(instance);
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
    }

    private void TryPlace()
    {
        // TrashBin interaction
        if (hoveredTrashBin != null)
        {
            hoveredTrashBin.Interact(this);
            return;
        }

        // Place in cooking station
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

                // Destroy the bag after refill
                Destroy(heldBag.gameObject);

                ClearHeld();
            }
            else
            {
                Debug.Log("Cannot refill this drawer with this bag type.");
            }
            return;
        }

        // E does nothing if not looking at anything
    }

    public void Grab(GameObject obj)
    {
        heldObject = obj;
        heldFood = obj.GetComponent<FoodItem>();
        heldBag = obj.GetComponent<FoodBag>();

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
    }

    #region CoolingCell-compatible UI Script Control
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
        }
    }
    #endregion
}
