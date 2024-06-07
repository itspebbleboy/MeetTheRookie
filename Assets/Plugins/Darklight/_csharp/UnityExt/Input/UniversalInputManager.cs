using System;
using System.Collections.Generic;

using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Editor;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Darklight.UnityExt.Input
{
    /// <summary>
    /// A MonoBehaviour singleton class that manages the input device type and the input actions for the current device.
    /// </summary>
    public class UniversalInputManager : MonoBehaviourSingleton<UniversalInputManager>
    {
        private bool _moveStarted;

        // -------------- [[ STATIC INPUT TYPE ]] -------------- >>
        public enum InputType { NULL, KEYBOARD, TOUCH, GAMEPAD }
        public static InputType DeviceInputType
        {
            get => Instance._deviceInputType;
            private set => Instance._deviceInputType = value;
        }

        // -------------- [[ SERIALIZED FIELDS ]] -------------- >>
        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField, ShowOnly] private InputType _deviceInputType;
        [SerializeField, ShowOnly] private Vector2 _moveInput;
        [SerializeField, ShowOnly] private bool _primaryInteract;
        [SerializeField, ShowOnly] private bool _secondaryInteract;
        [SerializeField, ShowOnly] private bool _pauseInteract;

        // -------------- [[ INPUT ACTION MAPS ]] -------------- >>
        InputActionMap _activeActionMap;
        InputActionMap _keyboardActionMap;
        InputActionMap _gamepadActionMap;
        InputActionMap _touchActionMap;

        // -------------- [[ INPUT ACTIONS ]] -------------- >>
        InputAction _moveInputAction => _activeActionMap.FindAction("MoveInput");
        InputAction _primaryButtonAction => _activeActionMap.FindAction("PrimaryInteract");
        InputAction _secondaryButtonAction => _activeActionMap.FindAction("SecondaryInteract");
        InputAction _menuButtonAction => _activeActionMap.FindAction("MenuButton");
        InputAction _pause => _activeActionMap.FindAction("PauseInteract");

        // -------------- [[ INPUT EVENTS ]] -------------- >>
        public delegate void OnInput_Trigger();
        public delegate void OnInput_Vec2(Vector2 moveInput);

        /// <summary> Event for the move input from the active device. </summary>
        public static event OnInput_Vec2 OnMoveInput;
        public static event OnInput_Vec2 OnMoveInputStarted;
        public static event OnInput_Trigger OnMoveInputCanceled;

        /// <summary> Event for the primary interaction input from the active device. </summary>
        public static event OnInput_Trigger OnPrimaryInteract;
        public static event OnInput_Trigger OnPrimaryInteractCanceled;

        /// <summary> Event for the secondary interaction input from the active device. </summary>
        public static event OnInput_Trigger OnSecondaryInteract;
        public static event OnInput_Trigger OnSecondaryInteractCanceled;

        /// <summary> Event for the menu button input from the active device. </summary>
        public static event OnInput_Trigger OnMenuButton;

        /// <summary> Event for the Pause interaction input from the active device. </summary>
        public static event OnInput_Trigger OnPauseInteract;
        public static event OnInput_Trigger OnPauseInteractCanceled;

        public override void Initialize()
        {
            if (DetectAndEnableInputDevice())
            {
                Debug.Log($"{Prefix}Found Input: {DeviceInputType}");
            }
            _inputActionAsset.Enable();
        }

        public void Reset()
        {
            ResetInputEvents();
            _inputActionAsset.Disable();
        }

        public void OnDestroy()
        {
            ResetInputEvents();
        }

        #region ---- [[ DEVICE INPUT DETECTION ]] ---->>

        bool DetectAndEnableInputDevice()
        {
            DisableAllActionMaps();
            return EnableDeviceBasedActionMap();
        }

        void DisableAllActionMaps()
        {
            if (_keyboardActionMap != null) { _keyboardActionMap.Disable(); }
            if (_gamepadActionMap != null) { _gamepadActionMap.Disable(); }
            if (_touchActionMap != null) { _touchActionMap.Disable(); }
        }

        bool EnableDeviceBasedActionMap()
        {
            // Detect the device and enable the action map
            switch (InputSystem.devices[0])
            {
                case Keyboard:
                    _keyboardActionMap = _inputActionAsset.FindActionMap("DefaultKeyboard");
                    return EnableActionMap(_keyboardActionMap, InputType.KEYBOARD);
                case Gamepad:
                    _gamepadActionMap = _inputActionAsset.FindActionMap("DefaultGamepad");
                    return EnableActionMap(_gamepadActionMap, InputType.GAMEPAD);
                case Touchscreen:
                    _touchActionMap = _inputActionAsset.FindActionMap("DefaultTouch");
                    return EnableActionMap(_touchActionMap, InputType.TOUCH);
                default:
                    Debug.LogError($"{Prefix}Could not find Input Type");
                    return false;
            }
        }

        bool EnableActionMap(InputActionMap map, InputType type)
        {
            DisableAllActionMaps();
            if (map == null)
            {
                Debug.LogError($"{Prefix} Could not find Action Map for {type}");
                return false;
            }

            // Set the active action map
            _activeActionMap = map;
            _activeActionMap.Enable();

            // Set the device input type
            DeviceInputType = type;

            // Enable the actions
            _moveInputAction.Enable();
            _primaryButtonAction.Enable();
            _secondaryButtonAction.Enable();
            _pause.Enable();

            // << -- Set the input events -- >>
            _moveInputAction.started += HandleMoveStarted;
            _moveInputAction.performed += HandleMovePerformed;
            _moveInputAction.canceled += HandleMoveCanceled;

            _primaryButtonAction.performed += HandlePrimaryPerformed;
            _primaryButtonAction.canceled += HandlePrimaryCanceled;

            _secondaryButtonAction.performed += HandleSecondaryPerformed;
            _secondaryButtonAction.canceled += HandleSecondaryCanceled;

            _menuButtonAction.started += HandleMenuStarted;

            _pause.performed += HandlePausePerformed;
            _pause.canceled += HandlePauseCanceled;
            return true;
        }

        void ResetInputEvents()
        {
            Debug.Log($"{Prefix} Reset Input Events ");

            // Unsubscribe from all input actions
            if (_moveInputAction != null)
            {
                _moveInputAction.started -= HandleMoveStarted;
                _moveInputAction.performed -= HandleMovePerformed;
                _moveInputAction.canceled -= HandleMoveCanceled;
            }

            if (_primaryButtonAction != null)
            {
                _primaryButtonAction.performed -= HandlePrimaryPerformed;
                _primaryButtonAction.canceled -= HandlePrimaryCanceled;
            }

            if (_secondaryButtonAction != null)
            {
                _secondaryButtonAction.performed -= HandleSecondaryPerformed;
                _secondaryButtonAction.canceled -= HandleSecondaryCanceled;
            }

            if (_pause != null)
            {
                _pause.performed -= HandlePausePerformed;
                _pause.canceled -= HandlePauseCanceled;
            }

            DisableAllActionMaps();
        }
        #endregion

        private void HandleMoveStarted(InputAction.CallbackContext ctx)
        {
            if (!_moveStarted)
            {
                Vector2 input = ctx.ReadValue<Vector2>();
                OnMoveInputStarted?.Invoke(input);
                _moveStarted = true;
            }
        }

        private void HandleMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
            OnMoveInput?.Invoke(_moveInput);
        }

        private void HandleMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveStarted = false;
            _moveInput = Vector2.zero;
            OnMoveInputCanceled?.Invoke();
        }

        private void HandlePrimaryPerformed(InputAction.CallbackContext ctx)
        {
            _primaryInteract = true;
            OnPrimaryInteract?.Invoke();
        }

        private void HandlePrimaryCanceled(InputAction.CallbackContext ctx)
        {
            _primaryInteract = false;
            OnPrimaryInteractCanceled?.Invoke();
        }

        private void HandleSecondaryPerformed(InputAction.CallbackContext ctx)
        {
            _secondaryInteract = true;
            OnSecondaryInteract?.Invoke();
        }

        private void HandleSecondaryCanceled(InputAction.CallbackContext ctx)
        {
            _secondaryInteract = false;
            OnSecondaryInteractCanceled?.Invoke();
        }

        private void HandleMenuStarted(InputAction.CallbackContext ctx)
        {
            OnMenuButton?.Invoke();
        }

        private void HandlePausePerformed(InputAction.CallbackContext ctx)
        {
            _pauseInteract = true;
            OnPauseInteract?.Invoke();
        }

        private void HandlePauseCanceled(InputAction.CallbackContext ctx)
        {
            _pauseInteract = false;
            OnPauseInteractCanceled?.Invoke();
        }

    }
}