using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        transform.forward = cam.transform.forward;
    }
}
