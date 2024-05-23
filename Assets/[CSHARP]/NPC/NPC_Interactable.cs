using UnityEngine;
using Darklight.UnityExt.Editor;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NPC_Controller))]
public class NPC_Interactable : Interactable, IInteract
{
    private NPCState stateBeforeTalkedTo = NPCState.IDLE;
    NPC_StateMachine stateMachine => GetComponent<NPC_Controller>().stateMachine;


    [Header("NPC : Speech Bubble")]
    private List<string> _speakerOptions => InkyStoryManager.Instance.SpeakerList;

    [DropdownAttribute("_speakerOptions")]
    public string speakerTag;

    public void Start()
    {
        Reset();

        // >> ON FIRST INTERACTION -------------------------------
        this.OnFirstInteraction += () => stateBeforeTalkedTo = stateMachine.CurrentState;

        // >> ON INTERACT ---------------------------------------
        // NOTE :: This event is only called when an Interaction is confirmed
        this.OnInteraction += (string text) =>
        {

        };
    }

    public override void Interact()
    {
        base.Interact();

        if (isComplete) return;
        if (_storyIterator.CurrentState != InkyStoryIterator.State.END)
        {
            // If the statemachine is not null, go to the speak state
            stateMachine?.GoToState(NPCState.SPEAK);
        }
    }

    public override void Complete()
    {
        base.Complete();



        // If the statemachine is not null, go to the state before talked to
        stateMachine?.GoToState(stateBeforeTalkedTo);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NPC_Interactable))]
public class InteractableNPCCustomEditor : InteractableCustomEditor
{
    public override void OnInspectorGUI()
    {
        NPC_Interactable interactableNPC = (NPC_Interactable)target;

        base.OnInspectorGUI();
    }
}
#endif