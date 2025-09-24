using UnityEngine;

public class Fryer : MonoBehaviour
{
    [SerializeField] private CookStation station;

    private void Awake()
    {
        station.accepts = FoodType.Fries;
    }
}
