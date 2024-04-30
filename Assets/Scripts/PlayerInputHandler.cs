using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private MainControls _controls;

    public event Action<Vector2> onMove;
    public event Action<Vector2> onLook;
    
    public event Action onRunPressed;
    public event Action onRunReleased;
    public event Action onSneakPressed;
    public event Action onSneakReleased;
    
    public event Action onJump;
    
    public event Action onThrowPressed;
    public event Action onThrowReleased;
    
    public event Action onThrowStop;
    
    public event Action onFlash;
    
    public event Action onTake;
    
    public event Action onDrink;
    
    public event Action onPause;

    private void Awake()
    {
        _controls = new MainControls();
        _controls.Enable();
    }

    private void OnEnable()
    {
        _controls.Main.Move.performed += OnMove;
        _controls.Main.Move.canceled += OnMove;
        _controls.Main.Look.performed += OnLook;
        _controls.Main.Look.canceled += OnLook;
        
        _controls.Main.Run.performed += OnRunPressed;
        _controls.Main.Run.canceled += OnRunReleased;
        _controls.Main.Sneak.performed += OnSneakPressed;
        _controls.Main.Sneak.canceled += OnSneakReleased;
        
        _controls.Main.Jump.performed += OnJump;
        
        _controls.Main.Throw.performed += OnThrowPressed;
        _controls.Main.Throw.canceled += OnThrowReleased;
        
        _controls.Main.StopThrowing.performed += OnThrowStop;
        
        _controls.Main.Flash.performed += OnFlash;
        
        _controls.Main.Take.performed += OnTake;
        
        _controls.Main.Drink.performed += OnDrink;
        
        _controls.Main.Pause.performed += OnPause;
    }
    
    private void OnDisable()
    {
        _controls.Main.Move.performed -= OnMove;
        _controls.Main.Move.canceled -= OnMove;
        _controls.Main.Look.performed -= OnLook;
        _controls.Main.Look.canceled -= OnLook;
        
        _controls.Main.Run.performed -= OnRunPressed;
        _controls.Main.Run.canceled -= OnRunReleased;
        _controls.Main.Sneak.performed -= OnSneakPressed;
        _controls.Main.Sneak.canceled -= OnSneakReleased;
        
        _controls.Main.Jump.performed -= OnJump;
        
        _controls.Main.Throw.performed -= OnThrowPressed;
        _controls.Main.Throw.canceled -= OnThrowReleased;
        
        _controls.Main.StopThrowing.performed -= OnThrowStop;
        
        _controls.Main.Flash.performed -= OnFlash;
        
        _controls.Main.Take.performed -= OnTake;
        
        _controls.Main.Drink.performed -= OnDrink;
        
        _controls.Main.Pause.performed -= OnPause;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }
    
    private void OnLook(InputAction.CallbackContext context)
    {
        onLook?.Invoke(context.ReadValue<Vector2>());
    }
    
    private void OnRunPressed(InputAction.CallbackContext context)
    {
        onRunPressed?.Invoke();
    }
    
    private void OnRunReleased(InputAction.CallbackContext context)
    {
        onRunReleased?.Invoke();
    }
    
    private void OnSneakPressed(InputAction.CallbackContext context)
    {
        onSneakPressed?.Invoke();
    }
    
    private void OnSneakReleased(InputAction.CallbackContext context)
    {
        onSneakReleased?.Invoke();
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        onJump?.Invoke();
    }
    
    private void OnThrowPressed(InputAction.CallbackContext context)
    {
        onThrowPressed?.Invoke();
    }
    
    private void OnThrowReleased(InputAction.CallbackContext context)
    {
        onThrowReleased?.Invoke();
    }
    
    private void OnThrowStop(InputAction.CallbackContext context)
    {
        onThrowStop?.Invoke();
    }
    
    private void OnFlash(InputAction.CallbackContext context)
    {
        onFlash?.Invoke();
    }
    
    private void OnTake(InputAction.CallbackContext context)
    {
        onTake?.Invoke();
    }
    
    private void OnDrink(InputAction.CallbackContext context)
    {
        onDrink?.Invoke();
    }
    
    private void OnPause(InputAction.CallbackContext context)
    {
        onPause?.Invoke();
    }
}