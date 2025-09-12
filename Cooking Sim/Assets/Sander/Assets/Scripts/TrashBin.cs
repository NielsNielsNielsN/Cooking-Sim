using UnityEngine;

public class TrashBin : MonoBehaviour, IPlacementZone
{
    public bool CanPlace(GameObject item)
    {
        ICookable cookable = item.GetComponent<ICookable>();
        return cookable != null; // Add burned check if implemented
    }

    public void Place(GameObject item)
    {
        Destroy(item);
    }
}