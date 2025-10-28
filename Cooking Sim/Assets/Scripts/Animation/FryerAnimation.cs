using UnityEngine;

public class FryerAnimation : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator basketAnimator;

    [Header("Fry Detection")]
    [SerializeField] private Transform basketTransform; // The basket GameObject
    [SerializeField] private GameObject targetFryPrefab; // Assign your fry prefab here

    private bool isFrying = false;

    void Update()
    {
        bool fryPresent = IsTargetChildPresent();

        // Only trigger animation when state changes
        if (fryPresent != isFrying)
        {
            isFrying = fryPresent;
            basketAnimator.SetBool("IsFrying", isFrying);
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
    public void SetFryingState(bool frying)
    {
        if (frying != isFrying)
        {
            isFrying = frying;
            basketAnimator.SetBool("IsFrying", isFrying);
        }
    }
}
