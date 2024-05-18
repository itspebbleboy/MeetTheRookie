using System;
using System.Collections.Generic;
using Darklight.UnityExt.Input;

using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Utility;

#if UNITY_EDITOR
#endif


public enum PlayerState { NONE, IDLE, WALK, INTERACTION, HIDE }

/// <summary>
/// This class is responsible for translating player input into movement and interaction.
/// </summary>

public class PlayerController : MonoBehaviour
{
    #region  [[ STATE MACHINE ]] ======================================================== >>
    public class StateMachine : FiniteStateMachine<PlayerState>
    {
        private PlayerController _controller;
        private PlayerAnimator _animator => _controller.animator;

        /// <param name="args">
        ///    args[0] = PlayerController ( playerController )
        /// </param>
        public StateMachine(Dictionary<PlayerState, FiniteState<PlayerState>> possibleStates, PlayerState initialState, params object[] args) : base(possibleStates, initialState, args)
        {
            _controller = (PlayerController)args[0];
        }

        public override void Step()
        {
            base.Step();
        }

        public override bool GoToState(PlayerState stateType)
        {
            bool result = base.GoToState(stateType);
            if (result)
            {
                _controller._currentState = stateType;
                _animator.PlayStateAnimation(stateType);
            }

            return result;
        }
    }

    public class State : FiniteState<PlayerState>
    {
        /// <param name="args">
        ///   args[0] = PlayerController ( playerController )
        public State(PlayerState stateType, params object[] args) : base(stateType, args) { }

        public override void Enter()
        {
            // Debug.Log($"Entering State: {stateType}");
        }

        public override void Exit()
        {
            // Debug.Log($"Exiting State: {stateType}");
        }

        public override void Execute()
        {
            // Debug.Log($"Executing State: {stateType}");
        }
    }

    #endregion

    public PlayerInteractor interactor => GetComponentInChildren<PlayerInteractor>();
    public PlayerAnimator animator => GetComponentInChildren<PlayerAnimator>();
    public PlayerCameraController cameraController => FindFirstObjectByType<PlayerCameraController>();
    public StateMachine stateMachine { get; private set; }

    [SerializeField, ShowOnly] PlayerState _currentState = PlayerState.NONE;
    [SerializeField, ShowOnly] Vector2 _activeMoveInput = Vector2.zero;


    [Header("Settings")]
    [Range(0.1f, 5f)] public float playerSpeed = 2.5f;
    public Vector2 moveVector = Vector2.zero; // this is the vector that the player is moving on
    private SceneBounds sceneBounds;

    void Awake()
    {
        stateMachine = new StateMachine(new Dictionary<PlayerState, FiniteState<PlayerState>> {
            {PlayerState.NONE, new State(PlayerState.NONE)},
            {PlayerState.IDLE, new State(PlayerState.IDLE)},
            {PlayerState.WALK, new State(PlayerState.WALK)},
            {PlayerState.INTERACTION, new State(PlayerState.INTERACTION)},
            {PlayerState.HIDE, new State(PlayerState.HIDE)}
        }, PlayerState.IDLE, this);
    }

    void Start()
    {
        UniversalInputManager.OnMoveInput += (Vector2 input) => _activeMoveInput = input;
        UniversalInputManager.OnMoveInputCanceled += () => _activeMoveInput = Vector2.zero;
        UniversalInputManager.OnPrimaryInteract += Interact;
        UniversalInputManager.OnSecondaryInteract += ToggleSynthesis;


        // << Find SceneBounds >>
        SceneBounds[] bounds = FindObjectsByType<SceneBounds>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (bounds.Length > 0)
        {
            sceneBounds = bounds[0];
        }
        else
        {
            sceneBounds = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void OnDestroy()
    {
        UniversalInputManager.OnMoveInput -= (Vector2 input) => _activeMoveInput = input;
        UniversalInputManager.OnMoveInputCanceled -= () => _activeMoveInput = Vector2.zero;
        UniversalInputManager.OnPrimaryInteract -= Interact;
        UniversalInputManager.OnSecondaryInteract -= ToggleSynthesis;
    }

    void HandleMovement()
    {
        // If the player is in an interaction state, do not allow movement
        if (stateMachine.CurrentState == PlayerState.INTERACTION) return;

        Vector2 moveDirection = _activeMoveInput; // Get the base Vec2 Input value
        moveDirection *= playerSpeed; // Scalar
        moveDirection *= moveVector; // Nullify the Y axis { Force movement on given axis only }

        // Set Target Position & Apply
        Vector3 targetPosition = transform.position + (Vector3)moveDirection;

        // Don't allow moving outside of SceneBounds
        if (sceneBounds)
        {
            if ((transform.position.x > sceneBounds.leftBound && moveDirection.x < 0) || (transform.position.x < sceneBounds.rightBound && moveDirection.x > 0))
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }

        // Update the Animation
        if (animator == null || animator.FrameAnimationPlayer == null) { Debug.Log("Player Controller has no FrameAnimationPlayer"); }
        animator.FrameAnimationPlayer.FlipTransform(moveDirection);

        // Update the State Machine
        if (moveDirection.magnitude > 0.1f)
            stateMachine.GoToState(PlayerState.WALK);
        else
            stateMachine.GoToState(PlayerState.IDLE);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Get Hidden Object Component
        Hideable_Object hiddenObject = other.GetComponent<Hideable_Object>();
        if (hiddenObject != null)
        {
            // debug.log for proof
            Debug.Log("Character is hidden");
            stateMachine.GoToState(PlayerState.HIDE);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset state to Walk/Idle 
        if (other.GetComponent<Hideable_Object>() != null)
        {
            stateMachine.GoToState(PlayerState.IDLE);
        }
    }

    /// <summary>
    /// Interaction Input has been pressed
    /// </summary>
    void Interact()
    {

        bool result = interactor.InteractWithTarget();
        if (result && stateMachine.CurrentState != PlayerState.INTERACTION)
        {
            stateMachine.GoToState(PlayerState.INTERACTION);
        }
    }

    public void ExitInteraction()
    {
        stateMachine.GoToState(PlayerState.IDLE);
    }

    #region Synthesis Management
    bool synthesisEnabled = false;
    void ToggleSynthesis()
    {
        synthesisEnabled = !synthesisEnabled;
        UIManager.Instance.synthesisManager.Show(synthesisEnabled);
        stateMachine.GoToState(synthesisEnabled ? PlayerState.INTERACTION : PlayerState.IDLE);
    }
    #endregion

}


