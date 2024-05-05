using System.Collections;
using System.Collections.Generic;
using Save_System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Также связан с PlayerInteractions (может поменять значение после паузы)
    public float mouseSensitivity;
    
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerSounds _playerSounds;
    [SerializeField] private Transform _head;
    [SerializeField] private Image _staminaImage;

    private PlayerInteractions _playerInteractions;
    private PlayerInputHandler _playerInput;
    private CharacterController _controller;
    private Animator _animator;
    private AudioSource _audio;
    private AudioSource[] _allAudio;
    
    // Массив со всеми аудиосорсами
    public AudioSource[] AllAudio => _allAudio;

    private float _currentSpeed;
    private float _currentZoneRadius;
    private float _currentStamina;
    private float _staminaRegenTimer;
    private float _verticalVelocity;

    private Vector2 _moveVector;
    private Vector2 _lookVector;
    private bool _isJumpPressed;
    
    // Последние значения перед прыжком
    private float _lastSpeed; 
    private float _lastZoneRadius; 
    
    private readonly float _gravityValue = -9.81f;
    private readonly float _killingZoneRadius = 1.2f;
    
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isMoving;
    private bool _isRunning;
    private bool _isSneaking;
    private bool _isJumpingExtraTime; // Дополнительное время, чтобы рейкаст не прибил к земле в начале прыжка
    private bool _isDead;
    
    // Параметры, связанные с PlayerInteractions
    public bool IsDead => _isDead;
    
    // Радиусы для сфер пересечений
    private List<float> _radii;
    
    private void Awake()
    {
        _playerInteractions = GetComponent<PlayerInteractions>();
        _playerInput = GetComponent<PlayerInputHandler>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _audio = GetComponentInChildren<AudioSource>();
        _allAudio = FindObjectsOfType<AudioSource>(); // Нужно, чтобы выключить все звуки при нажатии паузы или при смерти
        
        // Изначально задать максимальную стамину
        _currentStamina = _playerData.MaxStamina;
        
        _radii = new List<float>(){_killingZoneRadius, _playerData.WalkHearingZoneRadius}; // Радиусы для сфер пересечений
        
        // Записываем последние значения на случай, если игрок появится в воздухе
        _lastSpeed = _playerData.WalkSpeed;
        _lastZoneRadius = _playerData.WalkHearingZoneRadius;
        
        // Запустить обычный звук
        _audio.clip = _playerSounds.Idle;
        _audio.loop = true;
        _audio.Play();
        
        // Задать сохраненное (если есть) или дефолтное значение чувствительности
        mouseSensitivity = SaveLoadSystem.Instance.data.mouseSensitivity;
    }

    private void Update()
    {
        if (!_isDead) // Забрать управление, если убит
        {
            if (Time.timeScale != 0)
            {
                _isGrounded = CheckGrounded();
                Move();
                Jump();
                Look();
                Turn();
                Zone();
            }
        }
    }
    
    private void OnEnable()
    {
        _playerInteractions.onDrink += OnDrink;
        
        _playerInput.onMove += OnMove;
        _playerInput.onLook += OnLook;
        
        _playerInput.onRunPressed += OnRunPressed;
        _playerInput.onRunReleased += OnRunReleased;
        _playerInput.onSneakPressed += OnSneakPressed;
        _playerInput.onSneakReleased += OnSneakReleased;
        
        _playerInput.onJump += OnJump;
    }
    
    private void OnDisable()
    {
        _playerInteractions.onDrink -= OnDrink;
        
        _playerInput.onMove -= OnMove;
        _playerInput.onLook -= OnLook;
        
        _playerInput.onRunPressed -= OnRunPressed;
        _playerInput.onRunReleased -= OnRunReleased;
        _playerInput.onSneakPressed -= OnSneakPressed;
        _playerInput.onSneakReleased -= OnSneakReleased;
        
        _playerInput.onJump -= OnJump;
    }

    private void OnDrink()
    {
        _currentStamina = _playerData.MaxStamina;
        _staminaRegenTimer = 0.0f;
    }

    private void OnMove(Vector2 input)
    {
        _moveVector = input;
    }
    
    private void OnLook(Vector2 input)
    {
        _lookVector = input;
    }

    private void OnRunPressed()
    {
        _isRunning = true;
        _isSneaking = false; // Сбросить подкрадывание, если игрок не отпустил ctrl
    }
    
    private void OnRunReleased()
    {
        _isRunning = false;
    }
    
    private void OnSneakPressed()
    {
        _isSneaking = true;
        _isRunning = false; // Сбросить бег, если игрок не отпустил shift
    }
    
    private void OnSneakReleased()
    {
        _isSneaking = false;
    }

    private void OnJump()
    {
        if (Time.timeScale != 0 && _isGrounded)
        {
            _isJumpPressed = true;
        }
    }
    
    // Все, связанное с передвижением
    private void Move()
    {
        var input = new Vector3(
            _moveVector.x,
            0,
            _moveVector.y
        ).normalized;
        
        // Проверить, двигается ли игрок (чтобы просто так не тратить стамину и использовать нужную анимацию)
        _isMoving = input != Vector3.zero;
        
        // Анимации: 0 - idle, 1 - isWalking, 2 - isRunning, 3 - isSneaking, 4 - isJumping
        int status;

        if (!_isGrounded || _isJumping) // Если игрок не на земле или в режиме прыжка
        {
            status = 4; // Задать анимацию isJumping
            
            _currentSpeed = _lastSpeed; // Поменять текущую скорость на последнюю имевшуюся до прыжка (чтоб игрок не менял скорость во время прыжка)
            _currentZoneRadius = _lastZoneRadius; // Поменять текущую зону обнаружения на последнюю имевшуюся
            
            _isJumping = true; // Меняем значение на true, если падаем (чтобы был звук приземления)
        }
        else // Если игрок не прыгает
        {
            if (_isMoving) // Если игрок двигается
            {
                if (_isRunning && _currentStamina > 0) // Если игрок в режиме бега и у него еще есть стамина
                {
                    status = 2; // Анимация бега
                    
                    _currentSpeed = _playerData.RunSpeed; // Поменять текущую скорость на скорость бега
                    _currentZoneRadius = _playerData.RunHearingZoneRadius; // Поменять текущую зону обнаружения

                    // Уменьшить текущее количество стамины (не выходя за рамки)
                    _currentStamina = Mathf.Clamp(_currentStamina - (_playerData.StaminaDecreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxStamina);
                    _staminaRegenTimer = 0.0f; // Сбросить таймер восстановления стамины (нужно снова ждать начала восстановления)

                    if (_audio.clip != _playerSounds.Run) // Если еще не играет звук бега (чтобы не запускать его с начала каждый раз)
                    {
                        _audio.clip = _playerSounds.Run;
                        _audio.loop = true;
                        _audio.Play();
                    }
                }
                else if (_isSneaking) // Если игрок в режиме подкрадывания
                {
                    status = 3; // Анимация подкрадывания
                    
                    _currentSpeed = _playerData.SneakSpeed;
                    _currentZoneRadius = _playerData.SneakHearingZoneRadius;

                    if (_audio.clip != _playerSounds.Sneak) // Если еще не играет звук подкрадывания (чтобы не запускать его с начала каждый раз)
                    {
                        _audio.clip = _playerSounds.Sneak;
                        _audio.loop = true;
                        _audio.Play();
                    }
                }
                else // Иначе игрок идет
                {
                    status = 1; // Анимация шага
                    
                    _currentSpeed = _playerData.WalkSpeed;
                    _currentZoneRadius = _playerData.WalkHearingZoneRadius;

                    if (_audio.clip != _playerSounds.Walk) // Проверяем включен ли звук шага
                    {
                        _audio.clip = _playerSounds.Walk;
                        _audio.loop = true;
                        _audio.Play();
                    }
                }
            }
            else // Иначе игрок стоит
            {
                status = 0; // Анимация idle
                
                _currentSpeed = _playerData.WalkSpeed;
                _currentZoneRadius = _playerData.IdleHearingZoneRadius;
                    
                if (_audio.clip != _playerSounds.Idle) // Проверяем включен ли звук
                {
                    _audio.clip = _playerSounds.Idle;
                    _audio.loop = true;
                    _audio.Play();
                }
            }
            
            // Сохраняем последние значения до падения, чтобы они использовались во время полета
            _lastSpeed = _currentSpeed;
            _lastZoneRadius = _currentZoneRadius;
        }
        
        // Если текущая стамина меньше максимально возможной, а игрок не бежит или не двигается (даже с нажатой кнопкой shift), прыгает (не прерывать стамину, если игрок нажал shift в полете)
        if (_currentStamina < _playerData.MaxStamina && (!_isRunning || !_isMoving || _isJumping))
        {
            if (_staminaRegenTimer >= _playerData.StaminaTimeToRegen) // Если прошло нужное количество времени для регена, то прибавляем стамину
                _currentStamina = Mathf.Clamp(_currentStamina + (_playerData.StaminaIncreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxStamina);
            else // Если не прошло, то прибавляем прошедшее время
                _staminaRegenTimer += Time.deltaTime;
        }

        _controller.Move( transform.rotation * input * (_currentSpeed * Time.deltaTime)); // Двигаем игрока с нужной скоростью
        _animator.SetInteger("Status", status); // Ставим нужную анимацию
    }

    private void Jump()
    {
        if (_isGrounded) // Если сработал рейкаст
        {
            if (!_isJumpingExtraTime) // Если мы не переходим на прыжок
            {
                _verticalVelocity = -2f; // Прилипаем к земле, чтобы игрок не подскакивал на каждой кочке

                if (_isJumping) // Если мы уже были в прыжке (приземляемся)
                {
                    _isJumping = false;
                    
                    // Увеличиваем зону обнаружения на нужный коэффициент (звук громче при приземлении)
                    _currentZoneRadius = _lastZoneRadius * _playerData.LandingHearingZoneCoefficient;
                    
                    _audio.PlayOneShot(_playerSounds.Jumped); // Сыграть звук приземления
                }
            }

            if (_isJumpPressed) // Если нажат space
            {
                _isJumpPressed = false;
                _isJumping = true; // Переходим на прыжок
                
                float jumpHeight;

                if (_isRunning && _currentStamina - _playerData.RunJumpStaminaDecrease >= 0) // Если игрок в режиме бега и у него еще есть нужная стамина
                {
                    _currentSpeed = _playerData.RunSpeed;
                    _currentZoneRadius = _playerData.RunHearingZoneRadius;
                    jumpHeight = _playerData.RunJumpHeight;
                    
                    _currentStamina -= _playerData.RunJumpStaminaDecrease; // Отнять стамину
                    _staminaRegenTimer = 0.0f; // Сбросить таймер восстановления стамины (нужно снова ждать начала восстановления)
                }
                else if (_isSneaking) // Если игрок в режиме подкрадывания
                {
                    _currentSpeed = _playerData.SneakSpeed;
                    _currentZoneRadius = _playerData.SneakHearingZoneRadius;
                    jumpHeight = _playerData.SneakJumpHeight;
                }
                else // Если игрок стоит или идет
                {
                    _currentSpeed = _playerData.WalkSpeed;
                    _currentZoneRadius = _playerData.WalkHearingZoneRadius;
                    jumpHeight = _playerData.WalkJumpHeight;
                }
                
                // Сохраняем последние значения до падения, чтобы они использовались во время полета
                _lastSpeed = _currentSpeed;
                _lastZoneRadius = _currentZoneRadius;
                
                // Задаем правильную вертикальную скорость по формуле (исходя из текущей высоты прыжка)
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -3.0f * _gravityValue);

                StartCoroutine(JumpingRoutine()); // Даем дополнительное время, чтобы оторваться от земли
                
                _audio.clip = _playerSounds.Jump;
                _audio.loop = false;
                _audio.Play();
            }
        }

        _verticalVelocity += _gravityValue * Time.deltaTime; // Обновляем вертикальную скорость
        
        // После всего кода в Move и Jump
        _staminaImage.fillAmount = _currentStamina / _playerData.MaxStamina; // Отображаем правильное количество стамины в интерфейсе
        _radii[1] = _currentZoneRadius; // Сохраняем нужный радиус обнаружения в массив для удобства
        
        _controller.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime); // Двигаем игрока вертикально
    }
    
    // Просмотр вверх и вниз
    private void Look()
    {
        var input = -_lookVector.y;
        var rotation = _head.localEulerAngles;
        rotation.x += input * Time.deltaTime * mouseSensitivity;
        if (rotation.x > 180) rotation.x -= 360;
        rotation.x = Mathf.Clamp(rotation.x, -58, 58); // Не уходить за эти рамки, чтобы не мешала моделька игрока
        _head.localEulerAngles = rotation;
    }
    
    // Просмотр влево и вправо, поворот модельки
    private void Turn()
    {
        var input = _lookVector.x;
        transform.Rotate(0, input * Time.deltaTime * mouseSensitivity, 0);
    }
    
    // Проверка всех зон обнаружения
    private void Zone()
    {
        var i = 0; // Параметр для определения, какая это зона
        foreach (var radius in _radii) // Пробегаемся по радиусам
        {
            var hits = Physics.OverlapSphere(transform.position, radius); // Создаем сферкаст с нужным радиусом
            foreach (var hit in hits) // Для всех пойманных объектов
            {
                var hitGameObject = hit.gameObject;
                if (hitGameObject.CompareTag("Monster")) // Если тэг монстр
                {
                    var monsterAI = hitGameObject.GetComponentInParent<MonsterAI>(); // Взаимодействуем со скриптом монстра
                    var monsterPosition = hitGameObject.GetComponent<Transform>().position; // Местоположение головы монстра
                    var direction = monsterPosition - transform.position;
                    if (!monsterAI.isFlashed) // Если монстр не ослеплен
                    {
                        switch (i) // Проверяем, в какой он зоне
                        {
                            case 0: // Зона убийства
                                transform.forward = new Vector3(direction.x, 0, direction.z); // Поворачиваемся лицом к монстру
                                _head.forward = monsterPosition - _head.position;
                                _head.localEulerAngles += new Vector3(30f, 0, 0);

                                foreach (var audioSource in _allAudio) // Останавливаем все звуки на фоне
                                { 
                                    audioSource.Stop();
                                }
                                        
                                _animator.SetInteger("Status", 0); // Меняем аниимацию на idle 
                                        
                                monsterAI.isKilling = true; // Меняем состояние монстра на убийство
                                _isDead = true; // Забираем у игрока управление 
                                        
                                StartCoroutine(KillingRoutine()); // Ждем скример и запускаем меню смерти
                                break;
                                    
                            case 1: // Зона обнаружения
                                monsterAI.isChasing = true; // Переводим монстра в режим преследования
                                monsterAI.targetPosition = transform.position; // Задаем ему позицию игрока
                                break;
                        }
                    }
                }
            }
            i++; // Следующая зона
        }
    }
    
    // Сферкаст, проверка на нахождение на земле
    private bool CheckGrounded()
    {
        var ray = new Ray(transform.position, Vector3.down);
        return Physics.SphereCast(ray, _controller.radius, 0.8f);
    }
    
    // Дополнительное время для отрыва от земли
    private IEnumerator JumpingRoutine()
    {
        _isJumpingExtraTime = true;
        yield return new WaitForSeconds(0.1f);
        _isJumpingExtraTime = false;
    }
    
    // Время скримера и загрузка меню смерти
    private IEnumerator KillingRoutine()
    {
        yield return new WaitForSeconds(_playerData.WaitForDeath);
        SceneManager.LoadScene("Death Menu", LoadSceneMode.Single);
    }
}