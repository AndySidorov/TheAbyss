using System.Collections;
using UnityEngine.AI;
using UnityEngine;

public class RoarAIState : AIState
{
    private MonsterAI _monsterAI;
    
    public RoarAIState(NavMeshAgent agent, MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered Roar State");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exited Roar State");
    }

    public override void Update()
    {
        base.Update();

        // поворачиваемся к цели
        var monsterTransform = _agent.transform;
        var position = monsterTransform.position;
        _agent.SetDestination(position);
        var direction = _monsterAI.targetPosition - position;
        monsterTransform.forward = new Vector3(direction.x, 0, direction.z);
        CoroutineStarter.Instance.StartRoutine(RoarRoutine());
    }

    private IEnumerator RoarRoutine()
    {
        _monsterAI.isRoarCooldown = true; // Задержка на рык
        _monsterAI.isFirstTime = false; // Не дать корутину запускаться бесконечно
        
        _monsterAI.animator.SetInteger("Status", 1); // Анимация крика

        MonsterSoundPlayer.Instance.RoarSound(_monsterAI.audioSource);
        
        yield return new WaitForSeconds(_monsterAI.monsterData.TimeBeforeChase);
        _monsterAI.isRoarCooldown = false;
        _monsterAI.isFirstTime = true;
    }
}
