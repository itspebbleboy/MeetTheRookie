using System;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.SceneManagement;
using FMODUnity;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class MTR_SceneData : BuildSceneData
{
    private List<string> _knotNames
    {
        get
        {
            List<string> names = new List<string>();
            if (InkyStoryManager.Instance != null)
            {
                return InkyStoryManager.GlobalStoryObject.KnotNames;
            }
            return names;
        }
    }

    [NaughtyAttributes.Dropdown("_knotNames")]
    public string knot;

    public EventReference backgroundMusicEvent;
}

/// <summary>
/// Custom Scriptable object to hold MTR_SceneData.
/// </summary>
public class MTR_SceneDataObject : BuildSceneDataObject<MTR_SceneData>
{
    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetAllBuildSceneData().Find(x => x.knot == knot);
    }

    public EventReference GetActiveBackgroundMusicEvent()
    {
        MTR_SceneData data = GetActiveSceneData();
        return data.backgroundMusicEvent;
    }
}

/// <summary>
/// This is the Custom Scene Manager for Meet The Rookie
/// </summary>
public class MTR_SceneManager : BuildSceneDataManager<MTR_SceneData>
{
    MTR_SceneDataObject _mtrSceneDataObject
    {
        get
        {
            return buildSceneDataObject as MTR_SceneDataObject;
        }
        set
        {
            buildSceneDataObject = value;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        InkyStoryManager.Instance.OnStoryInitialized += OnStoryInitialized;
    }

    public override void CreateBuildSceneDataObject()
    {
        _mtrSceneDataObject =
            ScriptableObjectUtility.CreateOrLoadScriptableObject<MTR_SceneDataObject>(
                DATA_PATH,
                DATA_FILENAME
            );
    }

    public void OnStoryInitialized(Story story)
    {
        Debug.Log($"{Prefix} >> STORY INITIALIZED EVENT: {story}");
        story.BindExternalFunction(
            "ChangeGameScene",
            (string knotName) => ChangeGameScene(knotName)
        );
    }

    /// <summary>
    /// This is the main external function that is called from the Ink story to change the game scene.
    /// </summary>
    /// <param name="args">0 : The name of the sceneKnot</param>
    /// <returns>False if BuildSceneData is null. True if BuildSceneData is valid.</returns>
    public object ChangeGameScene(string knotName)
    {
        MTR_SceneData data = _mtrSceneDataObject.GetSceneDataByKnot(knotName);

        if (data == null)
            return false;

        // << LOAD SCENE >>
        LoadScene(data.Name);
        return true;
    }

    public MTR_SceneData GetSceneData(Scene sceneName)
    {
        return _mtrSceneDataObject.GetSceneData(sceneName);
    }

    public MTR_SceneData GetActiveSceneData()
    {
        if (_mtrSceneDataObject == null)
            return new MTR_SceneData();
        return _mtrSceneDataObject.GetActiveSceneData();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MTR_SceneManager))]
public class MTR_SceneManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MTR_SceneManager _script;

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MTR_SceneManager)target;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Initialize"))
        {
            _script.Initialize();
        }

        if (GUILayout.Button("Show Editor Window"))
        {
            BuildSceneManagerWindow.ShowWindow();
        }

        // Display the active scene name.
        MTR_SceneData activeScene = _script.GetActiveSceneData();
        if (activeScene != null)
        {
            CustomInspectorGUI.CreateTwoColumnLabel("Active Build Scene", activeScene.Name);
            CustomInspectorGUI.CreateTwoColumnLabel("Active Build Knot", activeScene.knot);
        }

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}


#endif
