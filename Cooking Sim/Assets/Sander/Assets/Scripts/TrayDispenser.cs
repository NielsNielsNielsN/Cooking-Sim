using UnityEngine;

public class TrayDispenser : MonoBehaviour
{
    public GameObject trayPrefab;

    public GameObject TakeTray()
    {
        if (trayPrefab == null) return null;
        return Instantiate(trayPrefab);
    }
}
