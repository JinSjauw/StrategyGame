//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.1
//     from Assets/DefaultInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @DefaultInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @DefaultInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DefaultInput"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""e417af02-a56b-4f4c-b18b-d9c3b806800a"",
            ""actions"": [
                {
                    ""name"": ""MouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""bfd3a0dc-b6ae-4734-b8e3-d8b225a453de"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Button"",
                    ""id"": ""c1cbd0e0-e4d2-460e-a959-75b7832e8a7a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CenterCamera"",
                    ""type"": ""Button"",
                    ""id"": ""8ac6d8be-0c9f-4f61-9cff-6ccc095eb4a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SelectionBox"",
                    ""type"": ""Button"",
                    ""id"": ""b574d9fa-1ffa-4d4f-a6fe-8ceedd98c40a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""Value"",
                    ""id"": ""a21ccf82-5db4-4613-a7f8-59a28b9526dd"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a7aa0231-49a6-4f7c-953e-b4f9143a9825"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86809108-2b4d-416e-8c18-8780fe240bd4"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""418cd8b3-d1bf-428e-a358-3b2e945691b2"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CenterCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""226f1abc-06d9-4091-86ed-e2ce0dd66278"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectionBox"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d814355f-f684-4a6b-80fc-c7857a8053c6"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ShootAction"",
            ""id"": ""3969002a-9a84-4bed-b93d-e90cdf3c1808"",
            ""actions"": [
                {
                    ""name"": ""AimMove"",
                    ""type"": ""Value"",
                    ""id"": ""ccb1ff13-e5ac-4704-ba39-0c9db063de1f"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""57997553-b7d7-4de7-8166-50146d56bd88"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0be6bfc5-d94b-43a2-a7fb-ce33e5c39950"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0143a177-3fb6-476c-a793-7f9879bcffa9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_MouseClick = m_Gameplay.FindAction("MouseClick", throwIfNotFound: true);
        m_Gameplay_MoveCamera = m_Gameplay.FindAction("MoveCamera", throwIfNotFound: true);
        m_Gameplay_CenterCamera = m_Gameplay.FindAction("CenterCamera", throwIfNotFound: true);
        m_Gameplay_SelectionBox = m_Gameplay.FindAction("SelectionBox", throwIfNotFound: true);
        m_Gameplay_MouseMove = m_Gameplay.FindAction("MouseMove", throwIfNotFound: true);
        // ShootAction
        m_ShootAction = asset.FindActionMap("ShootAction", throwIfNotFound: true);
        m_ShootAction_AimMove = m_ShootAction.FindAction("AimMove", throwIfNotFound: true);
        m_ShootAction_Shoot = m_ShootAction.FindAction("Shoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
    private readonly InputAction m_Gameplay_MouseClick;
    private readonly InputAction m_Gameplay_MoveCamera;
    private readonly InputAction m_Gameplay_CenterCamera;
    private readonly InputAction m_Gameplay_SelectionBox;
    private readonly InputAction m_Gameplay_MouseMove;
    public struct GameplayActions
    {
        private @DefaultInput m_Wrapper;
        public GameplayActions(@DefaultInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseClick => m_Wrapper.m_Gameplay_MouseClick;
        public InputAction @MoveCamera => m_Wrapper.m_Gameplay_MoveCamera;
        public InputAction @CenterCamera => m_Wrapper.m_Gameplay_CenterCamera;
        public InputAction @SelectionBox => m_Wrapper.m_Gameplay_SelectionBox;
        public InputAction @MouseMove => m_Wrapper.m_Gameplay_MouseMove;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
            @MouseClick.started += instance.OnMouseClick;
            @MouseClick.performed += instance.OnMouseClick;
            @MouseClick.canceled += instance.OnMouseClick;
            @MoveCamera.started += instance.OnMoveCamera;
            @MoveCamera.performed += instance.OnMoveCamera;
            @MoveCamera.canceled += instance.OnMoveCamera;
            @CenterCamera.started += instance.OnCenterCamera;
            @CenterCamera.performed += instance.OnCenterCamera;
            @CenterCamera.canceled += instance.OnCenterCamera;
            @SelectionBox.started += instance.OnSelectionBox;
            @SelectionBox.performed += instance.OnSelectionBox;
            @SelectionBox.canceled += instance.OnSelectionBox;
            @MouseMove.started += instance.OnMouseMove;
            @MouseMove.performed += instance.OnMouseMove;
            @MouseMove.canceled += instance.OnMouseMove;
        }

        private void UnregisterCallbacks(IGameplayActions instance)
        {
            @MouseClick.started -= instance.OnMouseClick;
            @MouseClick.performed -= instance.OnMouseClick;
            @MouseClick.canceled -= instance.OnMouseClick;
            @MoveCamera.started -= instance.OnMoveCamera;
            @MoveCamera.performed -= instance.OnMoveCamera;
            @MoveCamera.canceled -= instance.OnMoveCamera;
            @CenterCamera.started -= instance.OnCenterCamera;
            @CenterCamera.performed -= instance.OnCenterCamera;
            @CenterCamera.canceled -= instance.OnCenterCamera;
            @SelectionBox.started -= instance.OnSelectionBox;
            @SelectionBox.performed -= instance.OnSelectionBox;
            @SelectionBox.canceled -= instance.OnSelectionBox;
            @MouseMove.started -= instance.OnMouseMove;
            @MouseMove.performed -= instance.OnMouseMove;
            @MouseMove.canceled -= instance.OnMouseMove;
        }

        public void RemoveCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // ShootAction
    private readonly InputActionMap m_ShootAction;
    private List<IShootActionActions> m_ShootActionActionsCallbackInterfaces = new List<IShootActionActions>();
    private readonly InputAction m_ShootAction_AimMove;
    private readonly InputAction m_ShootAction_Shoot;
    public struct ShootActionActions
    {
        private @DefaultInput m_Wrapper;
        public ShootActionActions(@DefaultInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @AimMove => m_Wrapper.m_ShootAction_AimMove;
        public InputAction @Shoot => m_Wrapper.m_ShootAction_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_ShootAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ShootActionActions set) { return set.Get(); }
        public void AddCallbacks(IShootActionActions instance)
        {
            if (instance == null || m_Wrapper.m_ShootActionActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ShootActionActionsCallbackInterfaces.Add(instance);
            @AimMove.started += instance.OnAimMove;
            @AimMove.performed += instance.OnAimMove;
            @AimMove.canceled += instance.OnAimMove;
            @Shoot.started += instance.OnShoot;
            @Shoot.performed += instance.OnShoot;
            @Shoot.canceled += instance.OnShoot;
        }

        private void UnregisterCallbacks(IShootActionActions instance)
        {
            @AimMove.started -= instance.OnAimMove;
            @AimMove.performed -= instance.OnAimMove;
            @AimMove.canceled -= instance.OnAimMove;
            @Shoot.started -= instance.OnShoot;
            @Shoot.performed -= instance.OnShoot;
            @Shoot.canceled -= instance.OnShoot;
        }

        public void RemoveCallbacks(IShootActionActions instance)
        {
            if (m_Wrapper.m_ShootActionActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IShootActionActions instance)
        {
            foreach (var item in m_Wrapper.m_ShootActionActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ShootActionActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ShootActionActions @ShootAction => new ShootActionActions(this);
    public interface IGameplayActions
    {
        void OnMouseClick(InputAction.CallbackContext context);
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnCenterCamera(InputAction.CallbackContext context);
        void OnSelectionBox(InputAction.CallbackContext context);
        void OnMouseMove(InputAction.CallbackContext context);
    }
    public interface IShootActionActions
    {
        void OnAimMove(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}