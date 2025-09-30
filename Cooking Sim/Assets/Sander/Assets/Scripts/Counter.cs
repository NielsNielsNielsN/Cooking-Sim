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

    public Tray TakeTray()
    {
        if (traySpot.childCount == 0) return null;
        Tray tray = traySpot.GetChild(0).GetComponent<Tray>();
        if (tray != null)
        {
            tray.transform.SetParent(null);
            return tray;
        }
        return null;
    }
}
