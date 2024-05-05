using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BottleThrow : MonoBehaviour
{
    [SerializeField] private BottleData _bottleData;
    
    private AudioSource _audio;
    private bool _isBreaking;
    
    private void Awake()
    {
        _audio = GetComponentInChildren<AudioSource>();
        _audio.clip = _bottleData.ThrowingSound;
        _audio.loop = false;
        _audio.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isBreaking) // Если не сталкивался до этого
        {
            NavMeshHit meshHit;
            if (NavMesh.SamplePosition(transform.position, out meshHit, 4f, NavMesh.AllAreas))
            {
                var hits = Physics.OverlapSphere(transform.position, _bottleData.BreakingRadius);
                foreach (var hit in hits)
                { 
                    var hitGameObject = hit.gameObject; 
                    if (hitGameObject.CompareTag("Monster"))
                    { 
                        var monsterMovement = hitGameObject.GetComponentInParent<MonsterAI>(); 
                        monsterMovement.isChasing = true;
                        monsterMovement.targetPosition = meshHit.position;
                    }
                }
                StartCoroutine(BreakingRoutine());
            }
        }
    }
    
    private IEnumerator BreakingRoutine()
    {
        _audio.clip = _bottleData.BreakingSound;
        _audio.Play();
        
        _isBreaking = true;
        
        yield return new WaitForSeconds(2f);
        
        Destroy(gameObject);
    }
}
