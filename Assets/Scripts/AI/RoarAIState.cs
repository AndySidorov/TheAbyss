using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class RoarAIState : AIState
{
    private MonsterAI _monsterAI;
    
    private Coroutine _roarRoutine;
    
    public RoarAIState(NavMeshAgent agent, MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        _roarRoutine = CoroutineStarter.Instance.StartCoroutine(RoarRoutine());
    }

    public override void OnExit()
    {
        base.OnExit();
        
        if (_roarRoutine != null)
        {
            CoroutineStarter.Instance.StopCoroutine(_roarRoutine);
            _roarRoutine = null;
        }
        
        _monsterAI.isRoarCooldown = false;
    }

    public override void Update()
    {
        base.Update();

        // поворачиваемся к цели
        var monsterTransform = _agent.transform;
        var monsterPosition = monsterTransform.position;
        var direction = _monsterAI.targetPosition - monsterPosition;
        monsterTransform.forward = new Vector3(direction.x, 0, direction.z);
    }

    private IEnumerator RoarRoutine()
    {
        _monsterAI.isRoarCooldown = true;
        _monsterAI.animator.SetInteger("Status", 1); // Анимация крика
        MonsterSoundPlayer.Instance.RoarSound(_monsterAI.audioSource);
        yield return new WaitForSeconds(_monsterAI.monsterData.TimeBeforeChase);
        _monsterAI.isRoarCooldown = false;
        _roarRoutine = null;
    }
}
