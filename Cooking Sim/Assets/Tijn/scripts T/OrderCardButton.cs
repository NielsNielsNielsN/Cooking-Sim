using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OrderCardButton : MonoBehaviour
{
    public Customer customer;
    public CustomerOrder orderSystem;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners(); // Prevent duplicates
            btn.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (orderSystem != null && customer != null)
        {
            Debug.Log($"[OrderCardButton] Customer {customer.name} order ready!");
            orderSystem.OrderReady(customer);
        }
    }
}