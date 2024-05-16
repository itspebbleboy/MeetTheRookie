using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : OverlapGrid2D
{
    public PlayerController playerController => GetComponent<PlayerController>();
    protected HashSet<IInteract> interactables = new HashSet<IInteract>();
    [SerializeField, ShowOnly] int _interactablesCount;


    public override void Update()
    {
        base.Update();
        RefreshRadar();

        if (interactables.Count == 0) return;
        // Because this method is called every frame, this line will keep the target at the correct position
        interactables.First().TargetSet();
    }

    void RefreshRadar()
    {
        if (interactables.Count == 0) return;

        // Temporary list to hold items to be removed
        List<IInteract> toRemove = new List<IInteract>();

        // Update the interaction count
        _interactablesCount = interactables.Count;

        foreach (IInteract interactable in interactables)
        {
            if (interactable == null) continue;
            if (interactable.isComplete)
            {
                // Mark the interaction for removal
                toRemove.Add(interactable);
                interactable.TargetClear();
            }
        }

        // Remove the completed interactions from the HashSet
        foreach (IInteract completedInteraction in toRemove)
        {
            interactables.Remove(completedInteraction);
        }
    }

    public bool InteractWithTarget()
    {
        if (interactables.Count == 0) return false;

        // Get the Target Interactable
        IInteract targetInteractable = interactables.First();
        targetInteractable.TargetClear();
        if (targetInteractable.isComplete) return false;

        targetInteractable.Interact(); // << MAIN INTERACTION

        // Check if the target is complete
        if (targetInteractable.isComplete)
        {
            UIManager.Instance.DestroySpeechBubble();
            playerController.ExitInteraction();
            interactables.Remove(targetInteractable);
        }

        if (targetInteractable is NPC_Interactable)
        {
            NPC_Interactable npcInteractable = targetInteractable as NPC_Interactable;
            playerController.cameraController.SetOffsetRotation(playerController.transform, npcInteractable.transform);
            //npcInteractable.DialogueBubble.TextureUpdate();
        }
        // Show the player's dialogue bubble
        else if (targetInteractable is Interactable)
        {
            Interactable interactable = targetInteractable as Interactable;
            OverlapGrid2D_Data targetData = GetBestData();
            UIManager.Instance.CreateSpeechBubble(targetData.worldPosition, interactable.currentText, targetData.cellSize);
        }

        return true;
    }


    public Vector3 GetMidpoint(Vector3 point1, Vector3 point2)
    {
        return (point1 + point2) * 0.5f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        Debug.Log($"Player Interactor :: {interactable}");

        if (interactable == null) return;
        if (interactable.isComplete) return;
        interactables.Add(interactable);
    }


    void OnTriggerExit2D(Collider2D other)
    {
        IInteract interactable = other.GetComponent<IInteract>();
        if (interactable == null) return;
        interactables.Remove(interactable);
        interactable.TargetClear();
    }
}
