using UnityEngine;

// Scriptable object со спрайтами для интерфейса
[CreateAssetMenu (fileName = "UI List", menuName = "Custom/UI", order = 2)]
public class UIList : ScriptableObject
{
    [SerializeField] private Sprite _aimSprite;
    [SerializeField] private Sprite _reloadSprite;

    public Sprite AimSprite => _aimSprite;
    public Sprite ReloadSprite => _reloadSprite;
}
