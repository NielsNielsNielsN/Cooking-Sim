using UnityEngine;

public class Counter : MonoBehaviour
{
    [Header("Tray Spot")]
    [Tooltip("Drag the empty GameObject under the counter where trays are placed")]
    public Transform traySpot;

    public void AcceptTray(Tray tray)
    {
        if (tray == null)
        {
            Debug.LogWarning("Counter.AcceptTray: Tray is null!");
            return;
        }

        // Parent to traySpot (fallback to counter itself if not assigned)
        Transform parent = traySpot != null ? traySpot : transform;

        tray.transform.SetParent(parent);
        tray.transform.localPosition = Vector3.zero;
        tray.transform.localRotation = Quaternion.identity;

        Debug.Log($"Tray accepted: {tray.orderName}");
    }

    public Tray TakeTray()
    {
        if (traySpot == null || traySpot.childCount == 0)
            return null;

        Tray tray = traySpot.GetChild(0).GetComponent<Tray>();
        if (tray != null)
        {
            tray.transform.SetParent(null); // Detach from counter
            Debug.Log($"Tray taken by customer: {tray.orderName}");
            return tray;
        }

        return null;
    }

    public bool HasMatchingTray(string orderName)
    {
        if (traySpot == null || traySpot.childCount == 0) return false;

        foreach (Transform child in traySpot)
        {
            Tray tray = child.GetComponent<Tray>();
            if (tray != null && tray.orderName == orderName)
                return true;
        }
        return false;
    }
}