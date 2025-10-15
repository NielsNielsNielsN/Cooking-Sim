using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private bool visible = false;

    [SerializeField] private Canvas pauseCanvas;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            visible = !visible;
            pauseCanvas.gameObject.SetActive(visible);
        }
    }
}
