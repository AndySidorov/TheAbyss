using UnityEngine;

[CreateAssetMenu(fileName = "Monster Data", menuName = "Custom/Monster", order = 3)]
public class MonsterData : ScriptableObject
{
    [SerializeField] private float _speed;
    [SerializeField] private float _flashCooldown;
    [SerializeField] private float _timeBeforeChase;
    
    public float Speed => _speed;
    public float FlashCooldown => _flashCooldown;
    public float TimeBeforeChase => _timeBeforeChase;
}
