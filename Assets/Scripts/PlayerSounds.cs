using UnityEngine;

[CreateAssetMenu (fileName = "PlayerSounds", menuName = "Custom/PlayerSounds", order = 1)]
public class PlayerSounds : ScriptableObject
{
    [SerializeField] private AudioClip _idle;
    [SerializeField] private AudioClip _walk;
    [SerializeField] private AudioClip _sneak;
    [SerializeField] private AudioClip _run;
    [SerializeField] private AudioClip _jump;
    [SerializeField] private AudioClip _jumped;
    [SerializeField] private AudioClip _collect;
    [SerializeField] private AudioClip _drink;
    [SerializeField] private AudioClip _flash;
    
    public AudioClip Idle => _idle;
    public AudioClip Walk => _walk;
    public AudioClip Sneak => _sneak;
    public AudioClip Run => _run;
    public AudioClip Jump => _jump;
    public AudioClip Jumped => _jumped;
    public AudioClip Collect => _collect;
    public AudioClip Drink => _drink;
    public AudioClip Flash => _flash;
}
