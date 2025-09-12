using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hotdog : MonoBehaviour, IInteractable, ICookable
{
    public FoodType type = FoodType.Hotdog;
    public float cookTime = 10f;
    public GameObject progressBarPrefab;
    private Slider progressBar;
    private Coroutine cookingRoutine;
    private bool isBurned;

    public GameObject Pickup(Transform holder)
    {
        transform.SetParent(holder);
        transform.localPosition = Vector3.forward * 0.5f;
        return gameObject;
    }

    public void StartCooking()
    {
        if (isBurned) return;
        GameObject barObj = Instantiate(progressBarPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        barObj.transform.SetParent(transform);
        progressBar = barObj.GetComponent<Slider>();
        progressBar.maxValue = cookTime;
        progressBar.value = 0;
        cookingRoutine = StartCoroutine(Cook());
    }

    private IEnumerator Cook()
    {
        float time = 0;
        while (time < cookTime)
        {
            time += Time.deltaTime;
            progressBar.value = time;
            if (Random.Range(0, 100) < 5)
            {
                yield return StartCoroutine(QTE());
            }
            yield return null;
        }
        Destroy(progressBar.gameObject);
        // Swap to cooked model or material here
    }

    private IEnumerator QTE()
    {
        UIManager.Instance.ShowQTCPrompt("Press E!");
        float qteWindow = 2f;
        bool success = false;
        float startTime = Time.time;

        while (Time.time - startTime < qteWindow)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                success = true;
                break;
            }
            yield return null;
        }

        UIManager.Instance.HideQTCPrompt();
        if (!success)
        {
            Burn();
        }
    }

    public void Burn()
    {
        if (cookingRoutine != null) StopCoroutine(cookingRoutine);
        isBurned = true;
        // Swap to burned model or material here
        if (progressBar) Destroy(progressBar.gameObject);
    }
}