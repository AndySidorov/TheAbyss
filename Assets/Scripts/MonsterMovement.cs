using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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
    private NavMeshAgent _agent;
    private AudioSource _audio;
    
    // Анимации: 0 - idle, 1 - roar, 2 - run, 3 - flashed

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _audio = GetComponentInChildren<AudioSource>();
        
        _agent.speed = _monsterData.Speed;
        _agent.stoppingDistance = _monsterData.StoppingDistance;
        
        _audio.clip = _monsterSounds.Idle;
        _audio.loop = true;
        StartCoroutine(SoundRoutine());
    }

    private void Update()
    {
        if (isKilling) // Запустить убийство, если монстр достиг игрока (isKilling меняется скриптом на игроке)
            Kill();
        if (Time.timeScale != 0 && !_dontMove) // Если игра не на паузе или монстр не в режиме убийства
        {
            Move();
        }
    }

    private void Move()
    {
        if (!_isFlashCooldown) // Если монстр не ослеплен (нужен, чтобы не запускать корутин FlashRoutine бесконечно)
        {
            if (isFlashed) // Если игрок ослепил монстра (isFlashed меняется скриптом на игроке)
            {
                StartCoroutine(FlashRoutine());
            }
            else if (isChasing && !_isRoarCooldown) // Если монстр в режиме преследования (isChasing меняется скриптом на игроке) и перестал (или не начал) рычать
            {
                if (_isFirstTime) // Если еще не рычал
                {
                    var direction = playerPosition - transform.position;
                    transform.forward = new Vector3(direction.x, 0, direction.z); // Повернуться к игроку
                    StartCoroutine(ChaseRoutine());
                }
                else
                {
                    var monsterXZ = new Vector2(transform.position.x, transform.position.z); // Координаты монстра в плоскости XZ
                    var playerXZ = new Vector2(playerPosition.x, playerPosition.z); // Координаты игрока или бутылки в плоскости XZ
                    // Y не используется, потому что бутылка может удариться об потолок и не сработает код сброса цели
                    
                    if (Vector2.Distance(monsterXZ, playerXZ) < _monsterData.StoppingDistance) // Если монстр почти достиг цели, то ее надо сбросить
                    {
                        _agent.SetDestination(gameObject.transform.position); // Убрать цель (сразу тормозит таким образом)
                        
                        isChasing = false; // Отменить преследование
                        _isFirstTime = true; // Снова зарычит, если увидит
                        
                        _audio.clip = _monsterSounds.Idle;
                        _audio.loop = true;
                        _audio.Play();
                        
                        _animator.SetInteger("Status", 0); // Анимация спокойствия
                    }
                    else // Если монстр только закончил рычать или еще не достиг цель (последнюю переданную)
                    {
                        _agent.SetDestination(playerPosition); // Задать цель (playerPosition меняется скриптом на игроке, передается вместе с isChasing)
                    }
                }
            }
        }
    }
    
    // Монстр убивает
    private void Kill()
    {
        _agent.SetDestination(gameObject.transform.position); // Удаление цели (сразу тормозит таким образом)
        
        isKilling = false; // Не дать функции убийства запуститься несколько раз
        _dontMove = true; // Прервать все действия монстра
        
        var direction = playerPosition - transform.position;
        transform.forward = new Vector3(direction.x, 0, direction.z); // Повернуть монстра к нам
        
        _audio.clip = _monsterSounds.Roar;
        _audio.loop = false;
        _audio.Play();

        _animator.SetInteger("Status", 3); // Анимация убийства
    }
    
    // Монстр рычит и дает фору перед атакой
    private IEnumerator ChaseRoutine()
    {
        _isRoarCooldown = true; // Задержка на рык
        _isFirstTime = false; // Не дать корутину запускаться бесконечно
        
        _animator.SetInteger("Status", 1); // Анимация крика
        
        _audio.clip = _monsterSounds.Roar;
        _audio.loop = false;
        _audio.Play();
        
        yield return new WaitForSeconds(_monsterData.TimeBeforeChase);

        if (!_isFlashCooldown && !_dontMove) // Если монстра не успели ослепить
        {
            _isRoarCooldown = false; // Снять задержку на рык
        
            _animator.SetInteger("Status", 2); // Анимация бега
        
            _audio.clip = _monsterSounds.Run;
            _audio.loop = true;
            _audio.Play();
        }
    }
    
    // Монстр ослеплен на определенное время
    private IEnumerator FlashRoutine()
    {
        _agent.SetDestination(gameObject.transform.position); // Удаление цели (сразу тормозит таким образом)
        
        _isFlashCooldown = true; // Ослепление, прерывающее все действия (не запускать корутин ослепления бесконечно)
        _isRoarCooldown = false; // Прервать крик, если монстр ослеплен во время рыка
        isChasing = false; // Сбросить режим преследования
        
        _audio.clip = _monsterSounds.Flashed;
        _audio.loop = false;
        _audio.Play();
        
        _animator.SetInteger("Status", 4); //  Анимация ослепления
        
        yield return new WaitForSeconds(_monsterData.FlashCooldown);
        
        _isFlashCooldown = false; // Ослепление прошло
        _isFirstTime = true; // Монстр снова зарычит, если нас заметит
        isFlashed = false; // Корутин снова можно будет запустить
        
        _audio.clip = _monsterSounds.Idle;
        _audio.loop = true;
        _audio.Play();
        
        _animator.SetInteger("Status", 0); // Анимация спокойствия
    }
    
    // Монстры не начинают кряхтеть в один и тот же момент
    private IEnumerator SoundRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f)); // Ожидание от 0 до 3 секунд
        if (!_isFlashCooldown && !isChasing && !_dontMove) // Если ничего не произошло за это время
            _audio.Play();
    }
}
