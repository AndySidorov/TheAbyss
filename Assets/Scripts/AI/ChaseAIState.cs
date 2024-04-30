using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseAIState : AIState
{
    private MonsterAI _monsterAI;
    public ChaseAIState(NavMeshAgent agent,  MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered Chase State");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exited Chase State");
    }

    public override void Update()
    {
        base.Update();
        // мб корутин chase сюда добалю
        
        _monsterAI.animator.SetInteger("Status", 2); // Анимация бега
        
        MonsterSoundPlayer.Instance.RunSound(_monsterAI.audioSource);
        
        var monsterPosition = _agent.transform.position;
        var monsterXZ = new Vector2(monsterPosition.x, monsterPosition.z); 
        var targetXZ = new Vector2(_monsterAI.targetPosition.x, _monsterAI.targetPosition.z);
        // Y не используется, потому что бутылка может удариться об потолок и не сработает код сброса цели
                    
        if (Vector2.Distance(monsterXZ, targetXZ) < _agent.stoppingDistance) 
        {
            _agent.SetDestination(monsterPosition);
                
             // Снова зарычит, если увидит
            _monsterAI.isChasing = false; 
            
        }
        else // Если монстр только закончил рычать или еще не достиг цель (последнюю переданную)
        {
            _agent.SetDestination(_monsterAI.targetPosition); // Задать цель (playerPosition меняется скриптом на игроке, передается вместе с isChasing)
        }
    }
    
}
