using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text qtePrompt;

    void Awake()
    {
        Instance = this;
        if (qtePrompt) qtePrompt.gameObject.SetActive(false);
    }

    public void ShowQTCPrompt(string message)
    {
        if (qtePrompt)
        {
            qtePrompt.text = message;
            qtePrompt.gameObject.SetActive(true);
        }
    }

    public void HideQTCPrompt()
    {
        if (qtePrompt) qtePrompt.gameObject.SetActive(false);
    }
}