using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    public Transform itemPosition;
    public GameObject knifeVisual; // Visual representation of the knife on the board
    public GameObject currentItem;

    public bool PlaceItem(GameObject item)
    {
        if (currentItem != null) return false;
        currentItem = item;
        item.transform.SetParent(transform);
        item.transform.position = itemPosition.position;
        item.transform.rotation = itemPosition.rotation;

        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        return true;
    }

    public GameObject TakeItem()
    {
        if (currentItem == null) return null;
        GameObject item = currentItem;
        currentItem = null;

        var rb = item.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        item.transform.SetParent(null);
        return item;
    }

    public bool HasKnifeVisual() => knifeVisual != null && knifeVisual.activeSelf;
    public void SetKnifeVisual(bool state)
    {
        if (knifeVisual != null)
            knifeVisual.SetActive(state);
    }
}
