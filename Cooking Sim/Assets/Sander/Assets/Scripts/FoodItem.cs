using UnityEngine;

public enum CookState { Raw, Cooking, Cooked, Burned }

public class FoodItem : MonoBehaviour
{
    public FoodType foodType;
    public CookState state = CookState.Raw;

    // visual placeholders
    public GameObject cookedVisual;
    public GameObject burnedVisual;
    public GameObject rawVisual;

    private void Start()
    {
        UpdateVisual();
    }

    public void SetState(CookState newState)
    {
        state = newState;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (rawVisual) rawVisual.SetActive(state == CookState.Raw);
        if (cookedVisual) cookedVisual.SetActive(state == CookState.Cooked);
        if (burnedVisual) burnedVisual.SetActive(state == CookState.Burned);
    }
}
