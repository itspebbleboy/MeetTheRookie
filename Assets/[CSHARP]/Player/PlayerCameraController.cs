using System.Collections.Generic;
using Darklight.Game.Camera;
using Darklight.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteAlways]
public class PlayerCameraController : CameraController
{
    PlayerController _playerController;

    [Header(">> Player Camera State Adjustments")]
    [SerializeField, Range(-1, 1)] private float defaultStateFOVOffset = 0f;
    [SerializeField, Range(-1, 1)] private float followTargetStateFOVOffset = -0.25f;
    [SerializeField, Range(-1, 1)] private float closeUpStateFOVOffset = -0.5f;

    public void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        SetFocusTarget(_playerController.transform);

        // Create States
        CameraState defaultState = new CameraState(CameraStateKey.DEFAULT, this, defaultStateFOVOffset);
        CameraState followTargetState = new CameraState(CameraStateKey.FOLLOW_TARGET, this, followTargetStateFOVOffset);
        CameraState closeUpState = new CameraState(CameraStateKey.CLOSE_UP, this, closeUpStateFOVOffset);

        // Create State Machine
        _stateMachine = new StateMachine(new Dictionary<CameraStateKey, FiniteState<CameraStateKey>>()
        {
            { CameraStateKey.DEFAULT, defaultState },
            { CameraStateKey.FOLLOW_TARGET, followTargetState },
            { CameraStateKey.CLOSE_UP, closeUpState }
        }, CameraStateKey.DEFAULT);

    }

    public void Start()
    {
        if (_playerController != null && _playerController.stateMachine != null)
            _playerController.stateMachine.OnStateChanged += (PlayerState state) => OnPlayerStateChange(state);
    }

    public void OnPlayerStateChange(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.NONE:
            case PlayerState.IDLE:
                _stateMachine.GoToState(CameraStateKey.DEFAULT);
                break;
            case PlayerState.INTERACTION:
                _stateMachine.GoToState(CameraStateKey.CLOSE_UP);
                break;
            case PlayerState.WALK:
                _stateMachine.GoToState(CameraStateKey.FOLLOW_TARGET);
                break;
        }
    }

    public void OnDestroy()
    {
        if (_playerController != null && _playerController.stateMachine != null)
            _playerController.stateMachine.OnStateChanged -= (PlayerState state) => OnPlayerStateChange(state);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerCameraController))]
public class CustomEditorForScript : Editor
{
    SerializedObject _serializedObject;
    PlayerCameraController _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (PlayerCameraController)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif