using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Custom/Monster", order = 3)]
public class MonsterData : ScriptableObject
{
    [SerializeField] private float _speed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _flashCooldown;
    [SerializeField] private float _timeBeforeChase;
    [SerializeField] private float _stoppingDistance;
    [SerializeField] private float _walkRange;
    [SerializeField, Range(0f, 1f)] private float _chanceOfWalk;
    [SerializeField] private float _minTimeBeforeChoice;
    [SerializeField] private float _maxTimeBeforeChoice;
    
    public float Speed => _speed;
    public float WalkSpeed => _walkSpeed;
    public float FlashCooldown => _flashCooldown;
    public float TimeBeforeChase => _timeBeforeChase;
    public float StoppingDistance => _stoppingDistance;
    public float WalkRange => _walkRange;
    public float ChanceOfWalk => _chanceOfWalk;
    public float MinTimeBeforeChoice => _minTimeBeforeChoice;
    public float MaxTimeBeforeChoice => _maxTimeBeforeChoice;
}
