using UnityEngine;

public class Pan : MonoBehaviour
{
    [SerializeField] private CookStation station;

    private void Awake()
    {
        station.accepts = FoodType.Hotdog;
    }
}
