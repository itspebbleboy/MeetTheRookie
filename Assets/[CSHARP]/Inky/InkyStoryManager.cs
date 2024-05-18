using System;

using Ink.Runtime;

using UnityEngine;
using Darklight.UnityExt;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data. 
/// </summary>
[RequireComponent(typeof(InkyStoryLoader))]
public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>
{
    public InkyStoryLoader storyLoader => GetComponent<InkyStoryLoader>();

    public InkyStoryObject globalStoryObject;

    [SerializeField] private string _currentSpeaker;

    public override void Awake()
    {
        base.Awake();

        if (globalStoryObject != null)
        {
            InkyVariable currentSpeaker = globalStoryObject.GetVariable("CURRENT_SPEAKER");
            if (currentSpeaker != null)
                _currentSpeaker = currentSpeaker.Value.ToString();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InkyStoryManager))]
public class InkyStoryManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    InkyStoryManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (InkyStoryManager)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif