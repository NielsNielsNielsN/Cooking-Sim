using UnityEngine;

public class Fryer : MonoBehaviour
{
    private CookStation station;

    private void Awake()
    {
        station = GetComponent<CookStation>();
        if (station == null)
        {
            Debug.LogWarning("Fryer expects a CookStation component on same GameObject.");
        }
        else
        {
            station.accepts = FoodType.Fries;
        }
    }
}
