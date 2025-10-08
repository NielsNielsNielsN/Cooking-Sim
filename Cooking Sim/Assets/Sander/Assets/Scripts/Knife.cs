using UnityEngine;

public class Knife : MonoBehaviour
{
    public GameObject openBunPrefab;

    public void TryCut(CuttingBoard board)
    {
        if (board.currentItem == null) return;

        Cuttable cuttable = board.currentItem.GetComponent<Cuttable>();
        if (cuttable != null && !cuttable.isCut)
        {
            cuttable.Cut();
            return;
        }

        FoodItem food = board.currentItem.GetComponent<FoodItem>();
        if (food != null && food.foodType == FoodType.Hotdog)
        {
            Vector3 pos = board.currentItem.transform.position;
            Quaternion rot = board.currentItem.transform.rotation;

            Destroy(board.currentItem);

            GameObject openBun = Instantiate(openBunPrefab, pos, rot);
            openBun.transform.localScale = Vector3.one * 2f;
            board.currentItem = openBun;
        }
    }

}
