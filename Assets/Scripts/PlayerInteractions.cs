using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
    
    // Upgradeable Stats
    private float _flashDistance;
    private float _flashCooldown;
    private int _numberOfFlashes;
    private int _numberOfEnergyDrinks;
    private int _numberOfBottles;

    public float FlashDistance => _flashDistance;
    public float FlashCooldown => _flashCooldown;
    public int NumberOfFlashes => _numberOfFlashes;
    public int NumberOfEnergyDrinks => _numberOfEnergyDrinks;
    public int NumberOfBottles => _numberOfBottles;
    
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerSounds _playerSounds;
    [SerializeField] private UIList _UIList;
    [SerializeField] private Transform _head;
    [SerializeField] private Image _aimImage;
    [SerializeField] private Image _eImage;
    [SerializeField] private TextMeshProUGUI _flashText;
    [SerializeField] private TextMeshProUGUI _energyDrinkText;
    [SerializeField] private TextMeshProUGUI _bottleText;
    [SerializeField] private Image _powerImage;
    
    [SerializeField] private BottleThrow _bottleThrowPrefab;
    
    private PlayerMovement _playerMovement;
    private Light _light;
    private AudioSource _audio;
    
    private bool _isFlashCooldown;
    private bool _isThrowing;
    private bool _isPaused;
    
    private float _currentPower;
    
    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _light = GetComponentInChildren<Light>();
        _audio = GetComponentInChildren<AudioSource>();
        
        _aimImage.sprite = _UIList.AimSprite;
        _eImage.enabled = false;
        _flashText.text = _numberOfFlashes.ToString();
        _energyDrinkText.text = _numberOfEnergyDrinks.ToString();
        _currentPower = 0;
        _powerImage.fillAmount = _currentPower;
        
        _flashDistance = PlayerPrefs.HasKey("Flash Distance") ? PlayerPrefs.GetFloat("Flash Distance") : _playerData.FlashDistance;
        _flashCooldown = PlayerPrefs.HasKey("Flash Cooldown") ? PlayerPrefs.GetFloat("Flash Cooldown") : _playerData.FlashCooldown;
        _numberOfFlashes = PlayerPrefs.HasKey("Number of Flashes") ? PlayerPrefs.GetInt("Number of Flashes") : _playerData.NumberOfFlashes;
        _numberOfEnergyDrinks = PlayerPrefs.HasKey("Number of Energy Drinks") ? PlayerPrefs.GetInt("Number of Energy Drinks") : _playerData.NumberOfEnergyDrinks;
        _numberOfBottles = PlayerPrefs.HasKey("Number of Bottles") ? PlayerPrefs.GetInt("Number of Bottles") : _playerData.NumberOfBottles;

        _light.range = _flashDistance;
        _light.intensity = _playerData.DefaultLightIntensity;
    }

    private void Update()
    {
        if (!_playerMovement.IsDead && Time.timeScale != 0)
        {
            if (_isPaused) UnPause();
                
            _flashText.text = _numberOfFlashes.ToString();
            _energyDrinkText.text = _numberOfEnergyDrinks.ToString();
            _bottleText.text = _numberOfBottles.ToString();
            
            Take();
            Throw();
            if (Input.GetMouseButtonDown(0) && _numberOfFlashes > 0 && !_isFlashCooldown) Flash();
            if (Input.GetKeyDown(KeyCode.R)  && _numberOfEnergyDrinks > 0) Drink();
            
            if (Input.GetKeyDown(KeyCode.Escape)) Pause();
        }
    }
    
    private void Take()
    {
        _eImage.enabled = false;
        var ray = new Ray(_head.position + _head.forward * 0.8f, _head.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.2f))
        {
            var obj = hit.collider.gameObject;
            if (obj.CompareTag("Interactable"))
            {
                _eImage.enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _audio.PlayOneShot(_playerSounds.Collect);
                    var item = obj.GetComponent<Interactable>();
                    switch(item.itemName)
                    {
                        case "Battery":
                            _numberOfFlashes += _playerData.NumberOfFlashesToAdd;
                            break;
                        case "Energy Drink":
                            _numberOfEnergyDrinks += _playerData.NumberOfEnergyDrinksToAdd;
                            break;
                        case "Bottle":
                            _numberOfBottles += _playerData.NumberOfBottlesToAdd;
                            break;
                        case "Chip":
                            _flashDistance += _playerData.FlashDistanceToAdd;
                            _light.range = _flashDistance;
                            break;
                        case "Wires":
                            if (_flashCooldown - _playerData.FlashCooldownToDecrease >= 0)
                                _flashCooldown -= _playerData.FlashCooldownToDecrease;
                            break;
                    }
                    item.OnInteraction();
                }
            }
        }
    }
    
    private void Drink()
    {
        _audio.PlayOneShot(_playerSounds.Drink);
        _playerMovement.isDrinking = true;
        _numberOfEnergyDrinks -= 1;
    }
    
    private void Flash()
    {
        _numberOfFlashes--;
        _audio.PlayOneShot(_playerSounds.Flash);
        StartCoroutine(LightRoutine());
        var playerPosition = new Vector3(transform.position.x, _head.position.y, transform.position.z);
        var hits = Physics.OverlapSphere(playerPosition, _flashDistance);
        foreach (var hit in hits)
        {
            var hitGameObject = hit.gameObject;
            if (hitGameObject.CompareTag("Monster"))
            {
                var monsterPosition = hitGameObject.GetComponent<Transform>().position;
                var direction = monsterPosition - playerPosition;
                if (Vector3.Angle(_head.forward, direction) <= 45)
                {
                    var ray = new Ray(playerPosition, direction);
                    RaycastHit rayHit;
                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.gameObject.CompareTag("Monster"))
                        {
                            hitGameObject.GetComponentInParent<MonsterMovement>().isFlashed = true;
                        }
                    }
                }
            }
        }
        StartCoroutine(FlashRoutine());
    }

    private void Throw()
    {
        if (Input.GetMouseButtonDown(2) && _numberOfBottles > 0)
        {
            _isThrowing = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            if (_isThrowing)
            {
                _isThrowing = false;
                _numberOfBottles -= 1;
                var bottle = Instantiate(_bottleThrowPrefab, _head.position + _head.forward * 0.8f, Quaternion.identity);
                bottle.gameObject.GetComponent<Rigidbody>().AddForce(_head.forward * _currentPower);
                _currentPower = 0;
            }
        }

        if (_isThrowing)
        {
            _currentPower = Mathf.Clamp(_currentPower + (_playerData.PowerIncreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxPower);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _isThrowing = false;
            _currentPower = 0;
        }
        
        _powerImage.fillAmount = _currentPower / _playerData.MaxPower;
    }

    private void Pause()
    {
        Time.timeScale = 0;
        
        foreach (var audioSource in _playerMovement.AllAudio)
        {
            audioSource.Pause();
        }

        _currentPower = 0;
        _powerImage.fillAmount = 0;
        _isThrowing = false;
        _isPaused = true;
        
        SceneManager.LoadScene("Pause Menu", LoadSceneMode.Additive);
    }
    
    private void UnPause()
    {
        foreach (var audioSource in _playerMovement.AllAudio)
        {
            audioSource.UnPause();
        }
        
        _playerMovement.mouseSensitivity = PlayerPrefs.HasKey("Mouse Sensitivity") ? PlayerPrefs.GetFloat("Mouse Sensitivity") : _playerData.MouseSensitivity;

        _isPaused = false;
    }
    
    private IEnumerator FlashRoutine()
    {
        _isFlashCooldown = true;
        _aimImage.sprite = _UIList.ReloadSprite;
        yield return new WaitForSeconds(_flashCooldown);
        _isFlashCooldown = false;
        _aimImage.sprite = _UIList.AimSprite;
    }
    
    private IEnumerator LightRoutine()
    {
        _light.intensity = _playerData.FlashLightIntensity;
        yield return new WaitForSeconds(0.5f);
        _light.intensity = _playerData.DefaultLightIntensity;
    }
}
