using UnityEngine;
using UnityEngine.UI;

public class SyncedFullscreen : MonoBehaviour
{
    public Toggle toggleA;
    public Toggle toggleB;

    private void Start()
    { 
        toggleA.onValueChanged.AddListener(OnToggleAChanged);
        toggleB.onValueChanged.AddListener(OnToggleBChanged);
    }

    private void OnToggleAChanged(bool isOn)
    {
        if (toggleB.isOn != isOn)
            toggleB.isOn = isOn;
    }

    private void OnToggleBChanged(bool isOn)
    {
        if (toggleA.isOn != isOn)
            toggleA.isOn = isOn;
    }
}
