using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum InputState
{
    Gameplay = 0,
    ShootAction = 1,
}

public class MoveEventArgs : EventArgs
{
    public Vector2 m_Direction;
    
    public MoveEventArgs(Vector2 direction)
    {
        m_Direction = direction;
    }
}

public class ClickEventArgs : EventArgs
{
    public Vector2 m_Target;

    public ClickEventArgs(Vector2 target)
    {
        m_Target = target;
    }
}

[CreateAssetMenu]
public class InputReader : ScriptableObject, DefaultInput.IGameplayActions
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

    public event EventHandler<MoveEventArgs> PlayerMoveEvent; 
    public event EventHandler<ClickEventArgs> PlayerClickEvent;
    
    
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
        }
        _defaultInput.Gameplay.Enable();
    }

    /*public void EnableShootActions()
    {
        //Invoke event here that switches the cursor of the player to a target reticle
        Debug.Log("Enabled ShootAction");
        _defaultInput.Gameplay.Disable();
        _defaultInput.ShootAction.Enable();
        inputState = InputState.ShootAction;
    }*/

    public void EnableGameplay()
    {
        _defaultInput.Gameplay.Enable();
        inputState = InputState.Gameplay;
    }
    
    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MouseClickStart.Invoke();
            PlayerClickEvent?.Invoke(this, new ClickEventArgs(Mouse.current.position.ReadValue()));
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

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveDirection = Vector2.zero;

        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
            PlayerMoveEvent?.Invoke(this, new MoveEventArgs(moveDirection));
            //Debug.Log(moveDirection);
        }

    }
}
