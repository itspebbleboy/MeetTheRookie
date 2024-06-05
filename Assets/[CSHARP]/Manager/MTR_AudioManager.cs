using Darklight.UnityExt.Audio;
using FMODUnity;
using UnityEngine;

public class MTR_AudioManager : FMODEventManager
{
    public static new MTR_AudioManager Instance => FMODEventManager.Instance as MTR_AudioManager;


    [Header("Footstep Audio")]
    public EventReference footstepEventReference;
    [Range(0.1f, 1f)] public float footstepInterval = 0.5f;
    public void StartFootstepEvent()
    {
        StartRepeatingEvent(footstepEventReference, footstepInterval);
    }

    [Header("Interaction Audio")]
    public EventReference firstInteractionEventReference;
    public EventReference continuedInteractionEventReference;
    public EventReference endInteractionEventReference;

    public void PlayFirstInteractionEvent()
    {
        PlayOneShot(firstInteractionEventReference);
    }

    public void PlayContinuedInteractionEvent()
    {
        PlayOneShot(continuedInteractionEventReference);
    }

    public void PlayEndInteractionEvent()
    {
        PlayOneShot(endInteractionEventReference);
    }


}
