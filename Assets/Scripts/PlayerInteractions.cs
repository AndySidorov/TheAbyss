using System;
using System.Collections;
using Save_System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
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
    
    public event Action onDrink;

    private GameObject _interactable;
    
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
    private PlayerInputHandler _playerInput;
    private Light _light;
    private AudioSource _audio;
    
    private bool _isFlashCooldown;
    private bool _isThrowing;
    private bool _isPaused;
    
    private float _currentPower;
    
    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerInput = GetComponent<PlayerInputHandler>();
        _light = GetComponentInChildren<Light>();
        _audio = GetComponentInChildren<AudioSource>();
        
        _aimImage.sprite = _UIList.AimSprite;
        _eImage.enabled = false;
        _flashText.text = _numberOfFlashes.ToString();
        _energyDrinkText.text = _numberOfEnergyDrinks.ToString();
        _currentPower = 0;
        _powerImage.fillAmount = _currentPower;
        
        var data = SaveLoadSystem.Instance.data;
        _flashDistance = data.flashDistance;
        _flashCooldown = data.flashCooldown;
        _numberOfFlashes = data.numberOfFlashes;
        _numberOfEnergyDrinks = data.numberOfEnergyDrinks;
        _numberOfBottles = data.numberOfBottles;
        
        _light.range = _flashDistance;
        _light.intensity = _playerData.DefaultLightIntensity;
    }

    private void Update()
    {
        if (!_playerMovement.IsDead && Time.timeScale != 0) // Если игрок не умер и не на паузе
        {
            if (_isPaused) UnPause(); // Если был на паузе (TimeScale = 0), то запустит
            
            _flashText.text = _numberOfFlashes.ToString(); 
            _energyDrinkText.text = _numberOfEnergyDrinks.ToString();
            _bottleText.text = _numberOfBottles.ToString();
            
            Take();
            Throw();
        }
    }
    
    private void OnEnable()
    {
        _playerInput.onThrowPressed += OnThrowPressed;
        _playerInput.onThrowReleased += OnThrowReleased;
        
        _playerInput.onThrowStop += OnThrowStop;
        
        _playerInput.onFlash += OnFlash;
        
        _playerInput.onTake += OnTake;
        
        _playerInput.onDrink += OnDrink;
        
        _playerInput.onPause += OnPause;
    }
    
    private void OnDisable()
    {
        _playerInput.onThrowPressed -= OnThrowPressed;
        _playerInput.onThrowReleased -= OnThrowReleased;
        
        _playerInput.onThrowStop -= OnThrowStop;
        
        _playerInput.onFlash -= OnFlash;
        
        _playerInput.onTake -= OnTake;
        
        _playerInput.onDrink -= OnDrink;
        
        _playerInput.onPause -= OnPause;
    }

    private void OnThrowPressed()
    {
        if (!_playerMovement.IsDead && !_isPaused && _numberOfBottles > 0)
        {
            _isThrowing = true;
        }
    }

    private void OnThrowReleased()
    {
        if (!_playerMovement.IsDead && !_isPaused && _isThrowing)
        {
            _isThrowing = false;
            _numberOfBottles -= 1;
            var bottle = Instantiate(_bottleThrowPrefab, _head.position + _head.forward * 0.8f, Quaternion.identity);
            bottle.gameObject.GetComponent<Rigidbody>().AddForce(_head.forward * _currentPower);
            _currentPower = 0;
        }
    }

    private void OnThrowStop()
    {
        if (!_playerMovement.IsDead && !_isPaused)
        {
            _isThrowing = false;
            _currentPower = 0;
        }
    }

    private void OnFlash()
    {
        if (!_playerMovement.IsDead && !_isPaused && _numberOfFlashes > 0 && !_isFlashCooldown)
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
                        if (Physics.Raycast(ray, out rayHit)) // Запустить рейкаст для обнаружения препятствий
                        {
                            if (rayHit.collider.gameObject.CompareTag("Monster"))
                            {
                                hitGameObject.GetComponentInParent<MonsterAI>().isFlashed = true;
                            }
                        }
                    }
                }
            }
            StartCoroutine(FlashRoutine());
        }
    }

    private void OnTake()
    {
        if (!_playerMovement.IsDead && !_isPaused && _interactable != null)
        {
            _audio.PlayOneShot(_playerSounds.Collect);
            var item = _interactable.GetComponent<Interactable>();
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
            
            _interactable = null;
        }
    }

    private void OnDrink()
    {
        if (!_playerMovement.IsDead && !_isPaused && _numberOfEnergyDrinks > 0)
        {
            _audio.PlayOneShot(_playerSounds.Drink);
            onDrink?.Invoke(); // На скрипте PlayerMovement запуcкаем функцию
            _numberOfEnergyDrinks -= 1;
        }
    }

    private void OnPause()
    {
        if (!_playerMovement.IsDead && !_isPaused)
        {
            Time.timeScale = 0;
        
            foreach (var audioSource in _playerMovement.AllAudio) // Поставить на паузу все звуки
            {
                audioSource.Pause();
            }

            _currentPower = 0;
            _powerImage.fillAmount = 0;
            _isThrowing = false;
            _isPaused = true; 
        
            SceneManager.LoadScene("Pause Menu", LoadSceneMode.Additive);
        }
    }
    
    private void Take()
    {
        _eImage.enabled = false;
        _interactable = null;
        var ray = new Ray(_head.position + _head.forward * 0.8f, _head.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.2f))
        {
            var obj = hit.collider.gameObject;
            if (obj.CompareTag("Interactable"))
            {
                _eImage.enabled = true;
                _interactable = obj;
            }
        }
    }
    
    private void Throw()
    {
        if (_isThrowing)
        {
            _currentPower = Mathf.Clamp(_currentPower + (_playerData.PowerIncreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxPower);
        }
        
        _powerImage.fillAmount = _currentPower / _playerData.MaxPower;
    }
    
    private void UnPause()
    {
        foreach (var audioSource in _playerMovement.AllAudio)
        {
            audioSource.UnPause();
        }
        
        _playerMovement.mouseSensitivity = SaveLoadSystem.Instance.data.mouseSensitivity;

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