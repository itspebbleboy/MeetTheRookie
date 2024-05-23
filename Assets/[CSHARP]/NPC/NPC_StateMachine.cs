using System.Collections;
using System.Collections.Generic;
using Darklight.Utility;
using UnityEngine;

public enum NPCState { NONE, IDLE, WALK, SPEAK, FOLLOW, HIDE, CHASE, PLAY_ANIMATION }

public class NPC_StateMachine : FiniteStateMachine<NPCState>
{
    public NPC_Controller controller;
    public NPC_Animator animator;

    /// <summary>
    /// Constructor for the NPC State Machine
    /// </summary>
    /// <param name="possibleStates"></param>
    /// <param name="initialState"></param>
    /// <param name="args">
    ///     args[0] = NPC_Controller
    ///     args[1] = NPC_Animator
    /// </param>
    public NPC_StateMachine(Dictionary<NPCState, FiniteState<NPCState>> possibleStates, NPCState initialState, params object[] args) : base(possibleStates, initialState, args)
    {
        controller = (NPC_Controller)args[0];
        animator = (NPC_Animator)args[1];

        currentState = null;
    }

    public override void Step()
    {
        base.Step();
    }

    public override bool GoToState(NPCState newState)
    {
        bool result = base.GoToState(newState);
        if (result)
            animator.FrameAnimationPlayer.LoadSpriteSheet(animator.GetSpriteSheetWithState(newState));
        return result;
    }
}

#region ================== [ IDLE STATE ] ==================

// IDLE
public class IdleState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;
    private readonly MonoBehaviour _coroutineRunner;
    private Coroutine coroutine = null;
    private readonly float _maxDuration;

    private readonly bool _idleWalkLoop;

    /// <param name="args">
    ///     args[0] = NPC_StateMachine (_stateMachine)
    ///     args[1] = MonoBehaviour (_coroutineRunner)
    ///     args[2] = float (_maxDuration)
    ///     args[3] = bool  (_idleWalkLoop)
    /// </param>
    public IdleState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
        _coroutineRunner = (MonoBehaviour)args[1];
        _maxDuration = (float)args[2];
        _idleWalkLoop = (bool)args[3];
    }

    public override void Enter()
    {
        if (_maxDuration == 0) { _stateMachine.GoToState(NPCState.WALK); }
        
        if (_idleWalkLoop) { coroutine = _coroutineRunner.StartCoroutine(IdleTimer()); }
    }

    public override void Exit()
    {
        if (coroutine != null)
            _coroutineRunner.StopCoroutine(coroutine);
        coroutine = null;
    }

    public override void Execute() { }

    private IEnumerator IdleTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, _maxDuration));
        _stateMachine.GoToState(NPCState.WALK);
    }
}

#endregion

#region ================== [ WALK STATE ] ==================

// WALK
public class WalkState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;
    private readonly float _maxDuration;
    private int _walkDirection = 1;
    private readonly float _leftBound;
    private readonly float _rightBound;
    private readonly float _walkSpeed;
    private readonly bool _idleWalkLoop;

    private readonly MonoBehaviour _coroutineRunner;
    private Coroutine coroutine = null;

    /// <param name="args">
    ///   args[0] = FiniteStateMachine<NPCState> (_stateMachine)
    ///   args[1] = MonoBehaviour (_coroutineRunner)
    ///   args[2] = float (_walkSpeed)
    ///   args[3] = float (_maxDuration)
    ///   args[4] = float (_leftBound)
    ///   args[5] = float (_rightBound)
    ///   args[6] = bool  (_idleWalkLoop)
    /// </param>
    public WalkState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
        _coroutineRunner = (MonoBehaviour)args[1];
        _walkSpeed = (float)args[2];
        _maxDuration = (float)args[3];
        _leftBound = (float)args[4];
        _rightBound = (float)args[5];
        _idleWalkLoop = (bool)args[6];
    }

    public override void Enter()
    {
        NPC_Animator _animator = _stateMachine.animator;

        // When walking, it can be either direction randomly
        _walkDirection = (Random.Range(0, 2) == 0) ? -1 : 1;

        if (_idleWalkLoop) { coroutine = _coroutineRunner.StartCoroutine(WalkTimer()); }
    }

    public override void Exit()
    {
        if (coroutine != null)
        {
            _coroutineRunner.StopCoroutine(coroutine);
        }
        coroutine = null;
    }

    public override void Execute()
    {

        Transform transform = _stateMachine.controller.transform;
        float movement = _walkDirection * _walkSpeed;
        float targetX = transform.position.x + movement;

        // If we're going out of bounds, flip directions
        if (targetX < _leftBound || targetX > _rightBound)
        {
            _walkDirection *= -1;
            movement = _walkDirection * _walkSpeed;
            targetX = transform.position.x + movement;
        }

        // If we are already out of bounds, we want to walk back in bounds
        if (transform.position.x <= _leftBound)
        {
            _walkDirection = 1;
            movement = _walkDirection * _walkSpeed;
            targetX = transform.position.x + movement;
        }
        if (transform.position.x >= _rightBound)
        {
            _walkDirection = -1;
            movement = _walkDirection * _walkSpeed;
            targetX = transform.position.x + movement;
        }

        // move the character
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime);

        // Update the Animation
        _stateMachine.animator.FrameAnimationPlayer.FlipTransform(new Vector2(-_walkDirection, 0));
    }

    private IEnumerator WalkTimer()
    {
        yield return new WaitForSeconds(Random.Range(0, _maxDuration));
        _stateMachine.GoToState(NPCState.IDLE);
    }
}

#endregion

#region ================== [ SPEAK STATE ] ==================

public class SpeakState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;

    public SpeakState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
    }

    public override void Enter()
    {
        /*
        Vector2 playerPos = _stateMachine.controller.player.transform.position;
        Vector2 npcPos = _stateMachine.controller.transform.position;

        // Set NPC to face player when speaking
        _stateMachine.animator.FrameAnimationPlayer.FlipTransform(new Vector2(npcPos.x < playerPos.x ? -1 : 1, 0));
        */
    }
    public override void Exit() { }
    public override void Execute()
    {


    }
}

#endregion

#region ================== [ FOLLOW STATE ] ==================

public class FollowState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;
    private MonoBehaviour _coroutineRunner;
    private Coroutine coroutine = null;
    private NPC_Animator _animator;
    private NPC_Controller _controller;
    private GameObject player;

    private bool movingInFollowState = false;
    private float currentFollowDistance = 0;

    private float followDistance;
    private float followSpeed;

    /// <param name="args">
    ///    args[0] = NPC_StateMachine (ref _stateMachine)
    ///    args[1] = MonoBehaviour (_coroutineRunner)
    ///    args[2] = float (followDistance)
    ///    args[3] = float (followSpeed)
    /// </param>
    public FollowState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
        _coroutineRunner = (MonoBehaviour)args[1];
        followDistance = (float)args[2];
        followSpeed = (float)args[3];
    }

    public override void Enter()
    {
        if (player == null) { player = _stateMachine.controller.player; }

         coroutine = _coroutineRunner.StartCoroutine(FollowCheck());
    }

    public override void Exit()
    {
        if (coroutine != null)
        {
            _coroutineRunner.StopCoroutine(coroutine);
        }
        coroutine = null;
    }

    public override void Execute()
    {
        int followDirection = (currentFollowDistance < 0) ? -1 : 1;
        _stateMachine.animator.FrameAnimationPlayer.FlipTransform(new Vector2(-followDirection, 0));

        if (movingInFollowState)
        {
            NPC_Controller npc = _stateMachine.controller;

            float movement = followDirection * followSpeed;
            float targetX = npc.transform.position.x + movement;

            // move the character
            npc.transform.position = Vector3.Lerp(npc.transform.position, new Vector3(targetX, npc.transform.position.y, npc.transform.position.z), Time.deltaTime);
        }
    }

    private IEnumerator FollowCheck()
    {
        for (; ; )
        {
            // check 20 times per second
            yield return new WaitForSeconds(1 / 20);

            Vector3 playerPos = _stateMachine.controller.player.transform.position;
            Vector3 npcPos = _stateMachine.controller.transform.position;
            currentFollowDistance = playerPos.x - npcPos.x;
            bool beyondFollowDistance = Mathf.Abs(currentFollowDistance) > followDistance;

            // If NPC is moving and they're within distance, stop
            if (!movingInFollowState && beyondFollowDistance)
            {
                movingInFollowState = true;
                _stateMachine.animator.FrameAnimationPlayer.LoadSpriteSheet(_stateMachine.animator.GetSpriteSheetWithState(NPCState.WALK));
            }

            // If NPC not moving and they're outside distance, start
            else if (movingInFollowState && !beyondFollowDistance)
            {
                movingInFollowState = false;
                _stateMachine.animator.FrameAnimationPlayer.LoadSpriteSheet(_stateMachine.animator.GetSpriteSheetWithState(NPCState.IDLE));
            }
        }
    }
}

#endregion

#region ================== [ HIDE STATE ] ==================

public class HideState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;
    private MonoBehaviour _coroutineRunner;
    private Coroutine coroutine = null;

    private Hideable_Object[] hideableObjects;
    private GameObject closestHideableObject;
    private bool areThereHideableObjects = false;
    private bool movingInHideState = false;
    private readonly float validHideDistance = 0.01f;

    private float hideSpeed;

    /// <param name="args">
    ///   args[0] = NPC_StateMachine (_stateMachine)
    ///   args[1] = MonoBehaviour (_coroutineRunner)
    ///   args[2] = float (hideSpeed)
    /// </param>
    public HideState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
        _coroutineRunner = (MonoBehaviour)args[1];
        hideSpeed = (float)args[2];
    }

    public override void Enter()
    {
        coroutine = _coroutineRunner.StartCoroutine(HideCheck());
    }

    public override void Exit()
    {
        coroutine = _coroutineRunner.StartCoroutine(HideCheck());
        coroutine = null;
    }

    public override void Execute()
    {
        if (movingInHideState)
        {
            Transform npc = _stateMachine.controller.transform;
            Vector3 npcPos = _stateMachine.controller.transform.position;
            Vector3 hideObjectPos = closestHideableObject.transform.position;
            float currentHideDistance = hideObjectPos.x - npcPos.x;
            int hideDirection = (currentHideDistance < 0) ? -1 : 1;

            float movement = hideDirection * hideSpeed;
            float targetX = npcPos.x + movement;

            // move the character
            npc.transform.position = Vector3.Lerp(npc.transform.position,
                new Vector3(targetX, npc.transform.position.y, npc.transform.position.z),
                Time.deltaTime);
            _stateMachine.animator.FrameAnimationPlayer.FlipTransform(new Vector2(-hideDirection, 0));
        }
    }

    private IEnumerator HideCheck()
    {
        int hideCheckCount = 0;
        for (; ; )
        {
            hideableObjects = GameObject.FindObjectsByType<Hideable_Object>(FindObjectsSortMode.None);

            // find the closest hideable object, only if there are any
            if (hideableObjects != null && hideableObjects.Length > 0)
            {
                areThereHideableObjects = true;

                GameObject currentClosest = hideableObjects[0].gameObject;
                float currentPos = _stateMachine.controller.transform.position.x;
                float currentClosestDistance = Mathf.Abs(currentPos - currentClosest.transform.position.x);

                for (int i = 1; i < hideableObjects.Length; i++)
                {
                    float newDistance = Mathf.Abs(currentPos - hideableObjects[i].transform.position.x);

                    if (newDistance < currentClosestDistance)
                    {
                        currentClosest = hideableObjects[i].gameObject;
                        currentClosestDistance = newDistance;
                    }
                }

                // set the closest hideableObject
                closestHideableObject = currentClosest;

                // Setup animation states
                bool beyondValidHideDistance = currentClosestDistance > validHideDistance;
                // we are out of range, need to start moving
                if (beyondValidHideDistance && !movingInHideState)
                {
                    movingInHideState = true;
                    _stateMachine.animator.FrameAnimationPlayer.LoadSpriteSheet(_stateMachine.animator.GetSpriteSheetWithState(NPCState.WALK));
                }
                // We just got in range, and need to stop moving
                else if (!beyondValidHideDistance && movingInHideState)
                {
                    movingInHideState = false;
                    _stateMachine.animator.FrameAnimationPlayer.LoadSpriteSheet(_stateMachine.animator.GetSpriteSheetWithState(NPCState.HIDE));
                }
            }
            else
            // if there are no hideable objects, then we need to just play the idle anim
            // but only if there used to be hideable objects or this is the first check
            {
                if (areThereHideableObjects || hideCheckCount == 0)
                {
                    _stateMachine.animator.FrameAnimationPlayer.LoadSpriteSheet(_stateMachine.animator.GetSpriteSheetWithState(NPCState.IDLE));
                    areThereHideableObjects = false;
                    movingInHideState = false;
                }
            }

            // time between checks
            yield return new WaitForSeconds(1 / 20);
            hideCheckCount++;
        }
    }
}

#endregion

#region ================== [ CHASE STATE ] ==================

public class ChaseState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;

    private float chaseSpeakDistance;
    private float chaseSpeed;

    /// <param name="args">
    ///  args[0] = NPC_StateMachine (_stateMachine)
    ///  args[1] = float (chaseSpeakDistance)
    ///  args[2] = float (chaseSpeed)
    /// </param>
    public ChaseState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
        chaseSpeakDistance = (float)args[1];
        chaseSpeed = (float)args[2];
    }

    public override void Enter() { }
    public override void Exit() { }

    public override void Execute()
    {
        Transform npc = _stateMachine.controller.transform;
        Vector2 playerPos = _stateMachine.controller.player.transform.position;
        Vector2 npcPos = _stateMachine.controller.transform.position;
        float currentChaseDistance = playerPos.x - npcPos.x;

        if (currentChaseDistance <= chaseSpeakDistance)
        {
            _stateMachine.GoToState(NPCState.SPEAK);
            return;
        }

        int chaseDirection = (currentChaseDistance < 0) ? -1 : 1;

        float movement = chaseDirection * chaseSpeed;
        float targetX = npcPos.x + movement;

        // move the character
        npc.transform.position = Vector3.Lerp(npc.transform.position, new Vector3(targetX, npc.transform.position.y, npc.transform.position.z), Time.deltaTime);
        _stateMachine.animator.FrameAnimationPlayer.FlipTransform(new Vector2(-chaseDirection, 0));
    }
}

#endregion

#region ================== [ PLAY ANIMATION STATE ] ==================
public class PlayAnimationState : FiniteState<NPCState>
{
    public NPC_StateMachine _stateMachine;
    public NPCState _returnState;

    public PlayAnimationState(NPCState stateType, params object[] args) : base(stateType, args)
    {
        _stateMachine = (NPC_StateMachine)args[0];
        _returnState = (NPCState)args[1];
    }

    public override void Enter() { }
    public override void Exit() { }
    public override void Execute()
    {
        if ( _stateMachine.animator.FrameAnimationPlayer.AnimationIsOver() ) {
            _stateMachine.GoToState(_returnState);
        }

    }
}

#endregion