using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;          // Where the item will be held
    public float pickupRange = 3f;       // Max distance to pick items
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    private GameObject heldItem;
    private Rigidbody heldItemRb;

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldItem == null)
            {
                TryPickup();
            }
        }

        if (Input.GetKeyDown(dropKey))
        {
            if (heldItem != null)
            {
                DropItem();
            }
        }
    }

    void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldItem = hit.collider.gameObject;
                heldItemRb = heldItem.GetComponent<Rigidbody>();

                // Disable physics while holding
                heldItemRb.isKinematic = true;

                // Parent to hold point
                heldItem.transform.SetParent(holdPoint);
                heldItem.transform.localPosition = Vector3.zero;
                heldItem.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void DropItem()
    {
        // Unparent
        heldItem.transform.SetParent(null);

        // Enable physics again
        heldItemRb.isKinematic = false;

        // Add a little forward force so it drops naturally
        heldItemRb.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse);

        heldItem = null;
        heldItemRb = null;
    }
}
