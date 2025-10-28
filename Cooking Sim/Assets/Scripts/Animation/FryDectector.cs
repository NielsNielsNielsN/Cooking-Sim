using UnityEngine;

public class FryDectector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FryerAnimation fryerController;
    [SerializeField] private GameObject targetFryPrefab; // Assign your fry prefab here
    [SerializeField] private Transform basketTransform;  // Assign the basket GameObject here

    private bool fryPresentLastFrame = false;

    void Update()
    {
        bool fryPresentNow = IsTargetChildPresent();

        // Only notify fryerController when state changes
        if (fryPresentNow != fryPresentLastFrame)
        {
            fryerController.SetFryingState(fryPresentNow);
            fryPresentLastFrame = fryPresentNow;
        }
    }

    private bool IsTargetChildPresent()
    {
        foreach (Transform child in basketTransform)
        {
            if (child.gameObject.name.Contains(targetFryPrefab.name))
            {
                return true;
            }
        }
        return false;
    }
}