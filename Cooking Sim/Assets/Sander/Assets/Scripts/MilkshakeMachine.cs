using UnityEngine;

public class MilkshakeMachine : MonoBehaviour
{
    [SerializeField] private CookStation station;

    private void Awake()
    {
        station.accepts = FoodType.Milkshake;
    }
}
