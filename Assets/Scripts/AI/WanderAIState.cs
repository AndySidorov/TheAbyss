using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WanderAIState : AIState
{
    private MonsterAI _monsterAI;
    
    private Coroutine _idleOrWanderRoutine;
    
    private Vector3 _destination;
    
    private bool _isIdle;
    private bool _coroutineIsStart;
    private bool _destinationIsSet;
    
    public WanderAIState(NavMeshAgent agent,  MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        _isIdle = true;
    }

    public override void OnExit()
    {
        base.OnExit();
        
        if (_idleOrWanderRoutine != null)
        {
            CoroutineStarter.Instance.StopCoroutine(_idleOrWanderRoutine);
            _idleOrWanderRoutine = null;
        }
        
        _coroutineIsStart = false;
        _destinationIsSet = false;
        
        _agent.speed = _monsterAI.monsterData.Speed;
        _agent.SetDestination(_agent.transform.position);
    }

    public override void Update()
    {
        base.Update();
        
        if (_isIdle)
        {
            Idle();
        }
        else
        {
            if (!_destinationIsSet)
            {
                if (RandomPoint(_agent.transform.position, _monsterAI.monsterData.WalkRange, out _destination))
                {
                    _destinationIsSet = true;
                    _agent.speed = _monsterAI.monsterData.WalkSpeed;
                    
                    _monsterAI.animator.SetInteger("Status", 5); // Анимация ходьбы
                    MonsterSoundPlayer.Instance.WalkSound(_monsterAI.audioSource);

                    _agent.SetDestination(_destination);
                }
                else
                {
                    _isIdle = true;
                }
            }
            else
            {
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
        
        // запускаем корутину, определяет стоит или идет
        if (!_coroutineIsStart && !_destinationIsSet)
        {
            _idleOrWanderRoutine = CoroutineStarter.Instance.StartCoroutine(IdleOrWanderRoutine());
        }
    }

    private void Idle()
    {
        MonsterSoundPlayer.Instance.IdleSound(_monsterAI.audioSource);
        _monsterAI.animator.SetInteger("Status", 0); // Анимация спокойствия
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (var i = 0; i < 30; i++)
        {
            var randomInsideSphere = Random.insideUnitSphere;
            var randomPoint = center + range * new Vector3(randomInsideSphere.x, 0, randomInsideSphere.z);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 4.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    

    IEnumerator IdleOrWanderRoutine()
    {
        _coroutineIsStart = true;
        yield return new WaitForSeconds(Random.Range(_monsterAI.monsterData.MinTimeBeforeChoice, _monsterAI.monsterData.MaxTimeBeforeChoice));
        _isIdle = Random.Range(0f, 1f) > _monsterAI.monsterData.ChanceOfWalk;
        _coroutineIsStart = false;
        _idleOrWanderRoutine = null;
    }
}
