using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Utility;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.UXML;
using UnityEngine;

public class SceneManager : MonoBehaviourSingleton<SceneManager>
{
    [SerializeField] private UXML_UIDocumentPreset sceneTransitionPreset;

    public void LoadScene(SceneObject sceneObject)
    {
        SceneTransition sceneTransition = new GameObject("SceneTransition").AddComponent<SceneTransition>();
        sceneTransition.Initialize(sceneTransitionPreset, new string[] { "blackborder", "textlabel" });
        sceneTransition.BeginFadeOut(sceneObject);
    }
}
