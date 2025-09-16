using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Tooltip("Text shown when looking at this object.")]
    public string promptMessage = "Press E to interact";

    private Renderer rend;
    private Material originalMat;
    public Material highlightMat;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend) originalMat = rend.material;
    }

    public void Highlight(bool active)
    {
        if (rend == null) return;
        if (active && highlightMat != null)
            rend.material = highlightMat;
        else
            rend.material = originalMat;
    }
}
