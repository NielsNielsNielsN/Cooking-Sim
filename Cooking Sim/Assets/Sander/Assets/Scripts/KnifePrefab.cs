using UnityEngine;

public class KnifePrefab : MonoBehaviour
{
    public static KnifePrefab Instance;
    public GameObject knifePrefab;

    private void Awake()
    {
        Instance = this;
    }
}
