using UnityEngine;

public class Counter : MonoBehaviour
{
    public Transform traySpot;

    public void AcceptTray(Tray tray)
    {
        tray.transform.SetParent(traySpot != null ? traySpot : transform);
        tray.transform.localPosition = Vector3.zero;
        tray.transform.localRotation = Quaternion.identity;
    }
}
