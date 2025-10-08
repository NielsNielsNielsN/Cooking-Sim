using UnityEngine;

public class Cuttable : MonoBehaviour
{
    public Mesh closedMesh;
    public Mesh openMesh;
    public bool isCut;

    [SerializeField]private MeshFilter meshFilter;
    [SerializeField]private MeshRenderer meshRenderer;
    [SerializeField]private Material openMaterial;


    private void Awake()
    {
        meshFilter.mesh = closedMesh;
    }

    public void Cut()
    {
        if (isCut) return;
        isCut = true;
        meshFilter.mesh = openMesh;
        meshRenderer.material = openMaterial;
    }
}
