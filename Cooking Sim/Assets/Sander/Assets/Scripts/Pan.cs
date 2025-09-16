using UnityEngine;

public class Pan : MonoBehaviour
{
    private CookStation station;

    private void Awake()
    {
        station = GetComponent<CookStation>();
        if (station == null)
        {
            Debug.LogWarning("Pan expects a CookStation component on same GameObject.");
        }
        else
        {
            station.accepts = FoodType.Hotdog;
        }
    }
}
