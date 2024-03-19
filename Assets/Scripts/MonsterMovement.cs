using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MonsterMovement : MonoBehaviour
{
    [SerializeField] private MonsterData _monsterData;
    [SerializeField] private MonsterSounds _monsterSounds;
    
    [HideInInspector] public bool isFlashed;
    [HideInInspector] public bool isChasing;
    [HideInInspector] public Vector3 playerPosition;
    [HideInInspector] public bool isKilling;
    
    private bool _isRoarCooldown;
    private bool _isFlashCooldown;
    private bool _isFirstTime = true;
    private bool _dontMove;
    
    private Animator _animator;
    private CharacterController _controller;
    private AudioSource _audio;
    
    //Status: 0 - idle, 1 - roar, 2 - run, 3 - flashed

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _audio = GetComponentInChildren<AudioSource>();
        
        _audio.clip = _monsterSounds.Idle;
        _audio.loop = true;
        StartCoroutine(SoundRoutine());
    }

    private void Update()
    {
        if (isKilling) 
            Kill();
        if (Time.timeScale != 0 && !_dontMove)
        {
            Move();
        }
    }

    private void Move()
    {
        _controller.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
        if (!_isFlashCooldown)
        {
            if (isFlashed)
            {
                StartCoroutine(FlashRoutine());
            }
            else if (isChasing && !_isRoarCooldown)
            {
                if (_isFirstTime)
                {
                    var direction = playerPosition - transform.position;
                    transform.forward = new Vector3(direction.x, 0, direction.z);
                    StartCoroutine(ChaseRoutine());
                }
                else
                {
                    var monsterXZ = new Vector2(transform.position.x, transform.position.z);
                    var playerXZ = new Vector2(playerPosition.x, playerPosition.z);
                    if (Vector2.Distance(monsterXZ, playerXZ) < 0.95f)
                    {
                        isChasing = false;
                        _isFirstTime = true;
                        
                        _audio.clip = _monsterSounds.Idle;
                        _audio.loop = true;
                        _audio.Play();
                        
                        _animator.SetInteger("Status", 0);
                    }
                    else
                    {
                        var direction = playerPosition - transform.position;
                        transform.forward = new Vector3(direction.x, 0, direction.z);
                        _controller.Move( transform.forward * (_monsterData.Speed * Time.deltaTime));
                    }
                }
            }
        }
    }

    private void Kill()
    {
        isKilling = false;
        _dontMove = true;
        
        var direction = playerPosition - transform.position;
        transform.forward = new Vector3(direction.x, 0, direction.z);
        
        _audio.clip = _monsterSounds.Roar;
        _audio.loop = false;
        _audio.Play();

        _animator.SetInteger("Status", 3);
    }

    private IEnumerator ChaseRoutine()
    {
        _isRoarCooldown = true;
        _isFirstTime = false;
        
        _animator.SetInteger("Status", 1);
        
        _audio.clip = _monsterSounds.Roar;
        _audio.loop = false;
        _audio.Play();
        
        yield return new WaitForSeconds(_monsterData.TimeBeforeChase);

        if (!_isFlashCooldown && !_dontMove)
        {
            _isRoarCooldown = false;
        
            _animator.SetInteger("Status", 2);
        
            _audio.clip = _monsterSounds.Run;
            _audio.loop = true;
            _audio.Play();
        }
    }
    
    private IEnumerator FlashRoutine()
    {
        _isFlashCooldown = true;
        _isRoarCooldown = false;
        
        _audio.clip = _monsterSounds.Flashed;
        _audio.loop = false;
        _audio.Play();
        
        _animator.SetInteger("Status", 4);
        
        yield return new WaitForSeconds(_monsterData.FlashCooldown);
        
        _isFlashCooldown = false;
        _isFirstTime = true;
        isFlashed = false;
        isChasing = false;
        
        _audio.clip = _monsterSounds.Idle;
        _audio.loop = true;
        _audio.Play();
        
        _animator.SetInteger("Status", 0);
    }

    private IEnumerator SoundRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        if (!_isFlashCooldown && !isChasing && !_dontMove)
            _audio.Play();
    }
}
