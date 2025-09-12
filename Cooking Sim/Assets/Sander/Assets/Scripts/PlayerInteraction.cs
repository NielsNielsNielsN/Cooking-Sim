using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 2f;
    public LayerMask interactableLayer;
    private GameObject heldItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                TryPickup();
            }
            else
            {
                TryPlace();
            }
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                heldItem = interactable.Pickup(transform);
                if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
            }
            else
            {
                StorageContainer storage = hit.collider.GetComponent<StorageContainer>();
                if (storage != null && storage.TryDispense(out GameObject food))
                {
                    heldItem = food;
                    if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
                }
            }
        }
    }

    void TryPlace()
    {
        if (Physics.Raycast(transform.position + transform.up * 0.5f, transform.forward, out RaycastHit hit, interactionDistance))
        {
            IPlacementZone zone = hit.collider.GetComponent<IPlacementZone>();
            if (zone != null && zone.CanPlace(heldItem))
            {
                zone.Place(heldItem);
                heldItem = null;
            }
        }
    }
}