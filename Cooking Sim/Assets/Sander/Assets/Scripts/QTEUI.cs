using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public Slider timerSlider;

    public void Setup(string keyName, float duration)
    {
        if (promptText) promptText.text = $"Press {keyName}!";
        if (timerSlider)
        {
            timerSlider.maxValue = duration;
            timerSlider.value = duration;
        }
    }

    public void UpdateTimer(float timeLeft)
    {
        if (timerSlider) timerSlider.value = timeLeft;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
