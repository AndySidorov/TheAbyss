using UnityEngine;

// Scriptable object со всеми переменными для игрока
[CreateAssetMenu(fileName = "Default Player Data", menuName = "Custom/Player", order = 0)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sneakSpeed;
    [SerializeField] private float _runSpeed;
    
    [SerializeField] private float _walkJumpHeight; // Для Idle и Walk
    [SerializeField] private float _sneakJumpHeight;
    [SerializeField] private float _runJumpHeight;
    
    [SerializeField] private float _idleHearingZoneRadius;
    [SerializeField] private float _walkHearingZoneRadius;
    [SerializeField] private float _sneakHearingZoneRadius;
    [SerializeField] private float _runHearingZoneRadius;
    [SerializeField] private float _landingHearingZoneCoefficient; // Как сильно увеличивается зона слышимости при приземлении
    
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _staminaTimeToRegen;
    [SerializeField] private float _staminaDecreasePerFrame;
    [SerializeField] private float _staminaIncreasePerFrame;
    [SerializeField] private float _runJumpStaminaDecrease; // Количество стамины удаляемое при прыжке во время бега
    
    [SerializeField] private float _maxPower; // Максимальная сила броска
    [SerializeField] private float _powerIncreasePerFrame; // Сколько силы броска прибавляется в секунду
    
    [SerializeField] private float _flashDistance;
    [SerializeField] private float _flashCooldown; // Время перезарядки
    
    [SerializeField] private int _numberOfFlashes;
    [SerializeField] private int _numberOfEnergyDrinks;
    [SerializeField] private int _numberOfBottles;
    [SerializeField] private float _mouseSensitivity;
    
    [SerializeField] private int _numberOfFlashesToAdd;
    [SerializeField] private int _numberOfEnergyDrinksToAdd;
    [SerializeField] private int _numberOfBottlesToAdd;
    [SerializeField] private float _flashDistanceToAdd;
    [SerializeField] private float _flashCooldownToDecrease;
    
    [SerializeField] private float _defaultLightIntensity; // Яркость обычного фонарика
    [SerializeField] private float _flashLightIntensity; // Яркость вспышки
    
    [SerializeField] private float _waitForDeath; // Сколкьо времени ждать после убийства
    
    public float WalkSpeed => _walkSpeed;
    public float SneakSpeed => _sneakSpeed;
    public float RunSpeed => _runSpeed;
    
    public float WalkJumpHeight => _walkJumpHeight;
    public float SneakJumpHeight => _sneakJumpHeight;
    public float RunJumpHeight => _runJumpHeight;
    
    public float IdleHearingZoneRadius => _idleHearingZoneRadius;
    public float WalkHearingZoneRadius => _walkHearingZoneRadius;
    public float SneakHearingZoneRadius => _sneakHearingZoneRadius;
    public float RunHearingZoneRadius => _runHearingZoneRadius;
    public float LandingHearingZoneCoefficient => _landingHearingZoneCoefficient;
    
    public float MaxStamina => _maxStamina;
    public float StaminaTimeToRegen => _staminaTimeToRegen;
    public float StaminaDecreasePerFrame => _staminaDecreasePerFrame;
    public float StaminaIncreasePerFrame => _staminaIncreasePerFrame;
    public float RunJumpStaminaDecrease => _runJumpStaminaDecrease;
    
    public float MaxPower => _maxPower;
    public float PowerIncreasePerFrame => _powerIncreasePerFrame;
    
    public float FlashDistance => _flashDistance;
    public float FlashCooldown => _flashCooldown;
    
    public int NumberOfFlashes => _numberOfFlashes;
    public int NumberOfEnergyDrinks => _numberOfEnergyDrinks;
    public int NumberOfBottles => _numberOfBottles;
    public float MouseSensitivity => _mouseSensitivity;
    
    public int NumberOfFlashesToAdd => _numberOfFlashesToAdd;
    public int NumberOfEnergyDrinksToAdd => _numberOfEnergyDrinksToAdd;
    public int NumberOfBottlesToAdd => _numberOfBottlesToAdd;
    public float FlashDistanceToAdd => _flashDistanceToAdd;
    public float FlashCooldownToDecrease => _flashCooldownToDecrease;
    
    public float DefaultLightIntensity => _defaultLightIntensity;
    public float FlashLightIntensity => _flashLightIntensity;
    
    public float WaitForDeath => _waitForDeath;
}
