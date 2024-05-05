using UnityEngine;

[CreateAssetMenu (fileName = "Monster Sounds", menuName = "Custom/MonsterSounds", order = 4)]
public class MonsterSounds : ScriptableObject
{
    [SerializeField] private AudioClip _idle;
    [SerializeField] private AudioClip _roar;
    [SerializeField] private AudioClip _run;
    [SerializeField] private AudioClip _flashed;
    [SerializeField] private AudioClip _walk;
    
    public AudioClip Idle => _idle;
    public AudioClip Roar => _roar;
    public AudioClip Run => _run;
    public AudioClip Flashed => _flashed;
    public AudioClip Walk => _walk;
    
}
