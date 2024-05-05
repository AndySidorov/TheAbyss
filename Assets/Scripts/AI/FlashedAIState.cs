using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlashedAIState : AIState
{
    private MonsterAI _monsterAI;
    
    private Coroutine _flashRoutine;
    
    public FlashedAIState(NavMeshAgent agent, MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        _flashRoutine = CoroutineStarter.Instance.StartCoroutine(FlashRoutine());
    }

    public override void OnExit()
    {
        base.OnExit();
        _monsterAI.isChasing = false;
    }

    public override void Update()
    {
        base.Update();
    }
    
    // Монстр ослеплен на определенное время
    private IEnumerator FlashRoutine()
    {
        _agent.SetDestination(_agent.transform.position);
        MonsterSoundPlayer.Instance.FlashedSound(_monsterAI.audioSource);
        _monsterAI.animator.SetInteger("Status", 4); //  Анимация ослепления
        yield return new WaitForSeconds(_monsterAI.monsterData.FlashCooldown);
        _monsterAI.isFlashed = false;
    }
}
