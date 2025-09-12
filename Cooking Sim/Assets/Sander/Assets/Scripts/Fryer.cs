using UnityEngine;

public class Pan : MonoBehaviour, IPlacementZone
{
    public bool CanPlace(GameObject item)
    {
        return item.GetComponent<Hotdog>() != null;
    }

    public void Place(GameObject item)
    {
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = false;
        item.GetComponent<ICookable>().StartCooking();
    }
}