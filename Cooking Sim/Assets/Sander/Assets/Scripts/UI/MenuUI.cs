using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void Start()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
