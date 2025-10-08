using UnityEngine;

public class Knife : MonoBehaviour
{
    public GameObject openBunPrefab;

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
