using UnityEngine;

public class BowlDispenser : MonoBehaviour
{
    public GameObject bowlPrefab;

    public GameObject TakeBowl()
    {
        if (bowlPrefab == null) return null;
        return Instantiate(bowlPrefab);
    }
}
