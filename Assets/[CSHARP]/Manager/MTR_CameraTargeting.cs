using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Camera;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class MTR_CameraTargeting : MonoBehaviour
{
    private InkyStoryManager _storyManager;
    private CameraController _cameraController;
    [ShowOnly, SerializeField] private string _currentSpeaker;

    void Awake()
    {
        _storyManager = InkyStoryManager.Instance;
        _cameraController = GetComponent<CameraController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Observer the current speaker variable in the story
        _storyManager.OnSpeakerSet += SetSpeakerTarget;
    }

    void SetSpeakerTarget(string speaker)
    {
        _currentSpeaker = speaker;

        // Set the Camera Target to the Player
        if (_currentSpeaker == "Lupe")
        {
            PlayerController playerController = FindFirstObjectByType<PlayerController>();
            _cameraController.SetFocusTarget(playerController.transform);
            return;
        }

        // Set the Camera Target to a NPC
        NPC_Interactable[] interactables = FindObjectsByType<NPC_Interactable>(FindObjectsSortMode.None);
        foreach (NPC_Interactable interactable in interactables)
        {
            if (interactable.speakerTag.Contains(_currentSpeaker))
            {
                _cameraController.SetFocusTarget(interactable.transform);
            }
        }
    }
}
