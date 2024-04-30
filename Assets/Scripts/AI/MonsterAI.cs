using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [SerializeField] public CoroutineStarter coroutineStarter;
    [SerializeField] public MonsterData monsterData;
    [SerializeField] public MonsterSounds monsterSounds;
    
    public StateMachine stateMachine;
    
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public Animator animator;

    [HideInInspector] public bool isFlashed;
    [HideInInspector] public bool isChasing;
    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public bool isKilling;
    
    [HideInInspector] public bool isRoarCooldown;
    [HideInInspector] public bool isFlashCooldown;
    [HideInInspector] public bool isFirstTime = true;
    [HideInInspector] public bool dontMove;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine();
        
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        
        agent.speed = monsterData.Speed;
        agent.stoppingDistance = monsterData.StoppingDistance;

        coroutineStarter = CoroutineStarter.Instance;
        
        audioSource.clip = monsterSounds.Idle;
        audioSource.loop = true;
        StartCoroutine(SoundRoutine());
        
        // states
        var chaseState = new ChaseAIState(agent, this);
        stateMachine.AddState(chaseState);

        var flashedState = new FlashedAIState(agent, this);
        stateMachine.AddState(flashedState);

        var killState = new KillAIState(agent, this);
        stateMachine.AddState(killState);

        var roarState = new RoarAIState(agent, this);
        stateMachine.AddState(roarState);
        
        var wanderState = new WanderAIState(agent, this);
        stateMachine.AddState(wanderState);
        
        // conditions and transitions
        var wanderToRoarCondition = new Condition(() => !isFlashCooldown && isChasing && !isRoarCooldown && isFirstTime);
        stateMachine.AddTransition(wanderState, roarState, wanderToRoarCondition);

        var wanderToFlashedCondition = new Condition(() => !isFlashCooldown && isFlashed);
        stateMachine.AddTransition(wanderState, flashedState, wanderToFlashedCondition);

        var roarToChaseCondition = new Condition(() => !isFlashCooldown && isChasing && !isRoarCooldown);
        stateMachine.AddTransition(roarState, chaseState, roarToChaseCondition);

        var roarToFlashedCondition = new Condition(() => !isFlashCooldown && isFlashed);
        stateMachine.AddTransition(roarState, flashedState, roarToFlashedCondition);
        
        var roarToKillCondition = new Condition(() => isKilling);
        stateMachine.AddTransition(roarState, killState, roarToKillCondition);

        var flashedToWanderCondition = new Condition(() => !isFlashCooldown && !isFlashed); 
        stateMachine.AddTransition(flashedState, wanderState, flashedToWanderCondition);
        
        var chaseToKillCondition = new Condition(() => !isChasing && isKilling);
        stateMachine.AddTransition(chaseState, killState, chaseToKillCondition);
        
        var chaseToFlashedCondition = new Condition(() => !isFlashCooldown && isFlashed);
        stateMachine.AddTransition(chaseState, flashedState, chaseToFlashedCondition);
        
        var chaseToWanderCondition = new Condition(() => !isChasing);
        stateMachine.AddTransition(chaseState, wanderState, chaseToWanderCondition);
        
        stateMachine.SetInitialState(wanderState);
    }

    private void Update()
    {
        stateMachine.Update();
    }
    
    private IEnumerator SoundRoutine()
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        if (!isFlashCooldown && !isChasing && !dontMove)
            audioSource.Play();
    }
}
