using UnityEngine.AI;
using UnityEngine;

public class KillAIState : AIState
{
    private MonsterAI _monsterAI;
    public KillAIState(NavMeshAgent agent, MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        
        _agent.SetDestination(_agent.transform.position);

        var transform = _monsterAI.transform;
        var direction = _monsterAI.targetPosition - transform.position;
        transform.forward = new Vector3(direction.x, 0, direction.z); // Повернуть монстра к нам
        
        MonsterSoundPlayer.Instance.RoarSound(_monsterAI.audioSource);
        _monsterAI.animator.SetInteger("Status", 3); // Анимация убийства
    }
}
