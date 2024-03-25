using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Скрипт на MenuManager Empty GameObject отвечающий за загрузку уровней и меню
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
    
    // При загрузке меню сделать курсор видимым и активировать только главный Canvas, остальные отключить (если они есть)
    private void Awake()
    {
        Cursor.visible = true;
        
        if (_menu != null)
            _menu.gameObject.SetActive(true);
        if (_settings != null)
            _settings.gameObject.SetActive(false);
        if (_controls != null)
            _controls.gameObject.SetActive(false);

        _eventSystem.SetSelectedGameObject(_menuButton);
    }
    
    // Отключить курсор при выходе из меню
    private void OnDestroy()
    {
        Cursor.visible = false;
    }
    
    // Запустить сохраненный уровень (если есть) или дефолтный
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
    
    // Сбросить прогресс и восстановить дефолтные значения 
    public void RestartGame()
    {
        PlayerPrefs.SetFloat("Flash Distance", _playerData.FlashDistance);
        PlayerPrefs.SetFloat("Flash Cooldown", _playerData.FlashCooldown);
        PlayerPrefs.SetInt("Number of Flashes", _playerData.NumberOfFlashes);
        PlayerPrefs.SetInt("Number of Energy Drinks", _playerData.NumberOfEnergyDrinks);
        PlayerPrefs.SetInt("Number of Bottles", _playerData.NumberOfBottles);
        PlayerPrefs.SetString("Level", "Level 1");
        if (!SceneManager.GetSceneByName("Main Menu").isLoaded) // Если не в главном меню, то перекинуть туда
            SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    
    // Выйти из игры
    public void QuitGame()
    {
        Application.Quit();
    }
    
    // Загрузить главное меню
    public void LoadMainMenu()
    {
        Time.timeScale = 1; // Если переход был из меню паузы, то надо вернуть нормальное время
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    
    // Выйти из паузы
    public void UnloadPauseMenu()
    {
        Time.timeScale = 1; // Вернуть нормальное время
        SceneManager.UnloadSceneAsync("Pause Menu");
    }

    // Загрузить Canvas с настройками
    public void LoadSettings()
    {
        _settings.gameObject.SetActive(true);
        _menu.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_settingsButton);
    }
    
    // Отгрузить Canvas с настройками
    public void UnloadSettings()
    {
        _menu.gameObject.SetActive(true);
        _settings.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_menuButton);
    }
    
    // Загрузить Canvas с управлением
    public void LoadControls()
    {
        _controls.gameObject.SetActive(true);
        _menu.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_controlsButton);
    }
    
    // Отгрузить Canvas с управлением
    public void UnloadControls()
    {
        _menu.gameObject.SetActive(true);
        _controls.gameObject.SetActive(false);
        
        _eventSystem.SetSelectedGameObject(_menuButton);
    }
}
