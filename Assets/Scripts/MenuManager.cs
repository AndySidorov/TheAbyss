using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Скрипт на MenuManager Empty GameObject отвечающий за загрузку уровней и меню
public class MenuManager : MonoBehaviour
{
    [SerializeField] private Canvas _menu;
    [SerializeField] private Canvas _settings;
    [SerializeField] private Canvas _controls;
    [SerializeField] private PlayerData _playerData;
    
    // При загрузке меню сделать курсор видимым и активировать только главный Canvas, остальные отключить (если они есть)
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
        _settings.enabled = true;
        _menu.enabled = false;
    }
    
    // Отгрузить Canvas с настройками
    public void UnloadSettings()
    {
        _menu.enabled = true;
        _settings.enabled = false;
    }
    
    // Загрузить Canvas с управлением
    public void LoadControls()
    {
        _controls.enabled = true;
        _menu.enabled = false;
    }
    
    // Отгрузить Canvas с управлением
    public void UnloadControls()
    {
        _menu.enabled = true;
        _controls.enabled = false;
    }
}
