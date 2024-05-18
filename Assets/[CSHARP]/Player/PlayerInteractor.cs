using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : OverlapGrid2D
{
    public PlayerController playerController => GetComponent<PlayerController>();
    [SerializeField, ShowOnly] protected List<Interactable> _foundInteractables = new List<Interactable>();

    [ShowOnly] public Interactable targetInteractable;

    public override void Update()
    {
        base.Update();
        RefreshRadar();

        if (_foundInteractables.Count == 0) return;
        // Because this method is called every frame, this line will keep the target at the correct position
        //interactables.First().TargetSet();
    }

    void RefreshRadar()
    {
        if (_foundInteractables.Count == 0) return;

        // Temporary list to hold items to be removed
        List<Interactable> toRemove = new List<Interactable>();

        foreach (Interactable interactable in _foundInteractables)
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
        foreach (Interactable completedInteraction in toRemove)
        {
            _foundInteractables.Remove(completedInteraction);
        }
    }

    public bool InteractWithTarget()
    {
        if (_foundInteractables.Count == 0) return false;

        // Get the Target Interactable
        Interactable targetInteractable = _foundInteractables.First();
        if (targetInteractable == null) return false;
        targetInteractable.TargetClear();
        targetInteractable.Interact(); // << MAIN INTERACTION

        if (targetInteractable is NPC_Interactable)
        {
            NPC_Interactable npcInteractable = targetInteractable as NPC_Interactable;
            //playerController.cameraController.SetOffsetRotation(playerController.transform, npcInteractable.transform);
            //npcInteractable.DialogueBubble.TextureUpdate();
        }
        // Show the player's dialogue bubble
        else if (targetInteractable is Interactable)
        {
            Interactable interactable = targetInteractable as Interactable;
            OverlapGrid2D_Data targetData = GetBestData();
            UIManager.Instance.CreateSpeechBubble(targetData.worldPosition, interactable.currentText, targetData.cellSize);
        }

        if (targetInteractable.isComplete)
        {
            ExitInteraction();
            return false;
        }

        return true;
    }

    public void ExitInteraction()
    {
        UIManager.Instance.DestroySpeechBubble();
        playerController.ExitInteraction();
    }


    public Vector3 GetMidpoint(Vector3 point1, Vector3 point2)
    {
        return (point1 + point2) * 0.5f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        Debug.Log($"Player Interactor :: {interactable}");

        if (interactable == null) return;
        if (interactable.isComplete) return;
        _foundInteractables.Add(interactable);
        interactable.TargetSet();

    }


    void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable == null) return;
        _foundInteractables.Remove(interactable);
        interactable.TargetClear();
    }
}
