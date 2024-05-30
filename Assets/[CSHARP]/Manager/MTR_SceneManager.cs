using System.Collections.Generic;

using Darklight.UnityExt.Editor;
using Darklight.UnityExt.SceneManagement;
using Darklight.UnityExt.Inky;

using UnityEngine;
using FMODUnity;


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


public class MTR_SceneManager : BuildSceneDataManager<MTR_SceneData>
{
    public override void Initialize()
    {
        base.Initialize();

        InkyStoryManager.GlobalStoryObject.BindExternalFunction("ChangeGameScene", ChangeGameScene);
    }

    public override List<MTR_SceneData> GetAllBuildSceneData()
    {
        return base.GetAllBuildSceneData();
    }

    public MTR_SceneData GetSceneDataByKnot(string knot)
    {
        return GetAllBuildSceneData().Find(x => x.knot == knot);
    }

        /// <summary>
    /// This is the main external function that is called from the Ink story to change the game scene.
    /// </summary>
    /// <param name="args">0 : The name of the sceneKnot</param>
    /// <returns>False if BuildSceneData is null. True if BuildSceneData is valid.</returns>
    object ChangeGameScene(object[] args)
    {
        string knotName = (string)args[0];
        MTR_SceneData data = GetSceneDataByKnot(knotName);

        if (data == null) return false;

        // << LOAD SCENE >>
        LoadScene(data.Name);
        Debug.Log($"{Prefix} >> EXTERNAL FUNCTION: ChangeGameScene: {knotName}");

        return true;
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
