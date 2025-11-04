using UnityEngine;

public class Knife : MonoBehaviour
{
    public GameObject openBunPrefab;
    public AudioClip cutSound;      // Assign your cutting sound in the Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void TryCut(CuttingBoard board)
    {
        if (board == null || board.currentItem == null) return;

        FoodItem food = board.currentItem.GetComponent<FoodItem>();
        if (food == null || food.foodType != FoodType.ClosedBun) return;

        Vector3 pos = board.currentItem.transform.position;
        Quaternion rot = board.currentItem.transform.rotation;

        GameObject oldBun = board.currentItem;
        board.currentItem = null;
        Destroy(oldBun);

        if (cutSound != null && audioSource != null)
        {
            audioSource.clip = cutSound;
            audioSource.Play();
        }

        GameObject openBun = Object.Instantiate(openBunPrefab, pos, rot);
        openBun.transform.SetParent(board.transform);
        board.currentItem = openBun;

        Rigidbody rb = openBun.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }
}
