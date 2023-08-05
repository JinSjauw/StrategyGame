using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CustomInput
{
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
    public class InputReader : ScriptableObject, DefaultInput.IGameplayActions, DefaultInput.IInventoryActions
    {

        #region Gameplay Events
        
        public event UnityAction CenterCameraEvent = delegate {  };
        public event UnityAction MoveCameraStartEvent = delegate {  };
        public event UnityAction MoveCameraStopEvent = delegate {  };
        public event UnityAction MouseMoveStartEvent = delegate {  };
        public event UnityAction MouseMoveStopEvent = delegate {  };
        public event UnityAction BoxSelectionStartEvent = delegate {  };
        public event UnityAction BoxSelectionStopEvent = delegate {  };
        public event UnityAction MouseClickStart = delegate {  };
        public event UnityAction MouseClickStop = delegate {  };
        public event UnityAction ReloadStart = delegate { };
        public event UnityAction AimStart = delegate {  };
        public event UnityAction AimStop = delegate {  };
        public event EventHandler<MoveEventArgs> PlayerMoveEvent; 
        public event EventHandler<ClickEventArgs> PlayerClickEvent;
        
        #endregion
        //Gameplay Events

        #region Inventory Events

        public event EventHandler<ClickEventArgs> InventoryClickStartEvent;
        public event EventHandler<ClickEventArgs> InventoryClickEndEvent; 
        public event UnityAction CloseInventory = delegate {  };
        
        #endregion
        
        

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
                _defaultInput.Inventory.SetCallbacks(this);
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

        public void EnableGameplayInput()
        {
            _defaultInput.Gameplay.Enable();
            _defaultInput.Inventory.Disable();
            inputState = InputState.Gameplay;
        }
        
        private void EnableInventoryInput()
        {
            Debug.Log("Enabled Inventory Input");
            _defaultInput.Gameplay.Disable();
            _defaultInput.Inventory.Enable();
        }

        #region Inventory Inputs
        
        public void OnClickInventory(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                InventoryClickStartEvent?.Invoke(this, new ClickEventArgs(Mouse.current.position.ReadValue()));
            }

            if (context.canceled)
            {
                InventoryClickEndEvent?.Invoke(this, new ClickEventArgs(Mouse.current.position.ReadValue()));
            }
        }

        public void OnCloseInventory(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EnableGameplayInput();
                CloseInventory?.Invoke();
            }
        }
        #endregion
        
        #region Gameplay Inputs
        
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

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ReloadStart.Invoke();
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                AimStart.Invoke();
            }

            if (context.canceled)
            {
                AimStop.Invoke();
            }
        }

        public void OnOpenInventory(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EnableInventoryInput();
            }
        }

        #endregion
    }
}

