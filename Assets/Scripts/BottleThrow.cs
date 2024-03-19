using System;
using System.Collections;
using UnityEngine;

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
        if (!_isBreaking)
        {
            var hits = Physics.OverlapSphere(transform.position, _bottleData.BreakingRadius);
            foreach (var hit in hits) 
            { 
                var hitGameObject = hit.gameObject; 
                if (hitGameObject.CompareTag("Monster"))
                { 
                    var monsterMovement = hitGameObject.GetComponentInParent<MonsterMovement>(); 
                    monsterMovement.isChasing = true; 
                    monsterMovement.playerPosition = transform.position; 
                }
            }
            StartCoroutine(BreakingRoutine());
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
