using UnityEngine;

public enum CookState { Raw, Cooked, Burned, Other, Empty, Filled }

[RequireComponent(typeof(MeshRenderer))]
public class FoodItem : MonoBehaviour
{
    public FoodType foodType;
    public CookState state = CookState.Raw;

    public Material rawMaterial;
    public Material cookedMaterial;
    public Material burnedMaterial;

    public Material emptyMaterial;
    public Material filledMaterial;

    [SerializeField] private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        ApplyCurrentMaterial();
    }

    private void OnValidate()
    {
        ApplyCurrentMaterial();
    }

    public void SetState(CookState newState)
    {
        state = newState;
        ApplyCurrentMaterial();
    }

    private void ApplyCurrentMaterial()
    {
        if (CookState.Other == state) return;
        Material mat = null;
        switch (state)
        {
            case CookState.Raw: mat = rawMaterial; break;
            case CookState.Cooked: mat = cookedMaterial; break;
            case CookState.Burned: mat = burnedMaterial; break;
        }

        if (CookState.Filled == state && CookState.Empty == state) return;
        switch (state)
        {
            case CookState.Empty: mat = emptyMaterial; break;
            case CookState.Filled: mat = filledMaterial; break;
        }

        meshRenderer.material = mat;
    }
}