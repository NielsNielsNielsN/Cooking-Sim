using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashBin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FoodItem fi = other.GetComponent<FoodItem>();
        if (fi != null && fi.state == CookState.Burned)
        {
            Destroy(fi.gameObject);
        }
    }
}
