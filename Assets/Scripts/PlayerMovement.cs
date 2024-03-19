using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    
    // Settings
    public float mouseSensitivity;
    
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerSounds _playerSounds;
    [SerializeField] private Transform _head;
    [SerializeField] private Image _staminaImage;
    
    private CharacterController _controller;
    private Animator _animator;
    private AudioSource _audio;
    private AudioSource[] _allAudio;

    public AudioSource[] AllAudio => _allAudio;

    private float _currentSpeed;
    private float _currentJumpHeight;
    private float _currentZoneRadius;
    private float _currentStamina;
    private float _staminaRegenTimer;
    private float _verticalVelocity;
    
    // During jump
    private float _lastZoneRadius; 
    private float _lastSpeed; 
    
    private readonly float _gravityValue = -9.81f;
    private readonly float _killingZoneRadius = 1.2f;
    
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isMoving;
    private bool _isRunning;
    private bool _isSneaking;
    private bool _isJumpingExtraTime; // extra time for landing
    private bool _isDead;
    
    public bool IsDead => _isDead;
    [HideInInspector] public bool isDrinking;
    
    private List<float> _radii;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _audio = GetComponentInChildren<AudioSource>();
        _allAudio = FindObjectsOfType<AudioSource>();
        
        _lastSpeed = _playerData.DefaultSpeed;
        _lastZoneRadius = _playerData.DefaultHearingZoneRadius;
        _currentStamina = _playerData.MaxStamina;
        _radii = new List<float>(){_killingZoneRadius, _playerData.DefaultHearingZoneRadius};

        _audio.clip = _playerSounds.Idle;
        _audio.loop = true;
        _audio.Play();
        
        mouseSensitivity = PlayerPrefs.HasKey("Mouse Sensitivity") ? PlayerPrefs.GetFloat("Mouse Sensitivity") : _playerData.MouseSensitivity;
    }

    private void Update()
    {
        if (!_isDead)
        {
            CheckKeyDown();
            if (Time.timeScale != 0)
            {
                Move();
                Jump();
                Look();
                Turn();
                Zone();
            }
        }
    }

    private void CheckKeyDown()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isRunning = true;
            _isSneaking = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isRunning = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isSneaking = true;
            _isRunning = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _isSneaking = false;
        }
    }

    private void Move()
    {
        _currentSpeed = _playerData.DefaultSpeed;
        _currentJumpHeight = _playerData.DefaultJumpHeight;
        _currentZoneRadius = _playerData.DefaultHearingZoneRadius;
        _isGrounded = CheckGrounded();
        
        var input = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;
        
        _isMoving = input != Vector3.zero;
        
        // Animations: 0 - idle, 1 - isWalking, 2 - isRunning, 3 - isSneaking, 4 - isJumping
        var status = _isMoving ? 1:0;

        if (!_isGrounded || _isJumpingExtraTime)
        {
            status = 4;
            _currentSpeed = _lastSpeed;
            _currentZoneRadius = _lastZoneRadius;
            _isJumping = true;
        }
        else if (_isMoving)
        {
            _currentZoneRadius = _playerData.WalkHearingZoneRadius;
            
            if (_isRunning && _currentStamina > 0)
            {
                status = 2;
                _currentSpeed = _playerData.RunSpeed;
                _currentJumpHeight = _playerData.RunJumpHeight;
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
                status = 3;
                _currentSpeed = _playerData.SneakSpeed;
                _currentJumpHeight = _playerData.SneakJumpHeight;
                _currentZoneRadius = _playerData.SneakHearingZoneRadius;

                if (_audio.clip != _playerSounds.Sneak)
                {
                    _audio.clip = _playerSounds.Sneak;
                    _audio.loop = true;
                    _audio.Play();
                }
            }
            else if (_audio.clip != _playerSounds.Walk)
            {
                _audio.clip = _playerSounds.Walk;
                _audio.loop = true;
                _audio.Play();
            }
            
            _lastSpeed = _currentSpeed;
            _lastZoneRadius = _currentZoneRadius;
        }
        else if (_audio.clip != _playerSounds.Idle)
        {
            _audio.clip = _playerSounds.Idle;
            _audio.loop = true;
            _audio.Play();
        }
        
        if (_currentStamina < _playerData.MaxStamina && (!_isRunning || !_isMoving))
        {
            if (_staminaRegenTimer >= _playerData.StaminaTimeToRegen)
                _currentStamina = Mathf.Clamp(_currentStamina + (_playerData.StaminaIncreasePerFrame * Time.deltaTime), 0.0f, _playerData.MaxStamina);
            else
                _staminaRegenTimer += Time.deltaTime;
        }

        if (isDrinking)
        {
            _currentStamina = _playerData.MaxStamina;
            _staminaRegenTimer = 0.0f;
            isDrinking = false;
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _verticalVelocity = Mathf.Sqrt(_currentJumpHeight * -3.0f * _gravityValue);
                _isJumping = true;
                _lastZoneRadius = _currentZoneRadius;
                _lastSpeed = _currentSpeed;
                
                StartCoroutine(JumpingRoutine());
                
                if (_isRunning && _isMoving && _currentStamina > 0)
                {
                    _currentStamina -= _playerData.RunJumpStaminaDecrease;
                    if (_currentStamina < 0) _currentStamina = 0;
                }
                
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
        var input = -Input.GetAxis("Mouse Y");
        var rotation = _head.localEulerAngles;
        rotation.x += input * Time.deltaTime * mouseSensitivity;
        if (rotation.x > 180) rotation.x -= 360;
        rotation.x = Mathf.Clamp(rotation.x, -58, 58);
        _head.localEulerAngles = rotation;
    }

    private void Turn()
    {
        var input = Input.GetAxis("Mouse X");
        transform.Rotate(0, input * Time.deltaTime * mouseSensitivity, 0);
    }

    private void Zone()
    {
        var i = 0;
        foreach (var radius in _radii)
        {
            var hits = Physics.OverlapSphere(transform.position, radius);
            foreach (var hit in hits)
            {
                var hitGameObject = hit.gameObject;
                if (hitGameObject.CompareTag("Monster"))
                {
                    var monsterPosition = hitGameObject.GetComponent<Transform>().position;
                    var direction = monsterPosition - transform.position;
                    var ray = new Ray(transform.position, direction);
                    RaycastHit rayHit;
                    if (Physics.Raycast(ray, out rayHit))
                    {
                        if (rayHit.collider.gameObject.CompareTag("Monster"))
                        {
                            var monsterMovement = hitGameObject.GetComponentInParent<MonsterMovement>();
                            switch (i)
                            {
                                case 0:
                                    if (!monsterMovement.isFlashed)
                                    {
                                        transform.forward = new Vector3(direction.x, 0, direction.z);
                                        _head.forward = monsterPosition - _head.position;
                                        _head.localEulerAngles += new Vector3(30f, 0, 0);

                                        foreach (var audioSource in _allAudio)
                                        {
                                            audioSource.Stop();
                                        }
                                        
                                        _animator.SetInteger("Status", 0);
                                        
                                        monsterMovement.isKilling = true;
                                        _isDead = true;
                                        
                                        StartCoroutine(KillingRoutine());
                                    }
                                    break;
                                case 1:
                                    monsterMovement.isChasing = true;
                                    monsterMovement.playerPosition = transform.position;
                                    break;
                            }
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
