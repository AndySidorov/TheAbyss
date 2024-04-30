using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// Скрипт на MenuManager Empty GameObject отвечающий за настройки
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
    
    // Задать сохраненные (если есть) или дефолтные громкости звука и чувствительность мыши, подвинуть ползунки на нужные места, отобразить числа
    private void Start()
    {
        _playerVolume = PlayerPrefs.HasKey("Player Volume") ? PlayerPrefs.GetFloat("Player Volume") : 0;
        _mixer.SetFloat("PlayerVolume", _playerVolume);
        _playerSlider.value = Mathf.InverseLerp(-80f, 20f, _playerVolume);
        _playerText.text = _playerVolume.ToString("F2");
        
        _monsterVolume = PlayerPrefs.HasKey("Monster Volume") ? PlayerPrefs.GetFloat("Monster Volume") : 0;
        _mixer.SetFloat("MonsterVolume", _monsterVolume);
        _monsterSlider.value = Mathf.InverseLerp(-80f, 20f, _monsterVolume);
        _monsterText.text = _monsterVolume.ToString("F2");
        
        _ambientVolume = PlayerPrefs.HasKey("Ambient Volume") ? PlayerPrefs.GetFloat("Ambient Volume") : 0;
        _mixer.SetFloat("AmbientVolume",_ambientVolume);
        _ambientSlider.value = Mathf.InverseLerp(-80f, 20f, _ambientVolume);
        _ambientText.text = _ambientVolume.ToString("F2");
        
        _mouseSensitivity = PlayerPrefs.HasKey("Mouse Sensitivity") ? PlayerPrefs.GetFloat("Mouse Sensitivity") : _playerData.MouseSensitivity;
        _mouseSensitivitySlider.value = Mathf.InverseLerp(10, 500, _mouseSensitivity);
        _mouseSensitivityText.text = _mouseSensitivity.ToString("F2");
    }
    
    // Срабатывают при изменении положения ползунка: меняют громкость звука или чувствительность, отображаемые числа, сохраняют настройки
    
    public void PlayerVolumeChange()
    {
        var volume = Mathf.Lerp(-80f, 20f, _playerSlider.value);
        _mixer.SetFloat("PlayerVolume", volume);
        PlayerPrefs.SetFloat("Player Volume", volume);
        _playerText.text = volume.ToString("F2");
    }
    
    public void MonsterVolumeChange()
    {
        var volume = Mathf.Lerp(-80f, 20f, _monsterSlider.value);
        _mixer.SetFloat("MonsterVolume", volume);
        PlayerPrefs.SetFloat("Monster Volume", volume);
        _monsterText.text = volume.ToString("F2");
    }
    
    public void AmbientVolumeChange()
    {
        var volume = Mathf.Lerp(-80f, 20f, _ambientSlider.value);
        _mixer.SetFloat("AmbientVolume", volume);
        PlayerPrefs.SetFloat("Ambient Volume", volume);
        _ambientText.text = volume.ToString("F2");
    }
    
    public void MouseSensitivityChange()
    {
        var value = Mathf.Lerp(10, 500, _mouseSensitivitySlider.value);
        PlayerPrefs.SetFloat("Mouse Sensitivity", value);
        _mouseSensitivityText.text = value.ToString("F2");
    }
    
    // Сброс к дефолтным настройкам при нажатии кнопки: дефолтные громкость звука и чувствительность, отображаемые числа, сохраняются дефолтные настройки
    public void DefaultSettings()
    {
        _playerSlider.value = Mathf.InverseLerp(-80f, 20f, 0);
        _mixer.SetFloat("PlayerVolume", 0);
        PlayerPrefs.SetFloat("Player Volume", 0);
        _playerText.text = 0.ToString("F2");
        
        _monsterSlider.value = Mathf.InverseLerp(-80f, 20f, 0);
        _mixer.SetFloat("MonsterVolume", 0);
        PlayerPrefs.SetFloat("Monster Volume", 0);
        _monsterText.text = 0.ToString("F2");
        
        _ambientSlider.value = Mathf.InverseLerp(-80f, 20f, 0);
        _mixer.SetFloat("AmbientVolume", 0);
        PlayerPrefs.SetFloat("Ambient Volume", 0);
        _ambientText.text = 0.ToString("F2");
        
        _mouseSensitivitySlider.value = Mathf.InverseLerp(10, 500, _playerData.MouseSensitivity);
        PlayerPrefs.SetFloat("Mouse Sensitivity", _playerData.MouseSensitivity);
        _mouseSensitivityText.text = _playerData.MouseSensitivity.ToString("F2");
    }
}
