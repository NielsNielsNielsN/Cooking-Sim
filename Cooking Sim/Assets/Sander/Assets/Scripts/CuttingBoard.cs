using UnityEngine;

public class CuttingBoard : MonoBehaviour
{

    public GameObject knifeVisual;
    public Transform itemSpot;
    public GameObject currentItem;

    public bool hasKnife = true;
    public bool HasKnife()
    {
        return hasKnife;
    }

    public void SetKnife(bool visible)
    {
        hasKnife = visible;
        if (knifeVisual != null)
            knifeVisual.SetActive(visible);
    }

    public bool PlaceItem(GameObject item)
    {
        if (currentItem != null) return false;

        currentItem = item;
        currentItem.transform.SetParent(itemSpot != null ? itemSpot : transform);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        var rb = currentItem.GetComponent<Rigidbody>();
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
        currentItem.transform.SetParent(null);

        var rb = currentItem.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        currentItem = null;
        return item;
    }
}
