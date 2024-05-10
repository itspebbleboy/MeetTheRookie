public interface IInteract
{

    #region ------------------ >> STATE FLAGS
    /// <summary>
    /// Whether or not the interaction is being targeted.
    /// </summary>
    public bool isTarget { get; set; }

    /// <summary>
    /// Whether or not the interaction is allowed to be interacted with.
    /// </summary>
    public bool isActive { get; set; }

    /// <summary>
    /// Whether or not the interaction is complete.
    /// </summary>
    public bool isComplete { get; set; }
    #endregion

    /// <summary>
    /// The key to use to identify the interaction.
    /// </summary>
    public string interactionKey { get; }


    // ===================== [[ INTERACTION METHODS ]] =====================    
    /// <summary>
    /// Called when the player is targeting the interactable object.
    /// </summary>
    public virtual void TargetSet()
    {
        isTarget = true;
    }

    /// <summary>
    /// Called to disable the interactable object and hide any prompts.
    /// </summary>
    public virtual void TargetClear()
    {
        isTarget = false;
    }

    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    public virtual void Interact()
    {
        isActive = true;
    }

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

    // ===================== [[ EVENTS ]] =====================
    // Delegate Events
    delegate void OnFirstInteract();
    delegate void OnInteract(string currentText);
    delegate void OnComplete();

    /// <summary>
    /// Event for the first time the player interacts with an object, resets on OnCompleted
    /// </summary>
    public event OnFirstInteract OnFirstInteraction;

    /// <summary>
    /// Event for when the player interacts with the object.
    /// </summary>
    public event OnInteract OnInteraction;

    /// <summary>
    /// Event for when the interaction is complete.
    /// </summary>
    public event OnComplete OnCompleted;
}