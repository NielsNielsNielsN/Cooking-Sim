using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private bool visible = false;
    [SerializeField] private Canvas pauseCanvas;
    public GameObject startMenu;
    public GameObject settingsMenu;
    public Slider masterVolumeSlider;
    public AudioMixer audioMixer;
    public Toggle fullscreenToggle;
    public Button backButton;
    private bool backButtonPressed;
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

        backButton.onClick.AddListener(() => backButtonPressed = true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowStartMenu()
    {
        startMenu.SetActive(true);
        settingsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowSettingsMenu()
    {
        startMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    void Update()
    {
        PauseMenuOn();
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseMenuOn()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || backButtonPressed)
        {
            backButtonPressed = false;
            visible = !visible;
            pauseCanvas.gameObject.SetActive(visible);

            if (visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Freeze();
            }
            else
            {
                CursorLockModeOn();
            }
        }
    }

    public void CursorLockModeOn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Unfreeze();
    }

    public void Freeze()
    {
        Time.timeScale = 0f;
    }

    public void Unfreeze()
    {
        Time.timeScale = 1f;
    }
    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}
