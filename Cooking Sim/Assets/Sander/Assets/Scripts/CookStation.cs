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
        currentFood.transform.SetParent(transform);
        currentFood.transform.localPosition = Vector3.up * 0.2f;
        currentFood.transform.localRotation = Quaternion.identity;
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
            sliderInstance = Instantiate(cookingSliderPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity, transform);
            slider = sliderInstance.GetComponentInChildren<Slider>();

            // Fix scale
            sliderInstance.transform.localScale = Vector3.one * 0.01f;

            // Billboard
            sliderInstance.AddComponent<BillboardUI>();

            if (slider) slider.value = 0f;
        }
        cookingRoutine = StartCoroutine(CookingProcess());
    }

    private IEnumerator CookingProcess()
    {
        float elapsed = 0f;
        int qtesTriggered = 0;
        while (elapsed < cookingTime)
        {
            elapsed += Time.deltaTime;
            if (slider) slider.value = elapsed / cookingTime;

            if (QTEManager.Instance != null && qtesTriggered < qteCount)
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
            yield return null;
        }

        if (currentFood != null) currentFood.SetState(CookState.Cooked);

        float burnTimer = 0f;
        while (burnTimer < burnAfterCooked)
        {
            burnTimer += Time.deltaTime;
            yield return null;
        }

        BurnCurrent();
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
