using UnityEngine;

public interface IPlacementZone
{
    bool CanPlace(GameObject item);
    void Place(GameObject item);
}