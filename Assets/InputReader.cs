using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum InputState
{
    Gameplay = 0,
    ShootAction = 1,
}

[CreateAssetMenu]
public class InputReader : ScriptableObject, DefaultInput.IGameplayActions, DefaultInput.IShootActionActions
{

    //Gameplay Events
    public event UnityAction CenterCameraEvent = delegate {  };
    public event UnityAction MoveCameraStartEvent = delegate {  };
    public event UnityAction MoveCameraStopEvent = delegate {  };
    public event UnityAction MouseMoveStartEvent = delegate {  };
    public event UnityAction MouseMoveStopEvent = delegate {  };
    public event UnityAction BoxSelectionStartEvent = delegate {  };
    public event UnityAction BoxSelectionStopEvent = delegate {  };
    public event UnityAction MouseClickStart = delegate {  };
    public event UnityAction MouseClickStop = delegate {  };
    
    
    //Shoot events
    public event UnityAction ShootStart = delegate {  };
    public event UnityAction ShootEnd = delegate {  };
    public event UnityAction AimMove = delegate {  };

    public InputState inputState
    {
        get; private set;
    }
        
    private DefaultInput _defaultInput;

    private void OnEnable()
    {
        if (_defaultInput == null)
        {
            _defaultInput = new DefaultInput();
            _defaultInput.Gameplay.SetCallbacks(this);
            _defaultInput.ShootAction.SetCallbacks(this);
        }
        _defaultInput.Gameplay.Enable();
    }

    public void EnableShootActions()
    {
        //Invoke event here that switches the cursor of the player to a target reticle
        Debug.Log("Enabled ShootAction");
        _defaultInput.Gameplay.Disable();
        _defaultInput.ShootAction.Enable();
        inputState = InputState.ShootAction;
    }

    public void EnableGameplay()
    {
        _defaultInput.ShootAction.Disable();
        _defaultInput.Gameplay.Enable();
        inputState = InputState.Gameplay;
    }
    
    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MouseClickStart.Invoke();
        }
        
        if (context.canceled)
        {
            MouseClickStop.Invoke();
        }
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MoveCameraStartEvent.Invoke();
        }

        if (context.canceled)
        {
            MoveCameraStopEvent.Invoke();
        }
    }

    public void OnCenterCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CenterCameraEvent.Invoke();
        }
    }

    public void OnSelectionBox(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BoxSelectionStartEvent.Invoke();
        }

        if (context.canceled)
        {
            BoxSelectionStopEvent.Invoke();
        }
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            MouseMoveStartEvent.Invoke();
        }
        
        if (context.canceled)
        {
            MouseMoveStopEvent.Invoke();
        }
    }
    
    //Shoot Input;
    public void OnAimMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AimMove.Invoke();
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("ShootStart");
            ShootStart.Invoke();
        }

        if (context.canceled)
        {
            ShootEnd.Invoke();
        }
    }
}
