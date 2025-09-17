using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Interactable : MonoBehaviour
{
    [Tooltip("Text shown when looking at this object.")]
    public string promptMessage = "Press E to interact";

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void Highlight(bool active)
    {
        // No material changes anymore
        // You can still use this method for other effects if needed
    }
}