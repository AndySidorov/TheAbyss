using UnityEngine;

[CreateAssetMenu (fileName = "PlayerSounds", menuName = "Custom/PlayerSounds", order = 1)]
public class PlayerSounds : ScriptableObject
{
    [SerializeField] private AudioClip _idle;
    [SerializeField] private AudioClip _walk;
    [SerializeField] private AudioClip _sneak;
    [SerializeField] private AudioClip _run;
    [SerializeField] private AudioClip _jump; // Звук начала прыжка
    [SerializeField] private AudioClip _jumped; // Звук приземления
    [SerializeField] private AudioClip _collect; // Звук сбора ресурсов
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
