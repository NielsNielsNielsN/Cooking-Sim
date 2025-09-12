using UnityEngine;

public class RefillBag : MonoBehaviour, IInteractable
{
    public FoodType bagType;
    public int refillAmount = 10;

    public GameObject Pickup(Transform holder)
    {
        transform.SetParent(holder);
        transform.localPosition = Vector3.forward * 0.5f;
        return gameObject;
    }
}