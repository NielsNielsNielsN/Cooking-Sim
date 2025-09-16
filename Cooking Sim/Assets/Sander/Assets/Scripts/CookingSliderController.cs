using UnityEngine;
using UnityEngine.UI;

public class CookingSliderController : MonoBehaviour
{
    [Tooltip("Assign a Slider (UI) component in this prefab.")]
    public Slider slider;

    private void Reset()
    {
        slider = GetComponentInChildren<Slider>();
    }

    // The CookStation directly sets slider.value, so we don't need logic here.
}
