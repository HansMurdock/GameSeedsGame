using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panelPauseMenu;
    public GameObject panelSettingsDisplay;
    public GameObject panelSettingsAudio;
    public GameObject panelSettingsControls;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "NamaSceneMainMenuKamu";

    private bool isPaused = false;

    void Start()
    {
        // Pastikan semua menu tertutup saat game dimulai
        panelPauseMenu.SetActive(false);
        CloseAllSettingsPanels();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Jika sedang membuka salah satu menu Settings, kembali ke Pause Menu dulu
            if (panelSettingsDisplay.activeSelf || panelSettingsAudio.activeSelf || panelSettingsControls.activeSelf)
            {
                BackToPauseMenu();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        panelPauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        panelPauseMenu.SetActive(false);
        CloseAllSettingsPanels();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- FUNGSI NAVIGASI MENU ---

    public void OpenSettings()
    {
        panelPauseMenu.SetActive(false);
        OpenTabDisplay(); // Saat klik Settings dari Pause, otomatis buka tab Display
    }

    public void BackToPauseMenu()
    {
        CloseAllSettingsPanels();
        panelPauseMenu.SetActive(true);
    }

    // --- FUNGSI TAB SETTINGS ---

    public void OpenTabDisplay()
    {
        CloseAllSettingsPanels();
        panelSettingsDisplay.SetActive(true);
    }

    public void OpenTabAudio()
    {
        CloseAllSettingsPanels();
        panelSettingsAudio.SetActive(true);
    }

    public void OpenTabControls()
    {
        CloseAllSettingsPanels();
        panelSettingsControls.SetActive(true);
    }

    // Fungsi bantuan untuk mematikan semua tab sekaligus
    private void CloseAllSettingsPanels()
    {
        if (panelSettingsDisplay != null) panelSettingsDisplay.SetActive(false);
        if (panelSettingsAudio != null) panelSettingsAudio.SetActive(false);
        if (panelSettingsControls != null) panelSettingsControls.SetActive(false);
    }

    // --- FUNGSI KELUAR ---

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGameApplication()
    {
        UnityEngine.Debug.Log("Game Keluar!");
        UnityEngine.Application.Quit();
    }
}