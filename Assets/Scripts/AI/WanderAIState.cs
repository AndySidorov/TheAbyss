using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderAIState : AIState
{
    private Vector3 _destination;
    private bool _isIdle;
    private MonsterAI _monsterAI;
    private bool _coroutineIsStart;
    private bool _destinationIsSet;
    
    public WanderAIState(NavMeshAgent agent,  MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
        _isIdle = true;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered Wander State");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exited Wander State");
        _agent.speed = _monsterAI.monsterData.Speed;
        _destinationIsSet = false;
    }

    public override void Update()
    {
        base.Update();
        
        // запускаем корутину, определяет стоит или идет
        if(!_coroutineIsStart && !_destinationIsSet) CoroutineStarter.Instance.StartRoutine(IdleOrWanderRoutine());
        
        if (_isIdle)
        {
            Idle();
        }
        else
        {
            if (!_destinationIsSet)
            {
                NavMeshHit hit;
                Again:
                var randomInsideSphere = Random.insideUnitSphere;
                var randomPoint = _agent.transform.position + Random.Range(10f, 30f) 
                    * new Vector3(randomInsideSphere.x, 0, randomInsideSphere.z);
                if (NavMesh.SamplePosition(randomPoint, out hit, 4f, NavMesh.AllAreas))
                {
                    _destinationIsSet = true;
                    _destination = hit.position;

                    _agent.speed = _monsterAI.monsterData.Speed / 2;
                    _monsterAI.animator.SetInteger("Status", 5); // Анимация ходьбы

                    MonsterSoundPlayer.Instance.WalkSound(_monsterAI.audioSource);

                    _agent.SetDestination(_destination);
                    // Debug.Log(_destination);
                }
                else goto Again;
                    
            }
            else _agent.SetDestination(_destination);
            
            var monsterPosition = _agent.transform.position;
            var monsterXZ = new Vector2(monsterPosition.x, monsterPosition.z); 
            var targetXZ = new Vector2(_destination.x, _destination.z);
            if (Vector2.Distance(monsterXZ, targetXZ) < _agent.stoppingDistance)
            {
                _agent.SetDestination(_agent.transform.position);
                _destinationIsSet = false;
                _isIdle = true;
            }
            
        } 
    }

    private void Idle()
    {
        MonsterSoundPlayer.Instance.IdleSound(_monsterAI.audioSource);
        
        _monsterAI.animator.SetInteger("Status", 0); // Анимация спокойствия
    }
    

    IEnumerator IdleOrWanderRoutine()
    {
        _coroutineIsStart = true;
        yield return new WaitForSeconds(Random.Range(2f, 6f));
        _isIdle = Random.Range(0f, 1f) > 0.8f;
        _coroutineIsStart = false;
    }
    
}
