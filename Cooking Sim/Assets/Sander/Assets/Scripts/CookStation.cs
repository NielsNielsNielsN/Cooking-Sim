using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CookStation : MonoBehaviour
{
    public FoodType accepts;
    public float cookingTime = 6f;
    public float burnAfterCooked = 4f;
    public GameObject cookingSliderPrefab;

    private FoodItem currentFood;
    private GameObject sliderInstance;
    private Slider slider;
    private RectTransform cookMarker;   // vertical marker line
    private Coroutine cookingRoutine;
    private bool isCooking = false;

    public int qteCount = 2;
    public float qteChancePerSecond = 0.25f;
    public float qteWindow = 0.8f;

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

        // Save the world scale of the food before parenting
        Vector3 originalScale = currentFood.transform.lossyScale;

        // Parent it to the station
        currentFood.transform.SetParent(transform);

        // Set its position/rotation
        currentFood.transform.localPosition = Vector3.up * 0.2f;
        currentFood.transform.localRotation = Quaternion.identity;

        // Restore original scale so it doesn’t inherit from the pan/fryer
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
            // Instantiate in world space (no parent)
            sliderInstance = Instantiate(cookingSliderPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
            slider = sliderInstance.GetComponentInChildren<Slider>();

            // Billboard
            sliderInstance.AddComponent<BillboardUI>();

            if (slider) slider.value = 0f;

            // Add a follow script so it stays above the pan/fryer
            var follow = sliderInstance.AddComponent<UIFollower>();
            follow.target = transform;
            follow.offset = Vector3.up * 1.2f;

            // Add a vertical marker for the "cooked" point
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

            // Slider now fills over both cooking + burning
            if (slider) slider.value = Mathf.Clamp01(elapsed / totalTime);

            // QTEs only during cooking phase
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

            // Mark as cooked when cooking time ends
            if (elapsed >= cookingTime && currentFood != null && currentFood.state == CookState.Raw)
                currentFood.SetState(CookState.Cooked);

            // Burn when burn time ends
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
