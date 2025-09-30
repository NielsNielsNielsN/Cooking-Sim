using UnityEngine;

public enum CookState { Raw, Cooked, Burned, Other }

[RequireComponent(typeof(MeshRenderer))]
public class FoodItem : MonoBehaviour
{
    public FoodType foodType;
    public CookState state = CookState.Raw;

    public Material rawMaterial;
    public Material cookedMaterial;
    public Material burnedMaterial;

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

        meshRenderer.material = mat;
    }
}