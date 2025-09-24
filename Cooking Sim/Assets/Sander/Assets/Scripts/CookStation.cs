using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CookStation : MonoBehaviour
{
    public FoodType accepts;
    public float cookingTime;
    public float burnAfterCooked;
    public GameObject cookingSliderPrefab;
    public Vector3 foodLocalPosition = new Vector3(-0.4f, -0.255f, 0f);

    public int qteCount;
    public float qteChancePerSecond;
    public float qteWindow;

    private FoodItem currentFood;
    private GameObject sliderInstance;
    private Slider slider;
    private RectTransform cookMarker;
    private Coroutine cookingRoutine;
    private bool isCooking;

    private void OnTriggerEnter(Collider other)
    {
        FoodItem f = other.GetComponent<FoodItem>();
        if (f != null && currentFood == null && f.foodType == accepts)
        {
            PlaceFood(f);
        }
    }

    public bool CanAccept(FoodType type) => type == accepts && currentFood == null;

    public void PlaceFood(FoodItem f)
    {
        currentFood = f;
        Vector3 originalScale = currentFood.transform.lossyScale;
        currentFood.transform.SetParent(transform);
        currentFood.transform.localPosition = foodLocalPosition;
        currentFood.transform.localRotation = Quaternion.identity;
        currentFood.transform.localScale = new Vector3(
            originalScale.x / transform.lossyScale.x,
            originalScale.y / transform.lossyScale.y,
            originalScale.z / transform.lossyScale.z
        );
        StartCooking();
    }

    public FoodItem RemoveFood()
    {
        if (isCooking && cookingRoutine != null) StopCoroutine(cookingRoutine);
        isCooking = false;
        if (sliderInstance) Destroy(sliderInstance);
        FoodItem f = currentFood;
        if (f != null)
        {
            currentFood.transform.SetParent(null);
            currentFood = null;
        }
        return f;
    }

    private void StartCooking()
    {
        if (currentFood == null) return;
        isCooking = true;

        if (cookingSliderPrefab)
        {
            sliderInstance = Instantiate(cookingSliderPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
            slider = sliderInstance.GetComponentInChildren<Slider>();
            sliderInstance.AddComponent<BillboardUI>();
            if (slider) slider.value = 0f;
            var follow = sliderInstance.AddComponent<UIFollower>();
            follow.target = transform;
            follow.offset = Vector3.up * 1.2f;
            cookMarker = sliderInstance.transform.Find("CookMarker")?.GetComponent<RectTransform>();
            if (cookMarker != null)
            {
                float totalTime = cookingTime + burnAfterCooked;
                float normalizedCookPoint = cookingTime / totalTime;
                RectTransform sliderRect = slider.GetComponent<RectTransform>();
                float sliderWidth = sliderRect.rect.width;
                float xPos = (normalizedCookPoint * sliderWidth) - (sliderWidth * 0.5f);
                cookMarker.anchoredPosition = new Vector2(xPos, cookMarker.anchoredPosition.y);
            }
        }

        cookingRoutine = StartCoroutine(CookingProcess());
    }

    private IEnumerator CookingProcess()
    {
        float elapsed = 0f;
        int qtesTriggered = 0;
        float totalTime = cookingTime + burnAfterCooked;

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            if (slider) slider.value = Mathf.Clamp01(elapsed / totalTime);
            if (elapsed < cookingTime && QTEManager.Instance != null && qtesTriggered < qteCount)
            {
                if (Random.value < qteChancePerSecond * Time.deltaTime)
                {
                    bool qteSuccess = false;
                    yield return QTEManager.Instance.RunQTE(qteWindow, (success) => qteSuccess = success);
                    qtesTriggered++;
                    if (!qteSuccess)
                    {
                        BurnCurrent();
                        yield break;
                    }
                }
            }
            if (elapsed >= cookingTime && currentFood != null && currentFood.state == CookState.Raw)
                currentFood.SetState(CookState.Cooked);
            if (elapsed >= totalTime && currentFood != null && currentFood.state == CookState.Cooked)
                BurnCurrent();
            yield return null;
        }
    }

    private void BurnCurrent()
    {
        if (currentFood == null) return;
        currentFood.SetState(CookState.Burned);
        isCooking = false;
        if (cookingRoutine != null) StopCoroutine(cookingRoutine);
        cookingRoutine = null;
        if (sliderInstance) Destroy(sliderInstance, 1f);
    }

    private void OnDisable()
    {
        if (cookingRoutine != null) StopCoroutine(cookingRoutine);
    }
}

