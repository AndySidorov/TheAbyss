using UnityEngine;

// Scriptable object со звуками и переменными для бутылки
[CreateAssetMenu(fileName = "Bottle Break Data", menuName = "Custom/Bottle", order = 5)]
public class BottleData : ScriptableObject
{
    [SerializeField] private float _breakingRadius;
    [SerializeField] private AudioClip _throwingSound;
    [SerializeField] private AudioClip _breakingSound;

    public float BreakingRadius => _breakingRadius;

    public AudioClip ThrowingSound => _throwingSound;

    public AudioClip BreakingSound => _breakingSound;
}
