using UnityEngine;
using TMPro;

public class PlayerGrabber : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 2.5f;
    public Transform holdParent;
    public Vector3 holdLocalPosition = new Vector3(0.4f, -0.3f, 0.9f);
    public TextMeshProUGUI interactionText;

    private GameObject heldObject;
    private FoodItem heldFood;

    private Drawer hoveredDrawer;
    private CoolingCell hoveredCell;
    private CookStation hoveredStation;
    private FoodItem hoveredFoodItem;
    private Interactable lastInteractable;

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
        Ray r = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        if (Physics.Raycast(r, out RaycastHit hit, interactDistance))
        {
            hoveredDrawer = hit.collider.GetComponent<Drawer>();
            hoveredCell = hit.collider.GetComponent<CoolingCell>();
            hoveredStation = hit.collider.GetComponent<CookStation>();
            hoveredFoodItem = hit.collider.GetComponent<FoodItem>();

            // Handle highlight
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != lastInteractable)
            {
                if (lastInteractable) lastInteractable.Highlight(false);
                if (interactable) interactable.Highlight(true);
                lastInteractable = interactable;
            }

            if (interactionText)
                interactionText.text = interactable ? interactable.promptMessage : "";

            // Show drawer stock UI when hovering a drawer
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

            if (lastInteractable) lastInteractable.Highlight(false);
            lastInteractable = null;

            if (interactionText) interactionText.text = "";

            if (DrawerUIManager.Instance != null)
                DrawerUIManager.Instance.HideStock();
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

        if (hoveredStation != null)
        {
            FoodItem removed = hoveredStation.RemoveFood();
            if (removed != null) Grab(removed.gameObject);
            return;
        }
    }

    private void TryPlace()
    {
        if (hoveredStation != null && heldFood != null && hoveredStation.CanAccept(heldFood.foodType))
        {
            hoveredStation.PlaceFood(heldFood);
            ClearHeld();
        }
    }

    public void Grab(GameObject obj)
    {
        heldObject = obj;
        heldFood = obj.GetComponent<FoodItem>();

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

        heldObject = null;
        heldFood = null;
    }

    private void ClearHeld()
    {
        heldObject = null;
        heldFood = null;
    }
}
