using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    public event UnityAction MouseClickStop = delegate {  };
    
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


    public void OnSelect(InputAction.CallbackContext context)
    {
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
}
