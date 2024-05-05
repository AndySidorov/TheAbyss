using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [SerializeField] public MonsterData monsterData;
    
    private StateMachine stateMachine;

    public Vector3 Position => transform.position;
    
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public Animator animator;
    
    [HideInInspector] public Vector3 targetPosition;

    [HideInInspector] public bool isFlashed;
    [HideInInspector] public bool isChasing;
    [HideInInspector] public bool isKilling;
    
    [HideInInspector] public bool isRoarCooldown;

    private void Awake()
    {
        stateMachine = new StateMachine();
        
        agent = GetComponent<NavMeshAgent>();
        
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        
        agent.speed = monsterData.Speed;
        agent.stoppingDistance = monsterData.StoppingDistance;
        
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
        var wanderToRoarCondition = new Condition(() => isChasing);
        stateMachine.AddTransition(wanderState, roarState, wanderToRoarCondition);

        var wanderToFlashedCondition = new Condition(() => isFlashed);
        stateMachine.AddTransition(wanderState, flashedState, wanderToFlashedCondition);
        
        var wanderToKillCondition = new Condition(() => isKilling);
        stateMachine.AddTransition(wanderState, killState, wanderToKillCondition);

        var roarToChaseCondition = new Condition(() => !isRoarCooldown);
        stateMachine.AddTransition(roarState, chaseState, roarToChaseCondition);

        var roarToFlashedCondition = new Condition(() => isFlashed);
        stateMachine.AddTransition(roarState, flashedState, roarToFlashedCondition);
        
        var roarToKillCondition = new Condition(() => isKilling);
        stateMachine.AddTransition(roarState, killState, roarToKillCondition);

        var flashedToWanderCondition = new Condition(() => !isFlashed); 
        stateMachine.AddTransition(flashedState, wanderState, flashedToWanderCondition);
        
        var chaseToKillCondition = new Condition(() => isKilling);
        stateMachine.AddTransition(chaseState, killState, chaseToKillCondition);
        
        var chaseToFlashedCondition = new Condition(() => isFlashed);
        stateMachine.AddTransition(chaseState, flashedState, chaseToFlashedCondition);
        
        var chaseToWanderCondition = new Condition(() => !isChasing);
        stateMachine.AddTransition(chaseState, wanderState, chaseToWanderCondition);
        
        stateMachine.SetInitialState(wanderState);
    }

    private void Update()
    {
        stateMachine.Update();
    }
}
