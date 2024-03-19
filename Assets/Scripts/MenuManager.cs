using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Canvas _menu;
    [SerializeField] private Canvas _settings;
    [SerializeField] private Canvas _controls;
    [SerializeField] private PlayerData _playerData;

    private void Awake()
    {
        Cursor.visible = true;
        if (_menu != null)
            _menu.enabled = true;
        if (_settings != null)
            _settings.enabled = false;
        if (_controls != null)
            _controls.enabled = false;
    }

    private void OnDestroy()
    {
        Cursor.visible = false;
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("Level"), LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        }
    }

    public void RestartGame()
    {
        PlayerPrefs.SetFloat("Flash Distance", _playerData.FlashDistance);
        PlayerPrefs.SetFloat("Flash Cooldown", _playerData.FlashCooldown);
        PlayerPrefs.SetInt("Number of Flashes", _playerData.NumberOfFlashes);
        PlayerPrefs.SetInt("Number of Energy Drinks", _playerData.NumberOfEnergyDrinks);
        PlayerPrefs.SetInt("Number of Bottles", _playerData.NumberOfBottles);
        PlayerPrefs.SetString("Level", "Level 1");
        if (!SceneManager.GetSceneByName("Main Menu").isLoaded)
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    
    public void UnloadPauseMenu()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("Pause Menu");
    }

    public void LoadSettings()
    {
        _settings.enabled = true;
        _menu.enabled = false;
    }
    
    public void UnloadSettings()
    {
        _menu.enabled = true;
        _settings.enabled = false;
    }
    
    public void LoadControls()
    {
        _controls.enabled = true;
        _menu.enabled = false;
    }
    
    public void UnloadControls()
    {
        _menu.enabled = true;
        _controls.enabled = false;
    }
}
