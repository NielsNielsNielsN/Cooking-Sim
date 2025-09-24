using UnityEngine;

public class UIFollower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
