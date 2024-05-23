/*
 * Last Edited by Garrett Blake
 * 4/10/2024
 */

using System.Collections.Generic;
using UnityEngine;
using Darklight.Utility;
using Darklight.UnityExt.Editor;
using System;

[RequireComponent(typeof(NPC_Animator))]
public class NPC_Controller : MonoBehaviour
{
    public NPC_StateMachine stateMachine;
    public NPC_Animator animator => GetComponent<NPC_Animator>();
    [SerializeField, ShowOnly] NPCState currentState;

    // =============== [ PUBLIC INSPECTOR VALUES ] =================== //
    public GameObject player => FindFirstObjectByType<PlayerController>().gameObject;
    public NPCState startingState = NPCState.IDLE;

    public bool idleWalkLoop = false;

    [Tooltip("State to return to after animation")]
    public NPCState stateAfterAnimation = NPCState.IDLE;

    [Tooltip("Speed for the WALK state")]
    [Range(0.1f, 1f)] public float npcSpeed = .2f;

    [Tooltip("Speed for the FOLLOW state")]
    [Range(0.1f, 1f)] public float followSpeed = .5f;

    [Tooltip("Speed for the HIDE state")]
    [Range(0.1f, 1f)] public float hideSpeed = .5f;

    [Tooltip("Speed for the CHASE state")]
    [Range(0.1f, 1f)] public float chaseSpeed = .5f;

    [Tooltip("Left bound for the WALK state")]
    public float leftBound;

    [Tooltip("Right bound for the WALK state")]
    public float rightBound;

    [Tooltip("Distance the NPC will follow the player")]
    public float followDistance = 1;

    [Tooltip("Distance to cause dialogue in the SPEAK state")]
    public float chaseSpeakDistance = .75f;
    [Range(0f, 10f)] public float idleMaxDuration = 3f;
    [Range(0f, 10f)] public float walkMaxDuration = 3f;

    // ================ [ UNITY MAIN METHODS ] =================== //
    void Start()
    {
        // Create instances of the states
        IdleState idleState = new(NPCState.IDLE, new object[] { stateMachine, this, idleMaxDuration, idleWalkLoop });
        WalkState walkState = new(NPCState.WALK, new object[] { stateMachine, this, npcSpeed, walkMaxDuration, leftBound, rightBound, idleWalkLoop });
        SpeakState speakState = new(NPCState.SPEAK, new object[] { stateMachine });
        FollowState followState = new(NPCState.FOLLOW, new object[] { stateMachine, this, followDistance, followSpeed });
        HideState hideState = new(NPCState.HIDE, new object[] { stateMachine, this, hideSpeed });
        ChaseState chaseState = new(NPCState.CHASE, new object[] { stateMachine, chaseSpeakDistance, chaseSpeed });
        PlayAnimationState playAnimationState= new(NPCState.PLAY_ANIMATION, new object[] { stateMachine, stateAfterAnimation});

        // Create dictionary to hold the possible states
        Dictionary<NPCState, FiniteState<NPCState>> possibleStates = new()
        {
            { NPCState.IDLE, idleState },
            { NPCState.WALK, walkState },
            { NPCState.SPEAK, speakState },
            { NPCState.FOLLOW, followState },
            { NPCState.HIDE, hideState },
            { NPCState.CHASE, chaseState },
            { NPCState.PLAY_ANIMATION, playAnimationState}, 
        };

        // Create the NPCStateMachine
        stateMachine = new(possibleStates, startingState, this, animator);


        // Hacky solution to fix null reference bug, setting the stateMachine field for each state
        idleState._stateMachine = stateMachine;
        walkState._stateMachine = stateMachine;
        speakState._stateMachine = stateMachine;
        followState._stateMachine = stateMachine;
        hideState._stateMachine = stateMachine;
        chaseState._stateMachine = stateMachine;
        playAnimationState._stateMachine = stateMachine;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Step();
        currentState = stateMachine.CurrentState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(leftBound, this.GetComponent<Transform>().position.y, 0), new Vector3(rightBound, this.GetComponent<Transform>().position.y, 0));
    }

    // This is a workaround because you cannot call FindObjectsByType on a reference to this
    public Hideable_Object[] FindHideableObjects()
    {
        return FindObjectsByType<Hideable_Object>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }
}