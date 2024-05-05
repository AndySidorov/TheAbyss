using Save_System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private PlayerData _playerData;
    
    private float _playerVolume;
    private float _monsterVolume;
    private float _ambientVolume;
    private float _mouseSensitivity;

    [SerializeField] private Slider _playerSlider;
    [SerializeField] private Slider _monsterSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _mouseSensitivitySlider;

    [SerializeField] private TextMeshProUGUI _playerText;
    [SerializeField] private TextMeshProUGUI _monsterText;
    [SerializeField] private TextMeshProUGUI _ambientText;
    [SerializeField] private TextMeshProUGUI _mouseSensitivityText;
    
    private SaveLoadSystem _saveLoadSystem;
    
    private void Start()
    {
        _saveLoadSystem = SaveLoadSystem.Instance;
        var data = _saveLoadSystem.data;
        
        _playerVolume = data.playerVolume;
        _mixer.SetFloat("PlayerVolume", _playerVolume);
        _playerSlider.value = Mathf.InverseLerp(-80f, 20f, _playerVolume);
        _playerText.text = _playerVolume.ToString("F2");
        
        _monsterVolume = data.monsterVolume;
        _mixer.SetFloat("MonsterVolume", _monsterVolume);
        _monsterSlider.value = Mathf.InverseLerp(-80f, 20f, _monsterVolume);
        _monsterText.text = _monsterVolume.ToString("F2");
        
        _ambientVolume = data.ambientVolume;
        _mixer.SetFloat("AmbientVolume",_ambientVolume);
        _ambientSlider.value = Mathf.InverseLerp(-80f, 20f, _ambientVolume);
        _ambientText.text = _ambientVolume.ToString("F2");
        
        _mouseSensitivity = data.mouseSensitivity;
        _mouseSensitivitySlider.value = Mathf.InverseLerp(10, 500, _mouseSensitivity);
        _mouseSensitivityText.text = _mouseSensitivity.ToString("F2");
    }
    
    public void PlayerVolumeChange()
    {
        var volume = Mathf.Lerp(-80f, 20f, _playerSlider.value);
        _mixer.SetFloat("PlayerVolume", volume);
        _playerText.text = volume.ToString("F2");
        _saveLoadSystem.data.playerVolume = volume;
        _saveLoadSystem.SaveGame();
    }
    
    public void MonsterVolumeChange()
    {
        var volume = Mathf.Lerp(-80f, 20f, _monsterSlider.value);
        _mixer.SetFloat("MonsterVolume", volume);
        _monsterText.text = volume.ToString("F2");
        _saveLoadSystem.data.monsterVolume = volume;
        _saveLoadSystem.SaveGame();
    }
    
    public void AmbientVolumeChange()
    {
        var volume = Mathf.Lerp(-80f, 20f, _ambientSlider.value);
        _mixer.SetFloat("AmbientVolume", volume);
        _ambientText.text = volume.ToString("F2");
        _saveLoadSystem.data.ambientVolume = volume;
        _saveLoadSystem.SaveGame();
    }
    
    public void MouseSensitivityChange()
    {
        var value = Mathf.Lerp(10, 500, _mouseSensitivitySlider.value);
        _mouseSensitivityText.text = value.ToString("F2");
        _saveLoadSystem.data.mouseSensitivity = value;
        _saveLoadSystem.SaveGame();
    }
    
    public void DefaultSettings()
    {
        _playerSlider.value = Mathf.InverseLerp(-80f, 20f, 0);
        _mixer.SetFloat("PlayerVolume", 0);
        _playerText.text = 0.ToString("F2");
        _saveLoadSystem.data.playerVolume = 0;
        
        _monsterSlider.value = Mathf.InverseLerp(-80f, 20f, 0);
        _mixer.SetFloat("MonsterVolume", 0);
        _monsterText.text = 0.ToString("F2");
        _saveLoadSystem.data.monsterVolume = 0;
        
        _ambientSlider.value = Mathf.InverseLerp(-80f, 20f, 0);
        _mixer.SetFloat("AmbientVolume", 0);
        _ambientText.text = 0.ToString("F2");
        _saveLoadSystem.data.ambientVolume = 0;
        
        _mouseSensitivitySlider.value = Mathf.InverseLerp(10, 500, _playerData.MouseSensitivity);
        _mouseSensitivityText.text = _playerData.MouseSensitivity.ToString("F2");
        _saveLoadSystem.data.mouseSensitivity = _playerData.MouseSensitivity;
        
        _saveLoadSystem.SaveGame();
    }
}
