using UnityEngine;
using UnityEngine.UI;

public class SyncedSliders : MonoBehaviour
{
    public Slider sliderA;
    public Slider sliderB;

    private void Start()
    {
        // Add listeners to both sliders
        sliderA.onValueChanged.AddListener(OnSliderAChanged);
        sliderB.onValueChanged.AddListener(OnSliderBChanged);
    }

    private void OnSliderAChanged(float value)
    {
        if (sliderB.value != value)
            sliderB.value = value;
    }

    private void OnSliderBChanged(float value)
    {
        if (sliderA.value != value)
            sliderA.value = value;
    }
}
