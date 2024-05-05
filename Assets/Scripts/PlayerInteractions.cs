using System;
using System.Collections;
using Save_System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
    
    // Улучшаемые параметры
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
    [SerializeField] private TextMeshProUGUI _flashText; // Текст количества предметов
    [SerializeField] private TextMeshProUGUI _energyDrinkText;
    [SerializeField] private TextMeshProUGUI _bottleText;
    [SerializeField] private Image _powerImage; // Картинка силы броска
    
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
        
        _aimImage.sprite = _UIList.AimSprite; // Поставить в центр интерфейса картинку прицела
        _eImage.enabled = false; // Отключить картинку с буквой Е
        _flashText.text = _numberOfFlashes.ToString();
        _energyDrinkText.text = _numberOfEnergyDrinks.ToString();
        _currentPower = 0; // Занулить текущую силу броска
        _powerImage.fillAmount = _currentPower; // Занулить картинку силы броска
        
        // Загрузить сохраненные характеристики и предметы (если есть) или дефолтные
        var data = SaveLoadSystem.Instance.data;
        _flashDistance = data.flashDistance;
        _flashCooldown = data.flashCooldown;
        _numberOfFlashes = data.numberOfFlashes;
        _numberOfEnergyDrinks = data.numberOfEnergyDrinks;
        _numberOfBottles = data.numberOfBottles;
        
        // Подстроить освещение под характеристики (светом обозначается радиус поражения), задать нужную яркость
        _light.range = _flashDistance;
        _light.intensity = _playerData.DefaultLightIntensity;
    }

    private void Update()
    {
        if (!_playerMovement.IsDead && Time.timeScale != 0) // Если игрок не умер и не на паузе
        {
            if (_isPaused) UnPause(); // Если был на паузе (TimeScale = 0), то запустит оставшиеся параметры
            
            // Обновить количество ресурсов в интерфейсе
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
        if (!_playerMovement.IsDead && !_isPaused && _numberOfBottles > 0) // Если есть бутылки
        {
            _isThrowing = true; // Задать состояние броска
        }
    }

    private void OnThrowReleased()
    {
        if (!_playerMovement.IsDead && !_isPaused && _isThrowing) // Если все еще в состоянии броска
        {
            _isThrowing = false;
            _numberOfBottles -= 1;
            var bottle = Instantiate(_bottleThrowPrefab, _head.position + _head.forward * 0.8f, Quaternion.identity); // Заспавнить бутылку
            bottle.gameObject.GetComponent<Rigidbody>().AddForce(_head.forward * _currentPower); // Придать ей накопленное ускорение
            _currentPower = 0; // Занулить силы
        }
    }

    private void OnThrowStop()
    {
        if (!_playerMovement.IsDead && !_isPaused)
        {
            // Отменить бросок и занулить силы
            _isThrowing = false;
            _currentPower = 0;
        }
    }

    private void OnFlash()
    {
        if (!_playerMovement.IsDead && !_isPaused && _numberOfFlashes > 0 && !_isFlashCooldown)
        {
            _numberOfFlashes--;
        
            _audio.PlayOneShot(_playerSounds.Flash); // Звук вспышки
            StartCoroutine(LightRoutine()); // Временно увеличить интенсивность света
        
            var playerPosition = new Vector3(transform.position.x, _head.position.y, transform.position.z);
            var hits = Physics.OverlapSphere(playerPosition, _flashDistance); // Запустить сферу пересечения
            foreach (var hit in hits)
            {
                var hitGameObject = hit.gameObject;
                if (hitGameObject.CompareTag("Monster")) // Если попался тэг монстр
                {
                    var monsterPosition = hitGameObject.GetComponent<Transform>().position;
                    var direction = monsterPosition - playerPosition;
                    if (Vector3.Angle(_head.forward, direction) <= 45) // И угол между взглядом и монстром меньше 45 градусов
                    {
                        var ray = new Ray(playerPosition, direction);
                        RaycastHit rayHit;
                        if (Physics.Raycast(ray, out rayHit)) // Запустить рейкаст для обнаружения препятствий
                        {
                            if (rayHit.collider.gameObject.CompareTag("Monster")) // Если тэг снова монстр (нет преград)
                            {
                                hitGameObject.GetComponentInParent<MonsterAI>().isFlashed = true; // Ослепляем монстра
                            }
                        }
                    }
                }
            }
            StartCoroutine(FlashRoutine()); // Запускаем перезарядку
        }
    }

    private void OnTake()
    {
        if (!_playerMovement.IsDead && !_isPaused && _interactable != null) // Если видели объект
        {
            _audio.PlayOneShot(_playerSounds.Collect); // Звук сбора предмета
            var item = _interactable.GetComponent<Interactable>(); // Взаимодействуем со скриптом
            switch(item.itemName) // Узнаем, что за предмет и меняем характеристики
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
            item.OnInteraction(); // Действие предмета после подбирания
            
            _interactable = null; // Удаляем ссылку
        }
    }

    private void OnDrink()
    {
        if (!_playerMovement.IsDead && !_isPaused && _numberOfEnergyDrinks > 0)
        {
            _audio.PlayOneShot(_playerSounds.Drink); // Звук питья
            onDrink?.Invoke(); // На скрипте PlayerMovement запуcкаем функцию
            _numberOfEnergyDrinks -= 1;
        }
    }

    private void OnPause()
    {
        if (!_playerMovement.IsDead && !_isPaused)
        {
            Time.timeScale = 0; // Остановить время
        
            foreach (var audioSource in _playerMovement.AllAudio) // Поставить на паузу все звуки
            {
                audioSource.Pause();
            }

            _currentPower = 0; // Занулить силу
            _powerImage.fillAmount = 0; // Убрать картинку силы
            _isThrowing = false; // Отменить бросок
            _isPaused = true; // Поставить параметр паузы (сбросится в апдейте)
        
            SceneManager.LoadScene("Pause Menu", LoadSceneMode.Additive); // Загрузить меню паузы, не отгружая игру
        }
    }
    
    // Взятие предметов
    private void Take()
    {
        _eImage.enabled = false; // Изначально Е не отображается
        _interactable = null;
        var ray = new Ray(_head.position + _head.forward * 0.8f, _head.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.2f)) // Проверяем вещи перед собой рейкастом
        {
            var obj = hit.collider.gameObject;
            if (obj.CompareTag("Interactable")) // Если на нем есть скрипт Interactable
            {
                _eImage.enabled = true; // Включаем картинку с Е
                _interactable = obj; // Сохраняем ссылку на объект
            }
        }
    }
    
    // Бросок бутылки
    private void Throw()
    {
        if (_isThrowing) // Если в режиме бросания, то прибавить силы со временем
        {
            _currentPower = Mathf.Clamp(_currentPower + (_playerData.PowerIncreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxPower);
        }
        
        _powerImage.fillAmount = _currentPower / _playerData.MaxPower; // Отобразить процент силы броска на картинке
    }
    
    // MenuManager вернет TimeScale = 1 при выходе из паузы, на апдейте запустится эта функция
    private void UnPause()
    {
        foreach (var audioSource in _playerMovement.AllAudio) // Запустить звуки
        {
            audioSource.UnPause();
        }
        
        // Проверить чувствительность мыши и передать ее в PlayerMovement
        _playerMovement.mouseSensitivity = PlayerPrefs.HasKey("Mouse Sensitivity") ? PlayerPrefs.GetFloat("Mouse Sensitivity") : _playerData.MouseSensitivity;

        _isPaused = false; // Убрать параметр паузы
    }
    
    // Перезарядка
    private IEnumerator FlashRoutine()
    {
        _isFlashCooldown = true;
        _aimImage.sprite = _UIList.ReloadSprite; // Загрузить картинку перезарядки
        yield return new WaitForSeconds(_flashCooldown);
        _isFlashCooldown = false;
        _aimImage.sprite = _UIList.AimSprite; // Загрузить картинку прицела
    }
    
    // Изменение яркости при вспышке
    private IEnumerator LightRoutine()
    {
        _light.intensity = _playerData.FlashLightIntensity;
        yield return new WaitForSeconds(0.5f);
        _light.intensity = _playerData.DefaultLightIntensity;
    }
}