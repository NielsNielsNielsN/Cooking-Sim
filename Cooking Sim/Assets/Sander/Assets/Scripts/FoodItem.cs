using UnityEngine;

public enum CookState { Raw, Cooked, Burned }

[RequireComponent(typeof(MeshRenderer))]
public class FoodItem : MonoBehaviour
{
    public FoodType foodType;
    public CookState state = CookState.Raw;

    public Material rawMaterial;
    public Material cookedMaterial;
    public Material burnedMaterial;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        ApplyCurrentMaterial();
    }

    private void OnValidate()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        ApplyCurrentMaterial();
    }

    public void SetState(CookState newState)
    {
        state = newState;
        ApplyCurrentMaterial();
    }

    private void ApplyCurrentMaterial()
    {
        if (!meshRenderer) return;

        Material mat = null;
        switch (state)
        {
            case CookState.Raw: mat = rawMaterial; break;
            case CookState.Cooked: mat = cookedMaterial; break;
            case CookState.Burned: mat = burnedMaterial; break;
        }

        if (mat != null)
            meshRenderer.material = mat; // force unique instance material
    }
}