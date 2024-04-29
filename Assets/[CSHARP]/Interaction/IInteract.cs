using Darklight.Game.Grid;
using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IInteract
{
    /// <summary>
    /// Whether or not the interaction is being targeted.
    /// </summary>
    public bool isTargeted { get; set; }

    /// <summary>
    /// Whether or not the interaction is active.
    /// </summary>
    public bool isActive { get; set; }

    /// <summary>
    /// Whether or not the interaction is complete.
    /// </summary>
    public bool isComplete { get; set; }

    /// <summary>
    /// The key to use to identify the interaction.
    /// </summary>
    public string interactionKey { get; set; }

    /// <summary>
    /// The target transform for the interaction prompt UI.
    /// </summary>
    public Transform promptTarget { get; set; }

    /// <summary>
    /// Called when the player is targeting the interactable object.
    /// </summary>
    public virtual void TargetEnable()
    {
        isActive = true;

        if (promptTarget == null)
        {
            Debug.LogWarning("No prompt target set for interactable object.");
            return;
        }

        UIManager.InteractionUI.DisplayInteractPrompt(promptTarget.position);
    }

    /// <summary>
    /// Called to disable the interactable object and hide any prompts.
    /// </summary>
    public virtual void TargetDisable()
    {
        isActive = false;
        UIManager.InteractionUI.HideInteractPrompt();
    }

    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    public abstract void Interact();

    /// <summary>
    /// Called when the interaction is complete.
    /// </summary>
    public virtual void Complete()
    {
        isComplete = true;
        isActive = false;
    }

    /// <summary>
    /// Reset the interactable object to its default state.
    /// </summary>
    public virtual void Reset()
    {
        isComplete = false;
    }

    // Delegate Events
    delegate void OnInteract();
    delegate void OnComplete();

    /// <summary>
    /// Event for when the player interacts with the object.
    /// </summary>
    public event OnInteract OnInteraction;

    /// <summary>
    /// Event for when the interaction is complete.
    /// </summary>
    public event OnComplete OnCompleted;
}




