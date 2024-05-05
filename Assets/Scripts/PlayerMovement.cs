using System.Collections;
using System.Collections.Generic;
using Save_System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
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
        
        _currentStamina = _playerData.MaxStamina;
        
        _radii = new List<float>(){_playerData.KillingZoneRadius, _playerData.WalkHearingZoneRadius};
        
        // Записываем последние значения на случай, если игрок появится в воздухе
        _lastSpeed = _playerData.WalkSpeed;
        _lastZoneRadius = _playerData.WalkHearingZoneRadius;
        
        _audio.clip = _playerSounds.Idle;
        _audio.loop = true;
        _audio.Play();
        
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
    
    private void Move()
    {
        var input = new Vector3(
            _moveVector.x,
            0,
            _moveVector.y
        ).normalized;
        
        _isMoving = input != Vector3.zero; // Проверить, двигается ли игрок
        
        int status; // Анимации: 0 - idle, 1 - isWalking, 2 - isRunning, 3 - isSneaking, 4 - isJumping

        if (!_isGrounded || _isJumping)
        {
            status = 4; // Задать анимацию isJumping
            
            _currentSpeed = _lastSpeed;
            _currentZoneRadius = _lastZoneRadius;
            
            _isJumping = true;
        }
        else
        {
            if (_isMoving)
            {
                if (_isRunning && _currentStamina > 0)
                {
                    status = 2; // Анимация бега
                    
                    _currentSpeed = _playerData.RunSpeed;
                    _currentZoneRadius = _playerData.RunHearingZoneRadius;
                    
                    _currentStamina = Mathf.Clamp(_currentStamina - (_playerData.StaminaDecreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxStamina);
                    _staminaRegenTimer = 0.0f;

                    if (_audio.clip != _playerSounds.Run)
                    {
                        _audio.clip = _playerSounds.Run;
                        _audio.loop = true;
                        _audio.Play();
                    }
                }
                else if (_isSneaking)
                {
                    status = 3; // Анимация подкрадывания
                    
                    _currentSpeed = _playerData.SneakSpeed;
                    _currentZoneRadius = _playerData.SneakHearingZoneRadius;

                    if (_audio.clip != _playerSounds.Sneak)
                    {
                        _audio.clip = _playerSounds.Sneak;
                        _audio.loop = true;
                        _audio.Play();
                    }
                }
                else
                {
                    status = 1; // Анимация шага
                    
                    _currentSpeed = _playerData.WalkSpeed;
                    _currentZoneRadius = _playerData.WalkHearingZoneRadius;

                    if (_audio.clip != _playerSounds.Walk)
                    {
                        _audio.clip = _playerSounds.Walk;
                        _audio.loop = true;
                        _audio.Play();
                    }
                }
            }
            else
            {
                status = 0; // Анимация idle
                
                _currentSpeed = _playerData.WalkSpeed;
                _currentZoneRadius = _playerData.IdleHearingZoneRadius;
                    
                if (_audio.clip != _playerSounds.Idle)
                {
                    _audio.clip = _playerSounds.Idle;
                    _audio.loop = true;
                    _audio.Play();
                }
            }
            
            _lastSpeed = _currentSpeed;
            _lastZoneRadius = _currentZoneRadius;
        }
        
        if (_currentStamina < _playerData.MaxStamina && (!_isRunning || !_isMoving || _isJumping))
        {
            if (_staminaRegenTimer >= _playerData.StaminaTimeToRegen)
                _currentStamina = Mathf.Clamp(_currentStamina + (_playerData.StaminaIncreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxStamina);
            else
                _staminaRegenTimer += Time.deltaTime;
        }

        _controller.Move( transform.rotation * input * (_currentSpeed * Time.deltaTime));
        _animator.SetInteger("Status", status);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            if (!_isJumpingExtraTime)
            {
                _verticalVelocity = -2f;

                if (_isJumping)
                {
                    _isJumping = false;
                    
                    _currentZoneRadius = _lastZoneRadius * _playerData.LandingHearingZoneCoefficient;
                    
                    _audio.PlayOneShot(_playerSounds.Jumped);
                }
            }

            if (_isJumpPressed)
            {
                _isJumpPressed = false;
                _isJumping = true;
                
                float jumpHeight;

                if (_isRunning && _currentStamina - _playerData.RunJumpStaminaDecrease >= 0)
                {
                    _currentSpeed = _playerData.RunSpeed;
                    _currentZoneRadius = _playerData.RunHearingZoneRadius;
                    jumpHeight = _playerData.RunJumpHeight;
                    
                    _currentStamina -= _playerData.RunJumpStaminaDecrease;
                    _staminaRegenTimer = 0.0f;
                }
                else if (_isSneaking)
                {
                    _currentSpeed = _playerData.SneakSpeed;
                    _currentZoneRadius = _playerData.SneakHearingZoneRadius;
                    jumpHeight = _playerData.SneakJumpHeight;
                }
                else
                {
                    _currentSpeed = _playerData.WalkSpeed;
                    _currentZoneRadius = _playerData.WalkHearingZoneRadius;
                    jumpHeight = _playerData.WalkJumpHeight;
                }
                
                _lastSpeed = _currentSpeed;
                _lastZoneRadius = _currentZoneRadius;
                
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -3.0f * _gravityValue);

                StartCoroutine(JumpingRoutine()); // Даем дополнительное время, чтобы оторваться от земли
                
                _audio.clip = _playerSounds.Jump;
                _audio.loop = false;
                _audio.Play();
            }
        }

        _verticalVelocity += _gravityValue * Time.deltaTime;
        
        _staminaImage.fillAmount = _currentStamina / _playerData.MaxStamina;
        _radii[1] = _currentZoneRadius;
        
        _controller.Move(new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);
    }
    
    private void Look()
    {
        var input = -_lookVector.y;
        var rotation = _head.localEulerAngles;
        rotation.x += input * Time.deltaTime * mouseSensitivity;
        if (rotation.x > 180) rotation.x -= 360;
        rotation.x = Mathf.Clamp(rotation.x, -58, 58);
        _head.localEulerAngles = rotation;
    }
    
    private void Turn()
    {
        var input = _lookVector.x;
        transform.Rotate(0, input * Time.deltaTime * mouseSensitivity, 0);
    }
    
    private void Zone()
    {
        var i = 0;
        foreach (var radius in _radii)
        {
            if (_isDead) break;
            
            var hits = Physics.OverlapSphere(transform.position, radius);
            
            foreach (var hit in hits)
            {
                if (_isDead) break;
                
                var hitGameObject = hit.gameObject;
                
                if (hitGameObject.CompareTag("Monster"))
                {
                    var monsterAI = hitGameObject.GetComponentInParent<MonsterAI>();
                    var monsterPosition = monsterAI.Position;
                    var direction = monsterPosition - transform.position;
                    
                    if (!monsterAI.isFlashed)
                    {
                        switch (i)
                        {
                            case 0: // Зона убийства
                                monsterAI.targetPosition = transform.position;
                                
                                transform.forward = new Vector3(direction.x, 0, direction.z); // Поворачиваемся лицом к монстру
                                _head.forward = monsterPosition - _head.position;
                                _head.localEulerAngles += new Vector3(-40f, 0, 0);

                                foreach (var audioSource in _allAudio) // Останавливаем все звуки на фоне
                                { 
                                    audioSource.Stop();
                                }
                                        
                                _animator.SetInteger("Status", 0); // Меняем аниимацию на idle 
                                
                                monsterAI.isKilling = true;
                                _isDead = true;
                                        
                                StartCoroutine(KillingRoutine());
                                break;
                                    
                            case 1: // Зона обнаружения
                                monsterAI.isChasing = true;
                                monsterAI.targetPosition = transform.position;
                                break;
                        }
                    }
                }
            }
            i++;
        }
    }
    
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
    
    private IEnumerator KillingRoutine()
    {
        yield return new WaitForSeconds(_playerData.WaitForDeath);
        SceneManager.LoadScene("Death Menu", LoadSceneMode.Single);
    }
}