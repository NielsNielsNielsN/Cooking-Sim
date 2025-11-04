using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OrderCardButton : MonoBehaviour
{
    // This will be set at runtime – NOT in the prefab!
    public Customer customer;
    public CustomerOrder orderSystem;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        orderSystem?.OrderReady(customer);
    }
}