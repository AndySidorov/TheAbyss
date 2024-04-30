using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSoundPlayer : MonoBehaviour
{
    public static MonsterSoundPlayer Instance { get; private set; }
    [SerializeField] private MonsterSounds _monsterSounds;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public void IdleSound(AudioSource audioSource)
    {
        if (audioSource.clip == _monsterSounds.Idle) return;
        audioSource.clip = _monsterSounds.Idle;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void RoarSound(AudioSource audioSource)
    {
        if (audioSource.clip == _monsterSounds.Roar) return;
        audioSource.clip = _monsterSounds.Roar;
        audioSource.loop = false;
        audioSource.Play();
    }
    
    public void RunSound(AudioSource audioSource)
    {
        if (audioSource.clip == _monsterSounds.Run) return;
        audioSource.clip = _monsterSounds.Run;
        audioSource.loop = true;
        audioSource.Play();
    }
    
    public void FlashedSound(AudioSource audioSource)
    {
        if (audioSource.clip == _monsterSounds.Flashed) return;
        audioSource.clip = _monsterSounds.Flashed;
        audioSource.loop = false;
        audioSource.Play();
    }
    
    public void WalkSound(AudioSource audioSource)
    {
        if (audioSource.clip == _monsterSounds.Walk) return;
        audioSource.clip = _monsterSounds.Walk;
        audioSource.loop = true;
        audioSource.Play();
    }
}
