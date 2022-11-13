//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Controls/CameraControls.inputactions
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

public partial class @CameraControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CameraControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CameraControls"",
    ""maps"": [
        {
            ""name"": ""DefaultCameraControls"",
            ""id"": ""6c15a227-4917-4a6c-b9c3-fa525dbea50a"",
            ""actions"": [
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1c4af8b9-292a-46d1-8c30-d50ae842c17f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""4554e0bc-c354-481e-a65e-2f76f8252c41"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""081894c7-0898-4da9-801c-2a1637e2131a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PCSontrolScheme"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c1e3f6e1-7b57-428f-b44e-65efa385650d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PCSontrolScheme"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a1993e9e-abce-42c6-b48d-66233f29b854"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PCSontrolScheme"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3b94f5b0-bf2a-4b2a-a3f9-ecd22431401f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PCSontrolScheme"",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PCSontrolScheme"",
            ""bindingGroup"": ""PCSontrolScheme"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // DefaultCameraControls
        m_DefaultCameraControls = asset.FindActionMap("DefaultCameraControls", throwIfNotFound: true);
        m_DefaultCameraControls_MoveCamera = m_DefaultCameraControls.FindAction("MoveCamera", throwIfNotFound: true);
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

    // DefaultCameraControls
    private readonly InputActionMap m_DefaultCameraControls;
    private IDefaultCameraControlsActions m_DefaultCameraControlsActionsCallbackInterface;
    private readonly InputAction m_DefaultCameraControls_MoveCamera;
    public struct DefaultCameraControlsActions
    {
        private @CameraControls m_Wrapper;
        public DefaultCameraControlsActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveCamera => m_Wrapper.m_DefaultCameraControls_MoveCamera;
        public InputActionMap Get() { return m_Wrapper.m_DefaultCameraControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultCameraControlsActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultCameraControlsActions instance)
        {
            if (m_Wrapper.m_DefaultCameraControlsActionsCallbackInterface != null)
            {
                @MoveCamera.started -= m_Wrapper.m_DefaultCameraControlsActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.performed -= m_Wrapper.m_DefaultCameraControlsActionsCallbackInterface.OnMoveCamera;
                @MoveCamera.canceled -= m_Wrapper.m_DefaultCameraControlsActionsCallbackInterface.OnMoveCamera;
            }
            m_Wrapper.m_DefaultCameraControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveCamera.started += instance.OnMoveCamera;
                @MoveCamera.performed += instance.OnMoveCamera;
                @MoveCamera.canceled += instance.OnMoveCamera;
            }
        }
    }
    public DefaultCameraControlsActions @DefaultCameraControls => new DefaultCameraControlsActions(this);
    private int m_PCSontrolSchemeSchemeIndex = -1;
    public InputControlScheme PCSontrolSchemeScheme
    {
        get
        {
            if (m_PCSontrolSchemeSchemeIndex == -1) m_PCSontrolSchemeSchemeIndex = asset.FindControlSchemeIndex("PCSontrolScheme");
            return asset.controlSchemes[m_PCSontrolSchemeSchemeIndex];
        }
    }
    public interface IDefaultCameraControlsActions
    {
        void OnMoveCamera(InputAction.CallbackContext context);
    }
}
