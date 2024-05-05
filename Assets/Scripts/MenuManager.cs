using System;
using Save_System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem;
    
    [SerializeField] private Canvas _menu;
    [SerializeField] private Canvas _settings;
    [SerializeField] private Canvas _controls;
    
    [SerializeField] private GameObject _menuButton;
    [SerializeField] private GameObject _settingsButton;
    [SerializeField] private GameObject _controlsButton;
    
    [SerializeField] private PlayerData _playerData;

    private SaveLoadSystem _saveLoadSystem;
    
    private void Awake()
    {
        _saveLoadSystem = SaveLoadSystem.Instance;
        
        Cursor.visible = true;
        
        if (_menu != null)
            _menu.gameObject.SetActive(true);
        if (_settings != null)
            _settings.gameObject.SetActive(false);
        if (_controls != null)
            _controls.gameObject.SetActive(false);

        _eventSystem.SetSelectedGameObject(_menuButton);
    }
    
    private void OnDestroy()
    {
        Cursor.visible = false;
    }
    
    public void LoadGame()
    {
        SceneManager.LoadScene(_saveLoadSystem.data.sceneName, LoadSceneMode.Single);
    }
    
    public void RestartGame()
    {
        _saveLoadSystem.data.flashDistance = _playerData.FlashDistance;
        _saveLoadSystem.data.flashCooldown = _playerData.FlashCooldown;
        _saveLoadSystem.data.numberOfFlashes = _playerData.NumberOfFlashes;
        _saveLoadSystem.data.numberOfEnergyDrinks = _playerData.NumberOfEnergyDrinks;
        _saveLoadSystem.data.numberOfBottles = _playerData.NumberOfBottles;
        _saveLoadSystem.data.sceneName = "Level 1";
        _saveLoadSystem.SaveGame();
        
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
        _settings.gameObject.SetActive(true);
        _menu.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_settingsButton);
    }
    
    public void UnloadSettings()
    {
        _menu.gameObject.SetActive(true);
        _settings.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_menuButton);
    }
    
    public void LoadControls()
    {
        _controls.gameObject.SetActive(true);
        _menu.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_controlsButton);
    }
    
    public void UnloadControls()
    {
        _menu.gameObject.SetActive(true);
        _controls.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_menuButton);
    }
}
