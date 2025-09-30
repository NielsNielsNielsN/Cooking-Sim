using UnityEngine;

public class DoorRotator : MonoBehaviour
{
    public Transform door;
    public float angle;
    public float speed;
    private bool open;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = door.localRotation;
        openRot = closedRot * Quaternion.Euler(0, angle, 0);
    }

    void Update()
    {
        door.localRotation = Quaternion.Lerp(
            door.localRotation,
            open ? openRot : closedRot,
            Time.deltaTime * speed
        );

        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
            );
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.transform == transform)
                    open = !open;
            }
        }
    }
}
