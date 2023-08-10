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

public class MouseEventArgs : EventArgs
{
    public Vector2 MousePosition;

    public MouseEventArgs(Vector2 target)
    {
        MousePosition = target;
    }
}

    [CreateAssetMenu]
    public class InputReader : ScriptableObject, DefaultInput.IGameplayActions, DefaultInput.IInventoryActions, DefaultInput.IMainMenuActions
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
        public event UnityAction OpenInventory = delegate {  };
        public event EventHandler<MoveEventArgs> PlayerMoveEvent; 
        public event EventHandler<MouseEventArgs> PlayerClickEvent;
        public event EventHandler<int> PlayerScrollEvent;
        public event UnityAction CameraFollowToggle = delegate {  };
        public event UnityAction ThrowAimStart = delegate {  };
        public event UnityAction ThrowAimStop = delegate {  };

        public event EventHandler<GridPosition> PocketSelectionChanged; 

        #endregion
        //Gameplay Events

        #region Inventory Events

        public event EventHandler<MouseEventArgs> InventoryClickStartEvent;
        public event EventHandler<MouseEventArgs> InventoryMouseMoveEvent;
        public event EventHandler<MouseEventArgs> InventoryClickEndEvent; 
        public event EventHandler<MouseEventArgs> ItemClickedEvent;
        public event UnityAction CloseInventory = delegate {  };
        public event UnityAction RotateItem = delegate {  };
        public event UnityAction SpawnItem = delegate {  };

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
                _defaultInput.MainMenu.SetCallbacks(this);
            }
            _defaultInput.Gameplay.Enable();
        }

        public void EnableGameplayInput()
        {
            _defaultInput.Gameplay.Enable();
            _defaultInput.Inventory.Disable();
            _defaultInput.MainMenu.Disable();
        }

        public void EnableInventoryInput()
        {
            Debug.Log("Enabled Inventory Input");
            _defaultInput.Gameplay.Disable();
            _defaultInput.Inventory.Enable();
            _defaultInput.MainMenu.Disable();
        }

        public void EnableMainMenuInput()
        {
            _defaultInput.Gameplay.Disable();
            _defaultInput.Inventory.Disable();
            _defaultInput.MainMenu.Enable();
        }
        
        public void DisableMainMenuInput()
        {
            _defaultInput.MainMenu.Disable();
        }
        
        #region Inventory Inputs / Main Menu
        
        public void OnClickInventory(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                InventoryClickStartEvent?.Invoke(this, new MouseEventArgs(Mouse.current.position.ReadValue()));
            }

            if (context.canceled)
            {
                InventoryClickEndEvent?.Invoke(this, new MouseEventArgs(Mouse.current.position.ReadValue()));
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

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RotateItem.Invoke();
            }
        }

        public void OnSpawnItem(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SpawnItem.Invoke();
            }
        }

        public void OnInventoryMouseMove(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                InventoryMouseMoveEvent?.Invoke(this, new MouseEventArgs(Mouse.current.position.ReadValue()));
            }
        }

        public void OnClickItem(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ItemClickedEvent?.Invoke(this, new MouseEventArgs(Mouse.current.position.ReadValue()));
            }
        }

        #endregion
        
        #region Gameplay Inputs
        
        public void OnMouseClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MouseClickStart.Invoke();
                PlayerClickEvent?.Invoke(this, new MouseEventArgs(Mouse.current.position.ReadValue()));
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
                OpenInventory.Invoke();
                EnableInventoryInput();
            }
        }

        public void OnSwitchWeapon(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                float scroll = context.ReadValue<float>();
                if (scroll > 0)
                {
                    PlayerScrollEvent?.Invoke(this, 0);
                }
                else
                {
                    PlayerScrollEvent?.Invoke(this, 1);
                }
            }
        }

        public void OnCameraFollowToggle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CameraFollowToggle.Invoke();
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ThrowAimStart.Invoke();
            }

            if (context.canceled)
            {
                ThrowAimStop.Invoke();
            }
        }

        public void OnSelectOne(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PocketSelectionChanged?.Invoke(this, new GridPosition(0, 1));
            }
        }

        public void OnSelectTwo(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PocketSelectionChanged?.Invoke(this, new GridPosition(0, 2));
            }
        }

        public void OnSelectThree(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PocketSelectionChanged?.Invoke(this, new GridPosition(0, 3));
            }
        }

        public void OnSelectFour(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PocketSelectionChanged?.Invoke(this, new GridPosition(0, 4));
            }
        }

        public void OnSelectFive(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                PocketSelectionChanged?.Invoke(this, new GridPosition(0, 5));
            }
        }

        #endregion
    }
}

