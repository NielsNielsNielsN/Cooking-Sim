using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DayCycleManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float startTime = 14f;
    [SerializeField] private float endTime = 22f;
    [SerializeField] private float timeSpeed = 1f; // 1 real second = 1 in-game minute

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage; // Assign the FadePanel's Image component
    [SerializeField] private float fadeDuration = 1f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode nextDayKey = KeyCode.N;

    private int currentDay = 1;
    private float currentTime;

    private void Start()
    {
        currentTime = startTime;

        // Ensure fadeImage starts fully transparent
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    private void Update()
    {
        // Progress time
        currentTime += Time.deltaTime * timeSpeed;
        currentTime = Mathf.Min(currentTime, endTime);

        if (currentDay == 8)
        {
            print("You Win");
        }

        // Advance to next day only at 22:00
        if (Input.GetKeyDown(nextDayKey))
        {
            if (Mathf.Approximately(currentTime, endTime))
            {
                StartCoroutine(FadeToNextDay());
            }
            else
            {
                Debug.Log("You can't progress to the next day until 22:00.");
            }
        }

        UpdateClock();
    }

    private void UpdateClock()
    {
        int hour = Mathf.FloorToInt(currentTime);
        int minute = Mathf.FloorToInt((currentTime - hour) * 60);
        timeText.text = $"Time: {hour:D2}:{minute:D2}";
        dayText.text = $"Day: {currentDay}";
    }

    private IEnumerator FadeToNextDay()
    {
        yield return StartCoroutine(Fade(1f)); // Fade to black

        currentDay++;
        currentTime = startTime;

        yield return StartCoroutine(Fade(0f)); // Fade back in
    }

    private IEnumerator Fade(float targetAlpha)
    {
        Color startColor = fadeImage.color;
        float startAlpha = startColor.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}
