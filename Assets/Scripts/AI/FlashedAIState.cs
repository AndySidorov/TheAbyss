using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FlashedAIState : AIState
{
    private MonsterAI _monsterAI;
    public FlashedAIState(NavMeshAgent agent, MonsterAI monsterAI) : base(agent)
    {
        _monsterAI = monsterAI;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered Flashed State");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exited Flashed State");
    }

    public override void Update()
    {
        base.Update();
        // Flashed function
        CoroutineStarter.Instance.StartRoutine(FlashRoutine());
    }
    
    // Монстр ослеплен на определенное время
    private IEnumerator FlashRoutine()
    {
        _agent.SetDestination(_agent.transform.position);
        
        _monsterAI.isFlashCooldown = true; // Ослепление, прерывающее все действия (не запускать корутин ослепления бесконечно)
        _monsterAI.isRoarCooldown = false; // Прервать крик, если монстр ослеплен во время рыка
        _monsterAI.isChasing = false; // Сбросить режим преследования
        
        MonsterSoundPlayer.Instance.FlashedSound(_monsterAI.audioSource);
        
        _monsterAI.animator.SetInteger("Status", 4); //  Анимация ослепления
        
        yield return new WaitForSeconds(_monsterAI.monsterData.FlashCooldown);
        
        _monsterAI.isFlashCooldown = false; // Ослепление прошло
        _monsterAI.isFirstTime = true; // Монстр снова зарычит, если нас заметит
        _monsterAI.isFlashed = false; // Корутин снова можно будет запустить
    }
}
