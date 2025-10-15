using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject settingsMenu;
    public Slider masterVolumeSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        SetVolume(masterVolumeSlider.value);
        SetFullscreen(fullscreenToggle.isOn);

        masterVolumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        ShowStartMenu();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowStartMenu()
    {
        startMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ShowSettingsMenu()
    {
        startMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}
