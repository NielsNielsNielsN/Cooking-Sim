using UnityEngine;

public class Counter : MonoBehaviour
{
    [Header("Tray Spot")]
    public Transform traySpot;
    private Tray tray;

    public void AcceptTray(Tray tray)
    {
        if (tray == null) return;
        this.tray = tray;
        Transform parent = traySpot != null ? traySpot : transform;
        tray.transform.SetParent(parent);
        tray.transform.localPosition = Vector3.zero;
        tray.transform.localRotation = Quaternion.identity;
        Debug.Log($"[Counter] TRAY ACCEPTED: '{tray.orderName}'");
    }

    // PLAYER USES THIS TO TAKE TRAY
    public Tray TakeTray()
    {
        if (traySpot == null || traySpot.childCount == 0) return null;
        Transform child = traySpot.GetChild(0);
        Tray tray = child.GetComponent<Tray>();
        if (tray != null)
        {
            child.SetParent(null);
            Debug.Log($"[Counter] TRAY TAKEN BY PLAYER: '{tray.orderName}'");
        }
        return tray;
    }

    public bool HasMatchingTray(string orderName)
    {
        if (traySpot == null || traySpot.childCount == 0) return false;
        foreach (Transform child in traySpot)
        {
            Tray tray = child.GetComponent<Tray>();
            if (tray != null && !string.IsNullOrEmpty(tray.orderName) &&
                string.Equals(tray.orderName, orderName, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"[Counter] MATCH FOUND: '{orderName}'");
                return true;
            }
        }
        Debug.Log($"[Counter] NO MATCH for '{orderName}'");
        return false;
    }

    [ContextMenu("Clear Tray Spot")]
    public void ClearTraySpot()
    {
        if (traySpot == null) return;
        GameManager.Instance.AddMoney(40f);

        for (int i = traySpot.childCount - 1; i >= 0; i--)
            Destroy(traySpot.GetChild(i).gameObject);
    }
}